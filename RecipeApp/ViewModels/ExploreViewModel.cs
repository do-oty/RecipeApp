using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using RecipeApp.Models;
using RecipeApp.Services;

namespace RecipeApp.ViewModels
{
    public partial class ExploreViewModel : ObservableObject
    {
        private readonly IMealService _mealService;
        private List<Meal> _allMeals = new();

        [ObservableProperty]
        private ObservableCollection<Meal> meals = new();
        [ObservableProperty]
        private ObservableCollection<string> tags = new();
        [ObservableProperty]
        private ObservableCollection<string> selectedTags = new();
        [ObservableProperty]
        private ObservableCollection<string> filteredTags = new();
        [ObservableProperty]
        private string searchQuery = string.Empty;
        [ObservableProperty]
        private string selectedTag;
        [ObservableProperty]
        private bool isRefreshing;

        public ICommand GoBackCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LoadRandomMealsCommand { get; }

        public ExploreViewModel(IMealService mealService)
        {
            _mealService = mealService;
            Meals = new ObservableCollection<Meal>();
            Tags = new ObservableCollection<string>();
            SelectedTags = new ObservableCollection<string>();
            FilteredTags = new ObservableCollection<string>();
            SearchQuery = string.Empty;
            SelectedTag = string.Empty;
            IsRefreshing = false;

            GoBackCommand = new Command(async () => await GoBack());
            SearchCommand = new Command(async () => await Search());
            AddTagCommand = new Command<string>(AddTag);
            RemoveTagCommand = new Command<string>(RemoveTag);
            RefreshCommand = new Command(async () => await Refresh());
            LoadRandomMealsCommand = new Command(async () => await LoadRandomMeals());

            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            // You can move the tag/category/area loading logic here if needed
            await LoadRandomMeals();
        }

        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..", true);
        }

        private async Task Search()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await LoadRandomMeals();
                return;
            }

            try
            {
                var searchResults = await _mealService.SearchMealsAsync(SearchQuery);
                _allMeals = searchResults.ToList();
                FilterMeals();
            }
            catch { }
        }

        private void FilterMeals()
        {
            if (_allMeals == null || !_allMeals.Any())
            {
                Meals.Clear();
                return;
            }

            var filteredMeals = _allMeals.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredMeals = filteredMeals.Where(m =>
                    m.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    m.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    m.Area.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedTags.Any())
            {
                filteredMeals = filteredMeals.Where(m =>
                {
                    return SelectedTags.Any(tag =>
                    {
                        if (m.Category.Equals(tag, StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (m.Area.Equals(tag, StringComparison.OrdinalIgnoreCase))
                            return true;
                        if (!string.IsNullOrEmpty(m.Tags))
                        {
                            var mealTags = m.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                               .Select(t => t.Trim());
                            return mealTags.Any(mealTag =>
                                mealTag.Equals(tag, StringComparison.OrdinalIgnoreCase));
                        }
                        return false;
                    });
                });
            }

            Meals.Clear();
            foreach (var meal in filteredMeals)
            {
                Meals.Add(meal);
            }
        }

        private void FilterTags()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredTags = new ObservableCollection<string>(Tags);
            }
            else
            {
                var filtered = Tags.Where(t => t.ToLower().Contains(SearchQuery.ToLower())).ToList();
                FilteredTags = new ObservableCollection<string>(filtered);
            }
        }

        private void AddTag(string tag)
        {
            if (!SelectedTags.Contains(tag))
            {
                SelectedTags.Add(tag);
            }
        }

        private void RemoveTag(string tag)
        {
            SelectedTags.Remove(tag);
        }

        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadRandomMeals();
            IsRefreshing = false;
        }

        private async Task LoadRandomMeals()
        {
            try
            {
                var randomMeals = await _mealService.GetRandomMealsAsync(10);
                _allMeals = randomMeals.ToList();
                FilterMeals();
            }
            catch { }
        }
    }
} 
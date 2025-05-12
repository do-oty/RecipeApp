using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Input;
using RecipeApp.Services;
using RecipeApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using RecipeApp.ViewModels;

namespace RecipeApp.Pages
{
    [QueryProperty(nameof(SelectedTag), "SelectedTag")]
    public partial class ExplorePage : ContentPage
    {
        private readonly IMealService _mealService;
        private ObservableCollection<string> _tags = new();
        private ObservableCollection<string> _selectedTags = new();
        private ObservableCollection<string> _filteredTags = new();
        private string _searchQuery = string.Empty;
        private string _selectedTag;
        private bool _isRefreshing;
        private List<Meal> _allMeals = new();

        public ObservableCollection<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> SelectedTags
        {
            get => _selectedTags;
            set
            {
                _selectedTags = value;
                OnPropertyChanged();
                FilterMeals();
            }
        }

        public ObservableCollection<string> FilteredTags
        {
            get => _filteredTags;
            set
            {
                _filteredTags = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterTags();
                FilterMeals();
            }
        }

        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                _selectedTag = value;
                if (!string.IsNullOrEmpty(value))
                {
                    AddTag(value);
                }
            }
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand LoadRandomMealsCommand { get; }

        public ExplorePage(IMealService mealService)
        {
            InitializeComponent();
            _mealService = mealService;
            BindingContext = new ExploreViewModel(_mealService);

            // Initialize commands
            GoBackCommand = new Command(async () => await GoBack());
            SearchCommand = new Command(async () => await Search());
            AddTagCommand = new Command<string>(AddTag);
            RemoveTagCommand = new Command<string>(RemoveTag);
            RefreshCommand = new Command(async () => await Refresh());
            LoadRandomMealsCommand = new Command(async () => await LoadRandomMeals());

            // Initialize collections
            SelectedTags = new ObservableCollection<string>();

            // Load initial data
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            try
            {
                IsBusy = true;
                var categories = await _mealService.GetCategoriesAsync();
                var areas = await _mealService.GetAreasAsync();

                if (categories == null || !categories.Any())
                {
                    await DisplayAlert("Error", "Failed to load categories", "OK");
                    return;
                }

                if (areas == null || !areas.Any())
                {
                    await DisplayAlert("Error", "Failed to load areas", "OK");
                    return;
                }

                // Create tag buttons for categories
                foreach (var category in categories)
                {
                    var tagButton = new Border
                    {
                        BackgroundColor = Color.FromArgb("#5b8224"),
                        StrokeShape = new RoundRectangle { CornerRadius = 15 },
                        Padding = new Thickness(10, 5),
                        Margin = new Thickness(5, 0)
                    };

                    var label = new Label
                    {
                        Text = category.Name,
                        TextColor = Colors.White,
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    tagButton.Content = label;

                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += (s, e) => OnTagTapped(category.Name);
                    tagButton.GestureRecognizers.Add(tapGesture);

                    TagsContainer.Children.Add(tagButton);
                }

                // Create tag buttons for areas
                foreach (var area in areas)
                {
                    var tagButton = new Border
                    {
                        BackgroundColor = Color.FromArgb("#D98236"),
                        StrokeShape = new RoundRectangle { CornerRadius = 15 },
                        Padding = new Thickness(10, 5),
                        Margin = new Thickness(5, 0)
                    };

                    var label = new Label
                    {
                        Text = area,
                        TextColor = Colors.White,
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    };

                    tagButton.Content = label;

                    var tapGesture = new TapGestureRecognizer();
                    tapGesture.Tapped += (s, e) => OnTagTapped(area);
                    tagButton.GestureRecognizers.Add(tapGesture);

                    TagsContainer.Children.Add(tagButton);
                }

                // Load initial meals
                await LoadRandomMeals();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
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
                IsBusy = true;
                var searchResults = await _mealService.SearchMealsAsync(SearchQuery);
                
                if (!searchResults.Any())
                {
                    await DisplayAlert("No Results", $"No meals found matching '{SearchQuery}'", "OK");
                    return;
                }

                _allMeals = searchResults;
                FilterMeals();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to search meals: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterMeals()
        {
            if (_allMeals == null || !_allMeals.Any())
            {
                return;
            }

            var filteredMeals = _allMeals.AsEnumerable();

            // Apply search query filter
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredMeals = filteredMeals.Where(m => 
                    m.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    m.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    m.Area.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            // Apply tag filters
            if (SelectedTags.Any())
            {
                filteredMeals = filteredMeals.Where(m =>
                {
                    // Check if meal matches any of the selected tags
                    return SelectedTags.Any(tag =>
                    {
                        // Check category
                        if (m.Category.Equals(tag, StringComparison.OrdinalIgnoreCase))
                            return true;

                        // Check area (cuisine)
                        if (m.Area.Equals(tag, StringComparison.OrdinalIgnoreCase))
                            return true;

                        // Check if tag is in meal tags
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

            // Update the meals collection
            ((ExploreViewModel)BindingContext).Meals.Clear();
            foreach (var meal in filteredMeals)
            {
                ((ExploreViewModel)BindingContext).Meals.Add(meal);
            }
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchQuery = e.NewTextValue;
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
                CreateTagButton(tag);
            }
        }

        private void RemoveTag(string tag)
        {
            SelectedTags.Remove(tag);

            // Find and remove the tag button from the UI
            var tagButton = SelectedTagsContainer.Children.FirstOrDefault(x =>
                x is Border border &&
                border.Content is Grid grid &&
                grid.Children.OfType<Label>().FirstOrDefault()?.Text == tag);

            if (tagButton != null)
            {
                SelectedTagsContainer.Children.Remove(tagButton);
            }
        }

        private void CreateTagButton(string tag)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            var label = new Label
            {
                Text = tag,
                TextColor = Colors.White,
                FontSize = 12,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var button = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                Stroke = Color.FromArgb("#5b8224"),
                StrokeThickness = 0,
                BackgroundColor = Color.FromArgb("#5b8224"),
                Padding = new Thickness(8, 4),
                Content = label,
                Margin = new Thickness(4)
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => AddTag(tag);
            button.GestureRecognizers.Add(tapGesture);

            TagsContainer.Children.Add(button);
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
                IsBusy = true;
                var randomMeals = await _mealService.GetRandomMealsAsync(10);
                
                if (!randomMeals.Any())
                {
                    await DisplayAlert("Error", "Failed to load random meals. Please check your internet connection and try again.", "OK");
                    return;
                }

                _allMeals = randomMeals;
                FilterMeals();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load random meals: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            // This method is required by the XAML but we don't need any special scroll handling here
        }

        private void OnTagTapped(string tag)
        {
            // Implementation of OnTagTapped method
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadRandomMeals();
        }
    }
}
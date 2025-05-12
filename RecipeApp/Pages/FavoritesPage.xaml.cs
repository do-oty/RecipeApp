using System.Collections.ObjectModel;
using System.Windows.Input;
using RecipeApp.Models;
using RecipeApp.Services;
using Microsoft.Maui.Controls;

namespace RecipeApp.Pages
{
    public partial class FavoritesPage : ContentPage
    {
        private readonly IFavoriteService _favoriteService = App.Services.GetService<IFavoriteService>();
        private readonly IMealService _mealService = App.Services.GetService<IMealService>();
        private readonly RecipeApp.ViewModels.MealViewModel _mealViewModel = App.Services.GetService<RecipeApp.ViewModels.MealViewModel>();

        private ObservableCollection<Meal> _favorites = new();
        private ObservableCollection<Meal> _filteredFavorites = new();
        private string _searchQuery = string.Empty;
        private string _searchResultsText = string.Empty;

        public ObservableCollection<Meal> Favorites
        {
            get => _favorites;
            set { _favorites = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Meal> FilteredFavorites
        {
            get => _filteredFavorites;
            set { _filteredFavorites = value; OnPropertyChanged(); }
        }
        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); FilterFavorites(); }
        }
        public string SearchResultsText
        {
            get => _searchResultsText;
            set { _searchResultsText = value; OnPropertyChanged(); }
        }

        public ICommand RemoveFromFavoritesCommand { get; }
        public ICommand ViewRecipeCommand { get; }
        public ICommand ShowFilterModalCommand { get; }

        public FavoritesPage()
        {
            InitializeComponent();
            BindingContext = this;
            RemoveFromFavoritesCommand = new Command<Meal>(async (meal) => await RemoveFromFavorites(meal));
            ViewRecipeCommand = new Command<Meal>(async (meal) => await OnViewRecipe(meal));
            ShowFilterModalCommand = new Command(async () => await ShowFilterModal());
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFavoritesAsync();
        }

        private async Task LoadFavoritesAsync()
        {
            var favoriteIds = await _favoriteService.GetFavoriteRecipeIdsAsync();
            System.Diagnostics.Debug.WriteLine($"FavoritesPage: Loaded favoriteIds: {string.Join(",", favoriteIds)}");
            var favoriteMeals = new List<Meal>();
            foreach (var id in favoriteIds)
            {
                var meal = await _mealService.GetMealByIdAsync(id);
                if (meal != null)
                    favoriteMeals.Add(meal);
            }
            System.Diagnostics.Debug.WriteLine($"FavoritesPage: Loaded {favoriteMeals.Count} meals from MealDB");
            Favorites = new ObservableCollection<Meal>(favoriteMeals);
            FilteredFavorites = new ObservableCollection<Meal>(favoriteMeals);
            UpdateSearchResultsText();
        }

        private async Task RemoveFromFavorites(Meal meal)
        {
            if (meal == null) return;
            await _favoriteService.RemoveFavoriteAsync(meal.IdMeal);
            await LoadFavoritesAsync();
            Favorites.Remove(meal);
            FilteredFavorites.Remove(meal);
            UpdateSearchResultsText();
        }

        private async Task OnViewRecipe(Meal meal)
        {
            if (meal == null) return;
            var navigationParameter = new Dictionary<string, object>
            {
                { "idMeal", meal.IdMeal }
            };
            await Shell.Current.GoToAsync("RecipeDetailsPage", navigationParameter);
        }

        private void FilterFavorites()
        {
            var filtered = Favorites.Where(meal =>
                string.IsNullOrWhiteSpace(SearchQuery) ||
                meal.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                (meal.Category != null && meal.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
            );
            FilteredFavorites = new ObservableCollection<Meal>(filtered);
            UpdateSearchResultsText();
        }

        private void UpdateSearchResultsText()
        {
            string text = $"{FilteredFavorites.Count} favorite";
            text += FilteredFavorites.Count == 1 ? " recipe" : " recipes";
            if (!string.IsNullOrWhiteSpace(SearchQuery))
                text += $" matching '{SearchQuery}'";
            SearchResultsText = text;
        }

        private async Task ShowFilterModal()
        {
            await Shell.Current.Navigation.PushModalAsync(
                new FavoritesFilterModal(Favorites.ToList(), (selectedCategories, selectedAreas) =>
                {
                    var filtered = Favorites.Where(meal =>
                        (selectedCategories.Count == 0 || selectedCategories.Contains(meal.Category)) &&
                        (selectedAreas.Count == 0 || selectedAreas.Contains(meal.Area))
                    ).ToList();
                    FilteredFavorites = new ObservableCollection<Meal>(filtered);
                    UpdateSearchResultsText();
                })
            );
        }
    }
} 
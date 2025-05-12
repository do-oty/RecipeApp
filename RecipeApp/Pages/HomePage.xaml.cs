using System.Collections.ObjectModel;
using System.Windows.Input;
using RecipeApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using RecipeApp.Models;
using RecipeApp.Services;

namespace RecipeApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private const int ItemsPerPage = 9;
        private const double HideThreshold = 10.0;

        private readonly MealViewModel _viewModel;
        private readonly IMealService _mealService;
        private readonly IFavoriteService _favoriteService = App.Services.GetService<IFavoriteService>();

        public ObservableCollection<Meal> CarouselFeatured { get; set; }
        public ObservableCollection<Meal> TrendingRecipes { get; set; }
        public Meal RecipeOfTheDay { get; set; }
        public ObservableCollection<Category> PopularCategories { get; set; }
        public ObservableCollection<string> Areas { get; set; }

        public ICommand AddToCollectionCommand { get; }
        public ICommand AddToFavoritesCommand { get; }
        public ICommand ShareRecipeCommand { get; }
        public ICommand ViewRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }
        public ICommand NavigateToExploreWithTagCommand { get; }

        public HomePage()
        {
            InitializeComponent();
            _viewModel = App.Services.GetService<MealViewModel>();
            _mealService = App.Services.GetService<IMealService>();
            BindingContext = this;

            // Initialize collections
            CarouselFeatured = new ObservableCollection<Meal>();
            TrendingRecipes = new ObservableCollection<Meal>();
            PopularCategories = new ObservableCollection<Category>();
            Areas = new ObservableCollection<string>();

            // Initialize commands
            AddToCollectionCommand = new Command<Meal>(OnAddToCollection);
            AddToFavoritesCommand = new Command<Meal>(OnAddToFavorites);
            ShareRecipeCommand = new Command<Meal>(OnShareRecipe);
            ViewRecipeCommand = new Command<Meal>(OnViewRecipe);
            SaveRecipeCommand = new Command<Meal>(OnSaveRecipe);
            NavigateToExploreWithTagCommand = new Command<string>(OnNavigateToExploreWithTag);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadHomePageDataAsync();
        }

        private async Task LoadHomePageDataAsync()
        {
            try
            {
                // Load Recipe of the Day (random meal)
                var randomMeal = await _mealService.GetRandomMealsAsync(1);
                if (randomMeal.Any())
                {
                    RecipeOfTheDay = randomMeal.First();
                    OnPropertyChanged(nameof(RecipeOfTheDay));
                }

                // Load 5 random meals for featured recipes
                CarouselFeatured.Clear();
                var randomMeals = await _mealService.GetRandomMealsAsync(5);
                foreach (var meal in randomMeals)
                {
                    CarouselFeatured.Add(meal);
                }
                OnPropertyChanged(nameof(CarouselFeatured));

                // Load 5 random meals for trending recipes
                TrendingRecipes.Clear();
                var trendingMeals = await _mealService.GetRandomMealsAsync(5);
                foreach (var meal in trendingMeals)
                {
                    TrendingRecipes.Add(meal);
                }
                OnPropertyChanged(nameof(TrendingRecipes));

                // Load Popular Categories
                var categories = await _mealService.GetCategoriesAsync();
                PopularCategories.Clear();
                foreach (var category in categories)
                {
                    PopularCategories.Add(category);
                }
                OnPropertyChanged(nameof(PopularCategories));

                // Load Areas (Tags)
                var areas = await _mealService.GetAreasAsync();
                Areas.Clear();
                foreach (var area in areas)
                {
                    Areas.Add(area);
                }
                OnPropertyChanged(nameof(Areas));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load home page data: " + ex.Message, "OK");
            }
        }

        private void OnAddToCollection(Meal meal)
        {
            // TODO: Implement add to collection
        }

        private async void OnAddToFavorites(Meal meal)
        {
            try
            {
                if (meal == null) return;
                bool isFavorite = await _favoriteService.IsFavoriteAsync(meal.IdMeal);
                if (isFavorite)
                {
                    await _favoriteService.RemoveFavoriteAsync(meal.IdMeal);
                    await DisplayAlert("Removed", "Recipe removed from favorites.", "OK");
                }
                else
                {
                    await _favoriteService.AddFavoriteAsync(meal.IdMeal);
                    await DisplayAlert("Added", "Recipe added to favorites.", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Error", "Failed to update favorites", "OK");
            }
        }

        private async void OnShareRecipe(Meal meal)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = meal.Name,
                    Text = $"Check out this amazing recipe for {meal.Name}!"
                });
            }
            catch
            {
                await DisplayAlert("Error", "Failed to share recipe", "OK");
            }
        }

        private async void OnViewRecipe(Meal meal)
        {
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "idMeal", meal.IdMeal }
                };
                await Shell.Current.GoToAsync("RecipeDetailsPage", navigationParameter);
            }
            catch
            {
                await DisplayAlert("Error", "Failed to open recipe details", "OK");
            }
        }

        private async void OnSaveRecipe(Meal meal)
        {
            try
            {
                // TODO: Implement save recipe
                await DisplayAlert("Success", "Recipe saved successfully", "OK");
            }
            catch
            {
                await DisplayAlert("Error", "Failed to save recipe", "OK");
            }
        }

        private async void OnNavigateToExploreWithTag(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return;

            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "SelectedTag", tag }
                };
                await Shell.Current.GoToAsync("//ExplorePage", navigationParameter);
            }
            catch
            {
                await DisplayAlert("Error", "Failed to navigate to Explore page", "OK");
            }
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            NavbarContainer.IsVisible = e.ScrollY < HideThreshold;
        }

        private async void OnSearchTapped(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync("//ExplorePage");
            }
            catch
            {
                // Handle navigation error if needed
            }
        }

        private async void OnRecipeOfTheDayTapped(object sender, EventArgs e)
        {
            if (RecipeOfTheDay != null)
            {
                try
                {
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "idMeal", RecipeOfTheDay.IdMeal }
                    };
                    await Shell.Current.GoToAsync("RecipeDetailsPage", navigationParameter);
                }
                catch
                {
                    await DisplayAlert("Error", "Failed to open recipe details", "OK");
                }
            }
        }
    }
}

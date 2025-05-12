using System.Windows.Input;
using RecipeApp.Models;
using Microsoft.Maui.Controls;
using RecipeApp.Services;

namespace RecipeApp.Pages
{
    [QueryProperty(nameof(IdMeal), "idMeal")]
    public partial class RecipeDetailsPage : ContentPage
    {
        private Meal _meal;
        public Meal Meal
        {
            get => _meal;
            set
            {
                _meal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasYoutubeLink));
                if (_meal != null)
                {
                    ParseIngredients();
                }
            }
        }

        private string _idMeal;
        public string IdMeal
        {
            get => _idMeal;
            set
            {
                _idMeal = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(_idMeal))
                {
                    // Fetch the meal from the API
                    _ = LoadMealById(_idMeal);
                }
            }
        }

        private List<string> _ingredients;
        public List<string> Ingredients
        {
            get => _ingredients;
            set
            {
                _ingredients = value;
                OnPropertyChanged();
            }
        }

        public bool HasYoutubeLink => !string.IsNullOrEmpty(Meal?.YoutubeUrl);

        public ICommand GoBackCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand ShareRecipeCommand { get; }
        public ICommand OpenYoutubeCommand { get; }

        private readonly IFavoriteService _favoriteService = App.Services.GetService<IFavoriteService>();

        public RecipeDetailsPage()
        {
            GoBackCommand = new Command(async () => await GoBack());
            ToggleFavoriteCommand = new Command(async () => await ToggleFavorite());
            ShareRecipeCommand = new Command(async () => await ShareRecipe());
            OpenYoutubeCommand = new Command(async () => await OpenYoutube());
            InitializeComponent();
            BindingContext = this;
        }

        private async Task LoadMealById(string idMeal)
        {
            try
            {
                // Use your MealService to fetch the meal by ID
                var mealService = new RecipeApp.Services.MealService(new HttpClient(), null);
                var meal = await mealService.GetMealByIdAsync(idMeal);
                if (meal != null)
                {
                    Meal = meal;
                }
                else
                {
                    await DisplayAlert("Error", "Recipe not found.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch
            {
                await DisplayAlert("Error", "Failed to load meal details", "OK");
            }
        }

        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task ToggleFavorite()
        {
            if (Meal == null) return;
            try
            {
                bool isFavorite = await _favoriteService.IsFavoriteAsync(Meal.IdMeal);
                if (isFavorite)
                {
                    await _favoriteService.RemoveFavoriteAsync(Meal.IdMeal);
                    await DisplayAlert("Removed", "Recipe removed from favorites.", "OK");
                }
                else
                {
                    await _favoriteService.AddFavoriteAsync(Meal.IdMeal);
                    await DisplayAlert("Added", "Recipe added to favorites.", "OK");
                }
            }
            catch
            {
                await DisplayAlert("Error", "Failed to update favorites", "OK");
            }
        }

        private async Task ShareRecipe()
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = Meal.Name,
                    Text = $"Check out this amazing recipe for {Meal.Name}!\n\n" +
                          $"Category: {Meal.Category}\n" +
                          $"Area: {Meal.Area}\n\n" +
                          $"Instructions: {Meal.Instructions}\n\n" +
                          $"Watch on YouTube: {Meal.YoutubeUrl}"
                });
            }
            catch
            {
                await DisplayAlert("Error", "Failed to share recipe", "OK");
            }
        }

        private async Task OpenYoutube()
        {
            if (string.IsNullOrEmpty(Meal?.YoutubeUrl))
                return;

            try
            {
                await Launcher.OpenAsync(new Uri(Meal.YoutubeUrl));
            }
            catch
            {
                await DisplayAlert("Error", "Failed to open YouTube link", "OK");
            }
        }

        private void ParseIngredients()
        {
            var ingredients = new List<string>();
            
            // Get all ingredient properties using reflection
            var ingredientProperties = typeof(Meal).GetProperties()
                .Where(p => p.Name.StartsWith("Ingredient") && p.PropertyType == typeof(string));

            foreach (var prop in ingredientProperties)
            {
                var ingredient = (string)prop.GetValue(Meal);
                if (!string.IsNullOrWhiteSpace(ingredient))
                {
                    // Get the corresponding measure property
                    var measurePropName = prop.Name.Replace("Ingredient", "Measure");
                    var measureProp = typeof(Meal).GetProperty(measurePropName);
                    var measure = measureProp != null ? (string)measureProp.GetValue(Meal) : null;

                    // Combine measure and ingredient
                    var ingredientText = !string.IsNullOrWhiteSpace(measure) 
                        ? $"{measure} {ingredient}"
                        : ingredient;

                    ingredients.Add(ingredientText);
                }
            }

            Ingredients = ingredients;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!string.IsNullOrEmpty(IdMeal))
            {
                // Always fetch the meal when the page appears
                _ = LoadMealById(IdMeal);
            }
        }
    }
} 
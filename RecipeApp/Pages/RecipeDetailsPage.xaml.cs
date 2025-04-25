using System.Windows.Input;

namespace RecipeApp.Pages
{
    [QueryProperty(nameof(Recipe), "Recipe")]
    public partial class RecipeDetailsPage : ContentPage
    {
        private Recipe _recipe;
        public Recipe Recipe
        {
            get => _recipe;
            set
            {
                _recipe = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        public ICommand ShareRecipeCommand { get; }
        public ICommand StartCookingCommand { get; }

        public RecipeDetailsPage()
        {
            InitializeComponent();
            BindingContext = this;

            // Initialize commands
            GoBackCommand = new Command(async () => await GoBack());
            ToggleFavoriteCommand = new Command(async () => await ToggleFavorite());
            ShareRecipeCommand = new Command(async () => await ShareRecipe());
            StartCookingCommand = new Command(async () => await StartCooking());
        }

        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task ToggleFavorite()
        {
            try
            {
                Recipe.IsFavorite = !Recipe.IsFavorite;
                string message = Recipe.IsFavorite ? "Added to favorites" : "Removed from favorites";
                await DisplayAlert("Success", message, "OK");
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
                    Title = Recipe.Name,
                    Text = $"Check out this amazing recipe for {Recipe.Name}!"
                });
            }
            catch
            {
                await DisplayAlert("Error", "Failed to share recipe", "OK");
            }
        }

        private async Task StartCooking()
        {
            try
            {
                // TODO: Implement start cooking functionality
                // This could navigate to a cooking mode page or show cooking instructions
                await DisplayAlert("Coming Soon", "Cooking mode will be available soon!", "OK");
            }
            catch
            {
                await DisplayAlert("Error", "Failed to start cooking mode", "OK");
            }
        }
    }
} 
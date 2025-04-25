using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls.Shapes;

namespace RecipeApp.Pages
{
    public partial class FavoritesPage : ContentPage
    {
        private ObservableCollection<string> _tags = new();
        private ObservableCollection<string> _selectedTags = new();
        private ObservableCollection<FavoriteRecipe> _favorites = new();
        private ObservableCollection<FavoriteRecipe> _filteredFavorites = new();
        private string _searchQuery = string.Empty;
        private string _searchResultsText = string.Empty;

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
            }
        }

        public ObservableCollection<FavoriteRecipe> Favorites
        {
            get => _favorites;
            set
            {
                _favorites = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FavoriteRecipe> FilteredFavorites
        {
            get => _filteredFavorites;
            set
            {
                _filteredFavorites = value;
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
                FilterFavorites();
            }
        }

        public string SearchResultsText
        {
            get => _searchResultsText;
            set
            {
                _searchResultsText = value;
                OnPropertyChanged();
            }
        }

        public ICommand RemoveFromFavoritesCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ViewRecipeCommand { get; }

        public FavoritesPage()
        {
            InitializeComponent();
            BindingContext = this;

            // Initialize commands
            RemoveFromFavoritesCommand = new Command<FavoriteRecipe>(RemoveFromFavorites);
            SearchCommand = new Command(async () => await Shell.Current.GoToAsync("//ExplorePage"));
            ViewRecipeCommand = new Command<FavoriteRecipe>(OnViewRecipe);

            // Initialize tags
            Tags = new ObservableCollection<string>
            {
                "Breakfast", "Lunch", "Dinner", "Dessert", "Snacks",
                "Quick & Easy", "Healthy", "Vegetarian", "Comfort Food"
            };

            // Initialize with sample data (replace with actual data later)
            LoadSampleData();

            // Initialize filtered collections
            FilteredFavorites = new ObservableCollection<FavoriteRecipe>(Favorites);
            SelectedTags = new ObservableCollection<string>();

            UpdateSearchResultsText();
        }

        private void LoadSampleData()
        {
            Favorites = new ObservableCollection<FavoriteRecipe>
            {
                new FavoriteRecipe
                {
                    Name = "Chicken Adobo",
                    Description = "Classic Filipino dish with chicken marinated in vinegar, soy sauce, and garlic",
                    Image = "recipe1.png",
                    CookTime = "45 mins",
                    Category = "Main Course",
                    Tags = new List<string> { "Dinner", "Comfort Food" }
                },
                new FavoriteRecipe
                {
                    Name = "Pancit Bihon",
                    Description = "Filipino noodle dish with vegetables and meat",
                    Image = "recipe2.png",
                    CookTime = "30 mins",
                    Category = "Main Course",
                    Tags = new List<string> { "Quick & Easy", "Lunch" }
                }
            };
        }

        private void SearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchQuery = e.NewTextValue;
        }

        private void FilterFavorites()
        {
            var filtered = Favorites.Where(recipe =>
            {
                bool matchesSearch = string.IsNullOrWhiteSpace(SearchQuery) ||
                                   recipe.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                   recipe.Description.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);

                bool matchesTags = !SelectedTags.Any() ||
                                 recipe.Tags.Any(tag => SelectedTags.Contains(tag));

                return matchesSearch && matchesTags;
            });

            FilteredFavorites = new ObservableCollection<FavoriteRecipe>(filtered);
            UpdateSearchResultsText();
        }

        private void AddTag(string tag)
        {
            if (!SelectedTags.Contains(tag))
            {
                SelectedTags.Add(tag);
                CreateTagButton(tag);
                FilterFavorites();
            }
        }

        private void RemoveTag(string tag)
        {
            if (SelectedTags.Remove(tag))
            {
                var tagButton = SelectedTagsContainer.Children.FirstOrDefault(x =>
                    x is Border border &&
                    border.Content is Grid grid &&
                    grid.Children.OfType<Label>().FirstOrDefault()?.Text == tag);

                if (tagButton != null)
                {
                    SelectedTagsContainer.Children.Remove(tagButton);
                }

                FilterFavorites();
            }
        }

        private void RemoveFromFavorites(FavoriteRecipe recipe)
        {
            if (Favorites.Remove(recipe))
            {
                FilteredFavorites.Remove(recipe);
                UpdateSearchResultsText();
            }
        }

        private async void OnViewRecipe(FavoriteRecipe recipe)
        {
            if (recipe == null) return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Recipe", recipe }
            };
            await Shell.Current.GoToAsync("RecipeDetailsPage", navigationParameter);
        }

        private void UpdateSearchResultsText()
        {
            string text = $"{FilteredFavorites.Count} favorite";
            text += FilteredFavorites.Count == 1 ? " recipe" : " recipes";

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                text += $" matching '{SearchQuery}'";
            }

            if (SelectedTags.Any())
            {
                text += $" with tags: {string.Join(", ", SelectedTags)}";
            }

            SearchResultsText = text;
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
                VerticalOptions = LayoutOptions.Center
            };

            var removeButton = new ImageButton
            {
                Source = "close.svg",
                HeightRequest = 16,
                WidthRequest = 16,
                BackgroundColor = Colors.Transparent,
                Command = new Command(() => RemoveTag(tag)),
                Margin = new Thickness(3, 0, 0, 0)
            };

            Grid.SetColumn(removeButton, 1);
            grid.Children.Add(label);
            grid.Children.Add(removeButton);

            var button = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                BackgroundColor = Color.FromArgb("#5b8224"),
                Padding = new Thickness(8, 4),
                Content = grid
            };

            SelectedTagsContainer.Children.Add(button);
        }
    }

    public class FavoriteRecipe
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }
} 
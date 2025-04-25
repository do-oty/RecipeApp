using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RecipeApp.Pages
{
    public partial class HomePage : ContentPage
    {
        private const int ItemsPerPage = 9;
        private const double HideThreshold = 10.0;

        public ObservableCollection<Recipe> CarouselFeatured { get; set; }
        public ObservableCollection<CarouselCategoryItem> CarouselCategory { get; set; }
        public ObservableCollection<List<CarouselCategoryCollection>> PaginatedCategories { get; set; }
        public ObservableCollection<TagItem> Tags { get; set; }
        public ObservableCollection<Recipe> TrendingRecipes { get; set; }
        public ObservableCollection<ActivityItem> RecentActivity { get; set; }
        public RecipeOfTheDayItem RecipeOfTheDay { get; set; }
        private ObservableCollection<Recipe> _quickAccessRecipes;
        public ObservableCollection<Recipe> QuickAccessRecipes
        {
            get => _quickAccessRecipes;
            set
            {
                _quickAccessRecipes = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<RecipeCategory> _popularCategories;
        public ObservableCollection<RecipeCategory> PopularCategories
        {
            get => _popularCategories;
            set
            {
                _popularCategories = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddToCollectionCommand { get; }
        public ICommand AddToFavoritesCommand { get; }
        public ICommand ShareRecipeCommand { get; }
        public ICommand ViewRecipeCommand { get; }
        public ICommand SaveRecipeCommand { get; }
        public ICommand NavigateToExploreWithTagCommand { get; }

        public HomePage()
        {
            InitializeComponent();

            // Initialize commands
            AddToCollectionCommand = new Command<Recipe>(OnAddToCollection);
            AddToFavoritesCommand = new Command<Recipe>(OnAddToFavorites);
            ShareRecipeCommand = new Command<Recipe>(OnShareRecipe);
            ViewRecipeCommand = new Command<Recipe>(OnViewRecipe);
            SaveRecipeCommand = new Command<Recipe>(OnSaveRecipe);
            NavigateToExploreWithTagCommand = new Command<string>(OnNavigateToExploreWithTag);

            // Initialize collections with sample data
            InitializeCollections();

            carouselFeatured.CurrentItem = CarouselFeatured.FirstOrDefault();
            indicatorView.Position = 0;

            BindingContext = this;
        }

        private void InitializeCollections()
        {
            // Initialize Featured Recipes
            CarouselFeatured = new ObservableCollection<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Name = "Fried Chicken",
                    Image = "carousel1.svg",
                    Caption = "Crispy and juicy fried chicken",
                    CookTime = "45 mins",
                    Difficulty = "Medium",
                    Servings = "4",
                    Category = "Main Course",
                    IsFeatured = true,
                    Rating = 4.8,
                    TotalReviews = 128,
                    TrendingScore = 95,
                    PublishedDate = DateTime.Now.AddDays(-2),
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 2,
                    Name = "Pork Sisig",
                    Image = "carousel2.svg",
                    Caption = "Filipino sizzling pork dish",
                    CookTime = "30 mins",
                    Difficulty = "Easy",
                    Servings = "4",
                    Category = "Appetizer",
                    IsFeatured = true,
                    Rating = 4.9,
                    TotalReviews = 156,
                    TrendingScore = 92,
                    PublishedDate = DateTime.Now.AddDays(-1),
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 3,
                    Name = "Pancit Bihon",
                    Image = "carousel3.svg",
                    Caption = "Classic Filipino noodle dish",
                    CookTime = "25 mins",
                    Difficulty = "Easy",
                    Servings = "6",
                    Category = "Main Course",
                    IsFeatured = true,
                    Rating = 4.7,
                    TotalReviews = 142,
                    TrendingScore = 88,
                    PublishedDate = DateTime.Now,
                    IsFavorite = false
                }
            };

            // Initialize Trending Recipes
            TrendingRecipes = new ObservableCollection<Recipe>
            {
                new Recipe
                {
                    Id = 4,
                    Name = "Chocolate Cake",
                    Image = "carousel1.svg",
                    Caption = "Rich and moist chocolate cake",
                    CookTime = "60 mins",
                    Difficulty = "Medium",
                    Servings = "8",
                    Category = "Dessert",
                    IsFeatured = false,
                    Rating = 4.9,
                    TotalReviews = 203,
                    TrendingScore = 98,
                    PublishedDate = DateTime.Now.AddDays(-3),
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 5,
                    Name = "Greek Salad",
                    Image = "carousel2.svg",
                    Caption = "Fresh and healthy Greek salad",
                    CookTime = "15 mins",
                    Difficulty = "Easy",
                    Servings = "4",
                    Category = "Salad",
                    IsFeatured = false,
                    Rating = 4.6,
                    TotalReviews = 167,
                    TrendingScore = 85,
                    PublishedDate = DateTime.Now.AddDays(-4),
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 6,
                    Name = "Beef Steak",
                    Image = "carousel3.svg",
                    Caption = "Perfectly grilled beef steak",
                    CookTime = "25 mins",
                    Difficulty = "Medium",
                    Servings = "2",
                    Category = "Main Course",
                    IsFeatured = false,
                    Rating = 4.8,
                    TotalReviews = 189,
                    TrendingScore = 92,
                    PublishedDate = DateTime.Now.AddDays(-2),
                    IsFavorite = false
                }
            };

            // Initialize Tags
            Tags = new ObservableCollection<TagItem>
            {
                new TagItem { Name = "Italian", ImageUrl = "tag1.jpg" },
                new TagItem { Name = "Quick & Easy", ImageUrl = "tag2.jpg" },
                new TagItem { Name = "Vegetarian", ImageUrl = "tag3.jpg" },
                new TagItem { Name = "Desserts", ImageUrl = "tag4.jpg" },
                new TagItem { Name = "Asian", ImageUrl = "tag5.jpg" }
            };

            // Initialize Recent Activity
            RecentActivity = new ObservableCollection<ActivityItem>
            {
                new ActivityItem
                {
                    Message = "Sarah liked your Fried Chicken recipe",
                    TimeAgo = "2 hours ago",
                    Image = "user1.jpg",
                    ActionIcon = "heart.png",
                    Type = ActivityType.Like
                },
                new ActivityItem
                {
                    Message = "Mike saved your Pork Sisig recipe",
                    TimeAgo = "3 hours ago",
                    Image = "user2.jpg",
                    ActionIcon = "bookmark.png",
                    Type = ActivityType.Save
                },
                new ActivityItem
                {
                    Message = "Lisa commented on your Pancit Bihon recipe",
                    TimeAgo = "5 hours ago",
                    Image = "user3.jpg",
                    ActionIcon = "comment.png",
                    Type = ActivityType.Comment
                }
            };

            // Set Recipe of the Day
            RecipeOfTheDay = new RecipeOfTheDayItem
            {
                Image = CarouselFeatured[0].Image,
                Caption = CarouselFeatured[0].Caption,
                CookTime = CarouselFeatured[0].CookTime,
                Difficulty = CarouselFeatured[0].Difficulty,
                Servings = CarouselFeatured[0].Servings
            };

            CarouselCategory = new ObservableCollection<CarouselCategoryItem>
            {
                new CarouselCategoryItem { Image = "local_image1.png", Caption = "Drink" },
                new CarouselCategoryItem { Image = "local_image2.png", Caption = "Appetizer" },
                new CarouselCategoryItem { Image = "local_image3.png", Caption = "Main Course" },
                new CarouselCategoryItem { Image = "local_image4.png", Caption = "Dessert" },
                new CarouselCategoryItem { Image = "local_image5.png", Caption = "Salad" },
                new CarouselCategoryItem { Image = "local_image6.png", Caption = "Soup" },
                new CarouselCategoryItem { Image = "local_image7.png", Caption = "Snack" },
                new CarouselCategoryItem { Image = "local_image8.png", Caption = "Breakfast" },
                new CarouselCategoryItem { Image = "local_image9.png", Caption = "Lunch" },
                new CarouselCategoryItem { Image = "local_image10.png", Caption = "Dinner" }
            };

            PaginatedCategories = new ObservableCollection<List<CarouselCategoryCollection>>();
            LoadCategories();

            // Quick Access Recipes
            QuickAccessRecipes = new ObservableCollection<Recipe>
            {
                new Recipe
                {
                    Name = "Classic Spaghetti Carbonara",
                    Caption = "A traditional Italian pasta dish with eggs, cheese, pancetta, and black pepper",
                    Image = "carousel1.svg",
                    CookTime = "30 mins",
                    Difficulty = "Medium",
                    Servings = "4"
                },
                new Recipe
                {
                    Name = "Grilled Salmon with Vegetables",
                    Caption = "Healthy grilled salmon served with roasted seasonal vegetables",
                    Image = "carousel2.svg",
                    CookTime = "25 mins",
                    Difficulty = "Easy",
                    Servings = "2"
                },
                new Recipe
                {
                    Name = "Chocolate Chip Cookies",
                    Caption = "Soft and chewy cookies with melted chocolate chips",
                    Image = "carousel3.svg",
                    CookTime = "35 mins",
                    Difficulty = "Easy",
                    Servings = "24"
                }
            };

            // Popular Categories
            PopularCategories = new ObservableCollection<RecipeCategory>
            {
                new RecipeCategory
                {
                    Id = "1",
                    Name = "Breakfast",
                    Icon = "breakfast.svg",
                    RecipeCount = 45,
                    Description = "Start your day with delicious and nutritious breakfast recipes"
                },
                new RecipeCategory
                {
                    Id = "2",
                    Name = "Quick & Easy",
                    Icon = "quick.svg",
                    RecipeCount = 78,
                    Description = "30-minute meals perfect for busy weekdays"
                },
                new RecipeCategory
                {
                    Id = "3",
                    Name = "Healthy",
                    Icon = "healthy.svg",
                    RecipeCount = 62,
                    Description = "Nutritious recipes that don't compromise on taste"
                },
                new RecipeCategory
                {
                    Id = "4",
                    Name = "Desserts",
                    Icon = "dessert.svg",
                    RecipeCount = 53,
                    Description = "Sweet treats and baked goods for any occasion"
                },
                new RecipeCategory
                {
                    Id = "5",
                    Name = "Vegetarian",
                    Icon = "vegetarian.svg",
                    RecipeCount = 48,
                    Description = "Meat-free dishes that are full of flavor"
                },
                new RecipeCategory
                {
                    Id = "6",
                    Name = "Family Favorites",
                    Icon = "family.svg",
                    RecipeCount = 67,
                    Description = "Crowd-pleasing recipes loved by everyone"
                }
            };
        }

        private void LoadCategories()
        {
            var allCategories = new List<CarouselCategoryCollection>
            {
                new CarouselCategoryCollection { Image = "collection_image1.png", Caption = "Collection 1" },
                new CarouselCategoryCollection { Image = "collection_image2.png", Caption = "Collection 2" },
                new CarouselCategoryCollection { Image = "collection_image3.png", Caption = "Collection 3" },
                new CarouselCategoryCollection { Image = "collection_image4.png", Caption = "Collection 4" },
                new CarouselCategoryCollection { Image = "collection_image5.png", Caption = "Collection 5" },
                new CarouselCategoryCollection { Image = "collection_image6.png", Caption = "Collection 6" },
                new CarouselCategoryCollection { Image = "collection_image7.png", Caption = "Collection 7" },
                new CarouselCategoryCollection { Image = "collection_image8.png", Caption = "Collection 8" },
                new CarouselCategoryCollection { Image = "collection_image9.png", Caption = "Collection 9" },
                new CarouselCategoryCollection { Image = "collection_image10.png", Caption = "Collection 10" },
                new CarouselCategoryCollection { Image = "collection_image11.png", Caption = "Collection 11" },
                new CarouselCategoryCollection { Image = "collection_image12.png", Caption = "Collection 12" },
                new CarouselCategoryCollection { Image = "collection_image13.png", Caption = "Collection 13" },
                new CarouselCategoryCollection { Image = "collection_image14.png", Caption = "Collection 14" },
                new CarouselCategoryCollection { Image = "collection_image15.png", Caption = "Collection 15" }
            };

            for (int i = 0; i < allCategories.Count; i += ItemsPerPage)
            {
                var batch = allCategories.Skip(i).Take(ItemsPerPage).ToList();
                PaginatedCategories.Add(batch);
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

        private void OnAddToCollection(Recipe recipe)
        {
            // Implement add to collection logic
            // You can show a collection picker or navigate to collections page
        }

        private async void OnAddToFavorites(Recipe recipe)
        {
            try
            {
                recipe.IsFavorite = !recipe.IsFavorite;
                string message = recipe.IsFavorite ? "Added to favorites" : "Removed from favorites";
                await DisplayAlert("Success", message, "OK");
            }
            catch
            {
                await DisplayAlert("Error", "Failed to update favorites", "OK");
            }
        }

        private async void OnShareRecipe(Recipe recipe)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = recipe.Name,
                    Text = $"Check out this amazing recipe for {recipe.Name}!"
                });
            }
            catch
            {
                await DisplayAlert("Error", "Failed to share recipe", "OK");
            }
        }

        private async void OnViewRecipe(Recipe recipe)
        {
            try
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Recipe", recipe }
                };
                await Shell.Current.GoToAsync($"//RecipeDetailsPage", navigationParameter);
            }
            catch
            {
                await DisplayAlert("Error", "Failed to open recipe details", "OK");
            }
        }

        private async void OnSaveRecipe(Recipe recipe)
        {
            try
            {
                // TODO: Implement save recipe functionality
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
                await Shell.Current.GoToAsync($"//ExplorePage", navigationParameter);
            }
            catch
            {
                await DisplayAlert("Error", "Failed to navigate to Explore page", "OK");
            }
        }

        // Future method to load tags from API
        private async Task LoadTagsFromApi()
        {
            try
            {
                // Example of how the API integration would work
                // var apiTags = await _tagService.GetTags();
                // Tags = new ObservableCollection<TagItem>(apiTags);
            }
            catch (Exception ex)
            {
                // Handle API errors
                await DisplayAlert("Error", "Failed to load tags", "OK");
            }
        }
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Servings { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public int TrendingScore { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public bool IsFavorite { get; set; }
    }

    public class CarouselCategoryItem
    {
        public string Image { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }

    public class CarouselCategoryCollection
    {
        public string Image { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }

    public class RecipeOfTheDayItem
    {
        public string Image { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string CookTime { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public string Servings { get; set; } = string.Empty;
    }

    public class TagItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int RecipeCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ActivityItem
    {
        public string Message { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ActionIcon { get; set; } = string.Empty;
        public ActivityType Type { get; set; }
    }

    public enum ActivityType
    {
        Saved,
        Cooked,
        Shared,
        Rated,
        Commented,
        Like,
        Save,
        Comment
    }

    public class RecipeCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public int RecipeCount { get; set; }
        public string Description { get; set; }
    }
}

using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RecipeApp.Pages
{
    [QueryProperty(nameof(SelectedTag), "SelectedTag")]
    public partial class ExplorePage : ContentPage
    {
        private ObservableCollection<string> _tags = new();
        private ObservableCollection<string> _selectedTags = new();
        private ObservableCollection<string> _filteredTags = new();
        private string _searchQuery = string.Empty;
        private string _searchResultsText = string.Empty;
        private string _selectedTag;

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

        public ICommand GoBackCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddTagCommand { get; }
        public ICommand RemoveTagCommand { get; }

        public ExplorePage()
        {
            InitializeComponent();
            BindingContext = this;

            // Initialize commands
            GoBackCommand = new Command(async () => await GoBack());
            SearchCommand = new Command(async () => await Search());
            AddTagCommand = new Command<string>(AddTag);
            RemoveTagCommand = new Command<string>(RemoveTag);

            // Initialize collections
            Tags = new ObservableCollection<string>
            {
                "Breakfast", "Lunch", "Dinner", "Dessert", "Snacks",
                "Vegetarian", "Vegan", "Gluten-Free", "Quick & Easy",
                "Healthy", "Comfort Food", "Italian", "Mexican", "Asian",
                "Mediterranean", "American", "Indian", "Thai", "Japanese"
            };

            FilteredTags = new ObservableCollection<string>(Tags);
            SelectedTags = new ObservableCollection<string>();

            // Initialize search results text
            UpdateSearchResultsText();
        }

        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        private Task Search()
        {
            // Simulate search with selected tags and query
            string searchDescription = "Searching for recipes";

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                searchDescription += $" containing '{SearchQuery}'";
            }

            if (SelectedTags.Count > 0)
            {
                searchDescription += $" with tags: {string.Join(", ", SelectedTags)}";
            }

            SearchResultsText = searchDescription;
            return Task.CompletedTask;
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
                UpdateSearchResultsText();
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

            UpdateSearchResultsText();
        }

        private void UpdateSearchResultsText()
        {
            string searchDescription = "Searching for recipes..";

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                searchDescription += $" containing '{SearchQuery}'";
            }

            if (SelectedTags.Count > 0)
            {
                searchDescription += $" with tags: {string.Join(", ", SelectedTags)}";
            }

            SearchResultsText = searchDescription;
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
                Command = RemoveTagCommand,
                CommandParameter = tag,
                Margin = new Thickness(3, 0, 0, 0)
            };

            Grid.SetColumn(removeButton, 1);
            grid.Children.Add(label);
            grid.Children.Add(removeButton);

            var button = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                Stroke = Color.FromArgb("#5b8224"),
                StrokeThickness = 0,
                BackgroundColor = Color.FromArgb("#5b8224"),
                Padding = new Thickness(8, 4),
                Content = grid
            };

            SelectedTagsContainer.Children.Add(button);
        }

        private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        {
            // This method is required by the XAML but we don't need any special scroll handling here
        }
    }
}
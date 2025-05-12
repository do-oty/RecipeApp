using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RecipeApp.Models;
using RecipeApp.Services;
using RecipeApp.Views;
using System.Linq;
using System.Collections.Generic;
using System;

namespace RecipeApp.ViewModels
{
    public partial class MealViewModel : BaseViewModel
    {
        private readonly IMealService _mealService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Meal> _meals;

        [ObservableProperty]
        private string searchTerm;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private Meal selectedMeal;

        [ObservableProperty]
        private ObservableCollection<Category> _categories;

        [ObservableProperty]
        private ObservableCollection<SelectableArea> _areas;

        [ObservableProperty]
        private ObservableCollection<Category> _selectedCategories;

        [ObservableProperty]
        private ObservableCollection<SelectableArea> _selectedAreas;

        [ObservableProperty]
        private bool _isFilterModalVisible;

        private string _searchResultsText;
        public string SearchResultsText
        {
            get => _searchResultsText;
            set => SetProperty(ref _searchResultsText, value);
        }

        public MealViewModel(IMealService mealService, INavigationService navigationService)
        {
            _mealService = mealService;
            _navigationService = navigationService;
            Meals = new ObservableCollection<Meal>();
            Meals.CollectionChanged += (s, e) => UpdateSearchResultsText();
            Categories = new ObservableCollection<Category>();
            Areas = new ObservableCollection<SelectableArea>();
            SelectedCategories = new ObservableCollection<Category>();
            SelectedAreas = new ObservableCollection<SelectableArea>();
            Task.Run(async () => await LoadDataAsync());
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Load category names for filtering
                var categoryNames = await _mealService.GetCategoryNamesAsync();
                Categories.Clear();
                foreach (var name in categoryNames)
                {
                    var category = new Category { Name = name };
                    category.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(Category.IsSelected))
                        {
                            if (category.IsSelected && !SelectedCategories.Contains(category))
                                SelectedCategories.Add(category);
                            else if (!category.IsSelected && SelectedCategories.Contains(category))
                                SelectedCategories.Remove(category);
                        }
                    };
                    Categories.Add(category);
                }

                // Load areas
                var areas = await _mealService.GetAreasAsync();
                Areas.Clear();
                foreach (var areaName in areas)
                {
                    var area = new SelectableArea(areaName);
                    area.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(SelectableArea.IsSelected))
                        {
                            if (area.IsSelected && !SelectedAreas.Contains(area))
                                SelectedAreas.Add(area);
                            else if (!area.IsSelected && SelectedAreas.Contains(area))
                                SelectedAreas.Remove(area);
                        }
                    };
                    Areas.Add(area);
                }

                // Load initial meals
                await LoadMealsAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadMealsAsync()
        {
            try
            {
                var meals = await _mealService.GetMealsAsync();
                Meals.Clear();
                foreach (var meal in meals)
                {
                    Meals.Add(meal);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task SearchMeals()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                // If no search term, show filtered results
                if (SelectedCategories.Any() || SelectedAreas.Any())
                {
                    await ApplyFiltersAsync();
                }
                else
                {
                    await LoadMealsAsync();
                }
                return;
            }

            IsBusy = true;
            try
            {
                // First get search results
                var searchResults = await _mealService.SearchMealsAsync(SearchTerm);
                
                // Then apply any active filters
                if (SelectedCategories.Any() || SelectedAreas.Any())
                {
                    var selectedCategoryNames = SelectedCategories.Select(c => c.Name).ToList();
                    var selectedAreaNames = SelectedAreas.Select(a => a.Area).ToList();
                    
                    // Filter the search results based on selected categories and areas
                    searchResults = searchResults.Where(m =>
                    {
                        bool matchesCategory = !selectedCategoryNames.Any() || 
                            selectedCategoryNames.Contains(m.Category, StringComparer.OrdinalIgnoreCase);
                        bool matchesArea = !selectedAreaNames.Any() || 
                            selectedAreaNames.Contains(m.Area, StringComparer.OrdinalIgnoreCase);
                        return matchesCategory && matchesArea;
                    }).ToList();
                }

                Meals.Clear();
                foreach (var meal in searchResults)
                {
                    Meals.Add(meal);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to search meals: " + ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ShowFilterModalAsync()
        {
            IsFilterModalVisible = true;
            await _navigationService.PushModalAsync(new FilterModal(this));
        }

        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var selectedCategoryNames = SelectedCategories.Select(c => c.Name).ToList();
                var selectedAreaNames = SelectedAreas.Select(a => a.Area).ToList();

                // Only use one filter at a time (category takes priority)
                var filteredMeals = await _mealService.FilterMealsAsync(selectedCategoryNames, selectedAreaNames);

                Meals.Clear();
                foreach (var meal in filteredMeals)
                {
                    Meals.Add(meal);
                }

                // Close the modal
                IsFilterModalVisible = false;
                await _navigationService.PopModalAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ClearFiltersAsync()
        {
            SelectedCategories.Clear();
            SelectedAreas.Clear();
            await LoadMealsAsync();
        }

        [RelayCommand]
        private void ToggleCategorySelection(Category category)
        {
            if (SelectedCategories.Contains(category))
            {
                SelectedCategories.Remove(category);
                category.IsSelected = false;
            }
            else
            {
                SelectedCategories.Add(category);
                category.IsSelected = true;
            }
        }

        [RelayCommand]
        private void ToggleAreaSelection(SelectableArea area)
        {
            if (SelectedAreas.Contains(area))
            {
                SelectedAreas.Remove(area);
                area.IsSelected = false;
            }
            else
            {
                SelectedAreas.Add(area);
                area.IsSelected = true;
            }
        }

        partial void OnSelectedMealChanged(Meal value)
        {
            if (value != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Meal", value }
                };
                Shell.Current.GoToAsync("RecipeDetailsPage", navigationParameter);
            }
        }

        private void UpdateSearchResultsText()
        {
            string text = $"{Meals?.Count ?? 0} recipe";
            text += (Meals?.Count ?? 0) == 1 ? "" : "s";
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                text += $" matching '{SearchTerm}'";
            SearchResultsText = text;
        }

        partial void OnMealsChanged(ObservableCollection<Meal> value)
        {
            UpdateSearchResultsText();
        }

        public async Task LoadRandomMealsAsync()
        {
            try
            {
                IsBusy = true;
                var uniqueMeals = new Dictionary<string, Meal>();
                int attempts = 0;
                int targetCount = 25;
                int maxAttempts = 100;
                var random = new Random();
                while (uniqueMeals.Count < targetCount && attempts < maxAttempts)
                {
                    var randomMeals = await _mealService.GetRandomMealsAsync(1);
                    foreach (var meal in randomMeals)
                    {
                        if (!uniqueMeals.ContainsKey(meal.IdMeal))
                        {
                            uniqueMeals[meal.IdMeal] = meal;
                        }
                    }
                    attempts++;
                }
                // Shuffle the results
                var shuffled = uniqueMeals.Values.OrderBy(x => random.Next()).ToList();
                Meals.Clear();
                foreach (var meal in shuffled)
                {
                    Meals.Add(meal);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using RecipeApp.Models;
using Microsoft.Maui.Controls;

namespace RecipeApp.Pages
{
    public partial class FavoritesFilterModal : ContentPage
    {
        private List<Category> _categories;
        private List<SelectableArea> _areas;
        private List<Meal> _allFavorites;
        private readonly Action<List<string>, List<string>> _onApply;
        public FavoritesFilterModal(List<Meal> allFavorites, Action<List<string>, List<string>> onApply)
        {
            InitializeComponent();
            _allFavorites = allFavorites;
            _onApply = onApply;
            // Extract unique categories and areas from favorites
            _categories = allFavorites
                .Select(m => m.Category)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .Select(c => new Category { Name = c, IsSelected = false })
                .ToList();
            _areas = allFavorites
                .Select(m => m.Area)
                .Where(a => !string.IsNullOrEmpty(a))
                .Distinct()
                .Select(a => new SelectableArea(a) { IsSelected = false })
                .ToList();
            CategoriesCollection.ItemsSource = _categories;
            AreasCollection.ItemsSource = _areas;
        }

        private async void OnApplyClicked(object sender, EventArgs e)
        {
            var selectedCategories = _categories.Where(c => c.IsSelected).Select(c => c.Name).ToList();
            var selectedAreas = _areas.Where(a => a.IsSelected).Select(a => a.Area).ToList();
            _onApply?.Invoke(selectedCategories, selectedAreas);
            await Shell.Current.Navigation.PopModalAsync();
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            foreach (var c in _categories) c.IsSelected = false;
            foreach (var a in _areas) a.IsSelected = false;
            CategoriesCollection.ItemsSource = null;
            CategoriesCollection.ItemsSource = _categories;
            AreasCollection.ItemsSource = null;
            AreasCollection.ItemsSource = _areas;
        }
    }
} 
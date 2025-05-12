using Microsoft.Maui.Controls;
using RecipeApp.Models;
using RecipeApp.ViewModels;

namespace RecipeApp.Views;

public partial class ExplorePage : ContentPage
{
    private readonly MealViewModel _viewModel;

    public ExplorePage(MealViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRandomMealsAsync();
    }

    private async void OnMealSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is RecipeApp.Models.Meal selectedMeal)
        {
            // Clear the selection
            ((CollectionView)sender).SelectedItem = null;

            // Navigate to the meal details page
            var navigationParameter = new Dictionary<string, object>
            {
                { "idMeal", selectedMeal.IdMeal }
            };
            await Shell.Current.GoToAsync("RecipeDetailsPage", navigationParameter);
        }
    }
} 
using RecipeApp.Models;
using RecipeApp.ViewModels;

namespace RecipeApp.Views;

public partial class MealsPage : ContentPage
{
    private readonly MealViewModel _viewModel;

    public MealsPage(MealViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private void OnMealSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Meal selectedMeal)
        {
            _viewModel.SelectedMeal = selectedMeal;
            ((CollectionView)sender).SelectedItem = null;
        }
    }
} 
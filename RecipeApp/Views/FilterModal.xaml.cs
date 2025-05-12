using RecipeApp.Models;
using RecipeApp.ViewModels;

namespace RecipeApp.Views;

public partial class FilterModal : ContentPage
{
    private readonly MealViewModel _viewModel;

    public FilterModal(MealViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Ensure the view model is properly initialized
        if (_viewModel != null)
        {
            _viewModel.IsFilterModalVisible = true;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Clean up when the modal is closed
        if (_viewModel != null)
        {
            _viewModel.IsFilterModalVisible = false;
        }
    }

    private void OnCategoryCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is Category category)
        {
            _viewModel.ToggleCategorySelectionCommand.Execute(category);
        }
    }

    private void OnAreaCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is SelectableArea area)
        {
            _viewModel.ToggleAreaSelectionCommand.Execute(area);
        }
    }
} 
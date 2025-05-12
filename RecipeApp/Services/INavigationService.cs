using Microsoft.Maui.Controls;

namespace RecipeApp.Services;

public interface INavigationService
{
    Task PushModalAsync(Page page);
    Task PopModalAsync();
} 
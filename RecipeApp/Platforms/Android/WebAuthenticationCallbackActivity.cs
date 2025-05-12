using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace RecipeApp
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.recipe.recipeapp",
        DataPath = "/oauth2redirect")]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
} 
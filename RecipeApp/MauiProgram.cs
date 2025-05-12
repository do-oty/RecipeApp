using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using RecipeApp.Services;
using RecipeApp.ViewModels;
using RecipeApp.Views;
using CommunityToolkit.Maui;
#if ANDROID
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using Plugin.Firebase;
#endif
using Microsoft.Maui.LifecycleEvents;

namespace RecipeApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Auth Service
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register Firestore Service
        builder.Services.AddSingleton<FirestoreService>();

        // Register Favorite Service
        builder.Services.AddSingleton<IFavoriteService, FavoriteService>();

        // Register HttpClient and MealService
        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<IMealService, MealService>();

        // Register Navigation Service
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // Register ViewModels
        builder.Services.AddTransient<MealViewModel>();

        // Register Pages
        builder.Services.AddTransient<MealsPage>();
        builder.Services.AddTransient<ExplorePage>();
        builder.Services.AddTransient<FilterModal>();

#if ANDROID
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddAndroid(android => android.OnCreate((activity, _) =>
            {
                FirebaseAuthGoogleImplementation.Initialize("729481955530-hvrcg9nra2n171h09s32i3jngug0jhsm.apps.googleusercontent.com");
            }));
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif

        var app = builder.Build();

        return app;
    }
}

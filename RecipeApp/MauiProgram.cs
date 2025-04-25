using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

namespace RecipeApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");

            });

#if ANDROID
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(Entry), typeof(CustomEntryHandler));
        });
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
#if ANDROID
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (handler.MauiContext?.Context is Android.App.Activity activity)
                {
                    activity.Window.SetFlags(Android.Views.WindowManagerFlags.LayoutNoLimits, 
                                             Android.Views.WindowManagerFlags.LayoutNoLimits);
                    activity.Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
                }
            });
#endif
        });

        return app;
    }
}

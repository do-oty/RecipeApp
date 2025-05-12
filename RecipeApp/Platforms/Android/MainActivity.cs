using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Plugin.Firebase.Auth;
using Android.Content;
using RecipeApp.Platforms.Android;
using Microsoft.Maui;
using Firebase;
using Plugin.Firebase;

namespace RecipeApp;

[Activity(Theme = "@style/Maui.MainTheme.Base",
          MainLauncher = true,
          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                                ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                                ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        // Initialize Firebase
        if (FirebaseApp.GetInstance(FirebaseApp.DefaultAppName) == null)
        {
            FirebaseApp.InitializeApp(this);
        }

        // Hide the title bar before adding content
        RequestWindowFeature(WindowFeatures.NoTitle);
        
        base.OnCreate(savedInstanceState);

        if (Window != null)
        {
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            
            // Remove the fullscreen flag to keep status bar visible
            Window.ClearFlags(WindowManagerFlags.Fullscreen);
            
            // Make status bar transparent and allow content behind it
            Window.AddFlags(WindowManagerFlags.LayoutNoLimits);
            Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
            Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#D98236"));
        }
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        base.OnActivityResult(requestCode, resultCode, data);

        if (requestCode == GoogleSignInService.RC_SIGN_IN)
        {
            GoogleSignInService.HandleSignInResult(this, resultCode, data);
        }
    }
}

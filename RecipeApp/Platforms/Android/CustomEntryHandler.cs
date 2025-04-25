using Android.OS;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;

namespace RecipeApp;

public class CustomEntryHandler : EntryHandler
{
    protected override void ConnectHandler(AppCompatEditText nativeView)
    {
        base.ConnectHandler(nativeView);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            nativeView.Background = null;
        }
        else
        {
            nativeView.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}

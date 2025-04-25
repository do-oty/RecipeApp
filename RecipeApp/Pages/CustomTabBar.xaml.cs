using System.Windows.Input;

namespace RecipeApp.Controls
{
    public partial class CustomTabBar : ContentView
    {
        public CustomTabBar()
        {
            InitializeComponent();
            BindingContext = this;
            SetPlatformSpecificPadding();
        }

        public ICommand NavigateCommand => new Command<string>(async (page) =>
        {
            try
            {
                // Get the current location
                var location = Shell.Current.CurrentState.Location.ToString();
                
                // Only navigate if we're not already on the page
                if (!location.EndsWith(page))
                {
                    await Shell.Current.GoToAsync($"//{page}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        });

        private void SetPlatformSpecificPadding()
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                this.Padding = new Thickness(0, 0, 0, 40);
            }
            else if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                this.Padding = new Thickness(0, 0, 0, 40);
            }
        }
    }
}
using Microsoft.Maui.Controls;
using RecipeApp.Services;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeApp
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;
        public static IServiceProvider Services { get; private set; }

        public App(IAuthService authService, IServiceProvider services)
        {
            InitializeComponent();
            _authService = authService;
            Services = services;
            SetInitialPage();
        }

        private void SetInitialPage()
        {
            MainPage = new AppShell();
            // Wait for Shell to be ready before navigating
            if (Shell.Current != null)
            {
                Shell.Current.Navigated += OnShellNavigated;
            }
        }

        private async void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
        {
            if (e.Current.Location.ToString() == "//")
            {
                bool autoLogin = Preferences.Default.Get("AutoLogin", false);
                string route = (autoLogin && _authService.IsUserSignedIn()) ? "//HomePage" : "//MainPage";
                await Shell.Current.GoToAsync(route);
                Shell.Current.Navigated -= OnShellNavigated;
            }
        }
    }
}
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using RecipeApp.Services;

namespace RecipeApp
{
    public partial class AppShell : Shell
    {
        private bool _hasNavigatedOnStart = false;
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            
            // Handle navigation events
            Navigating += AppShell_Navigating;
            Navigated += AppShell_Navigated;
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("MainPage", typeof(Pages.MainPage));
            Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
            Routing.RegisterRoute("SignUpPage", typeof(Pages.SignUpPage));
            Routing.RegisterRoute("HomePage", typeof(Pages.HomePage));
            Routing.RegisterRoute("ExplorePage", typeof(Pages.ExplorePage));
            Routing.RegisterRoute("ProfilePage", typeof(Pages.ProfilePage));
            Routing.RegisterRoute("RecipeDetailsPage", typeof(Pages.RecipeDetailsPage));
        }

        private void AppShell_Navigating(object? sender, ShellNavigatingEventArgs e)
        {
            // Navigation handling without AppTabs reference
            // The CustomTabBar will handle its own visibility
        }

        private async void AppShell_Navigated(object sender, ShellNavigatedEventArgs e)
        {
            if (_hasNavigatedOnStart)
                return;
            _hasNavigatedOnStart = true;

            var authService = new AuthService();
            bool autoLogin = Preferences.Default.Get<bool>("AutoLogin", false);
            if (autoLogin && authService.IsUserSignedIn())
            {
                await GoToAsync("//HomePage");
            }
            else
            {
                await GoToAsync("//MainPage");
            }
        }

        public async Task NavigateToHome()
        {
            await GoToAsync("//HomePage");
        }

        public async Task NavigateToExplore()
        {
            await GoToAsync("//ExplorePage");
        }

        public async Task NavigateToProfile()
        {
            await GoToAsync("//ProfilePage");
        }

        public async Task NavigateToMain()
        {
            await GoToAsync("//MainPage");
        }

        public async Task NavigateToLogin()
        {
            await GoToAsync("//LoginPage");
        }

        public async Task NavigateToSignUp()
        {
            await GoToAsync("//SignUpPage");
        }
    }
}

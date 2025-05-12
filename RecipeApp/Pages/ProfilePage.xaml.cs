using RecipeApp.Services;

namespace RecipeApp.Pages
{
    public partial class ProfilePage : ContentPage
    {
        private readonly IAuthService _authService;
        private const string RememberMeKey = "RememberMe";
        private const string SavedEmailKey = "SavedEmail";
        private const string AutoLoginKey = "AutoLogin";

        public ProfilePage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            LoadUserProfile();
        }

        private void LoadUserProfile()
        {
            if (_authService.IsUserSignedIn())
            {
                string userId = _authService.GetCurrentUserId();
                string email = _authService.GetCurrentUserEmail();
                EmailLabel.Text = email;
                GoogleConnectionLabel.Text = "Not Connected";
                GuestUserNotice.IsVisible = false;
            }
            else
            {
                GuestUserNotice.IsVisible = true;
                EmailLabel.Text = "Guest User";
                GoogleConnectionLabel.Text = "Not Available";
            }
        }

        private async void OnSignOutClicked(object sender, EventArgs e)
        {
            var success = await _authService.SignOutAsync();
            if (success)
            {
                // Clear Remember Me settings
                Preferences.Default.Remove(RememberMeKey);
                Preferences.Default.Remove(SavedEmailKey);
                Preferences.Default.Remove(AutoLoginKey);
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Error", "Failed to sign out", "OK");
            }
        }

        private async void OnConnectGoogleClicked(object sender, EventArgs e)
        {
            // Note: Google Sign-In functionality will be implemented in a future update
            await DisplayAlert("Coming Soon", "Google Sign-In functionality will be available soon", "OK");
        }

        private async void OnChangeEmailTapped(object sender, EventArgs e)
        {
            string newEmail = await DisplayPromptAsync("Change Email", "Enter your new email address:", "Change", "Cancel", "New email address");
            
            if (string.IsNullOrWhiteSpace(newEmail))
                return;

            try
            {
                var success = await _authService.ChangeEmailAsync(newEmail);
                if (success)
                {
                    await DisplayAlert("Success", "Email address updated successfully", "OK");
                    LoadUserProfile(); // Refresh the profile
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update email address", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnResetPasswordTapped(object sender, EventArgs e)
        {
            string email = _authService.GetCurrentUserEmail();
            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "No email address found", "OK");
                return;
            }

            try
            {
                var success = await _authService.ResetPasswordAsync(email);
                if (success)
                {
                    await DisplayAlert("Success", "Password reset email has been sent", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to send password reset email", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteAccountTapped(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Delete Account", 
                "Are you sure you want to delete your account? This action cannot be undone.", 
                "Delete", "Cancel");

            if (!confirm)
                return;

            try
            {
                var success = await _authService.DeleteAccountAsync();
                if (success)
                {
                    // Clear Remember Me settings
                    Preferences.Default.Remove(RememberMeKey);
                    Preferences.Default.Remove(SavedEmailKey);
                    Preferences.Default.Remove(AutoLoginKey);
                    
                    await DisplayAlert("Success", "Account deleted successfully", "OK");
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to delete account", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }

        private void OnDisconnectFacebook(object sender, EventArgs e)
        {

            DisplayAlert("Disconnected", "Facebook account has been disconnected.", "OK");
        }

        private void OnDisconnectGoogle(object sender, EventArgs e)
        {

            DisplayAlert("Disconnected", "Facebook account has been disconnected.", "OK");
        }
    }
}

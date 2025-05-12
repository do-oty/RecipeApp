using RecipeApp.Services;
using System.Text.Json;
using Microsoft.Maui.Authentication;
#if ANDROID
using Android.App;
using RecipeApp.Platforms.Android;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Auth.Google;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
#endif

namespace RecipeApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthService _authService;
        private const string RememberMeKey = "RememberMe";
        private const string SavedEmailKey = "SavedEmail";
        private const string AutoLoginKey = "AutoLogin";

        public LoginPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            LoadRememberedCredentials();
            CheckAutoLogin();
        }

        private void LoadRememberedCredentials()
        {
            if (Preferences.Default.Get(RememberMeKey, false))
            {
                RememberMeCheckBox.IsChecked = true;
                EmailEntry.Text = Preferences.Default.Get(SavedEmailKey, string.Empty);
            }
        }

        private async void CheckAutoLogin()
        {
            if (Preferences.Default.Get(AutoLoginKey, false) && _authService.IsUserSignedIn())
            {
                await Shell.Current.GoToAsync("//HomePage");
            }
        }

        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordEntry == null || PasswordValidationIcon == null) return;

            string password = PasswordEntry.Text ?? string.Empty;
            bool isValid = !string.IsNullOrEmpty(password);

            PasswordValidationIcon.Source = isValid ? "check.svg" : "wrong.svg";
            PasswordValidationIcon.IsVisible = !string.IsNullOrEmpty(password);
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailEntry.Text) || string.IsNullOrWhiteSpace(PasswordEntry.Text))
                {
                    await DisplayAlert("Error", "Please enter both email and password", "OK");
                    return;
                }

                var success = await _authService.SignInAsync(EmailEntry.Text, PasswordEntry.Text);
                if (success)
                {
                    // Save credentials if Remember Me is checked
                    bool rememberMe = RememberMeCheckBox.IsChecked;
                    Preferences.Default.Set(RememberMeKey, rememberMe);
                    Preferences.Default.Set(AutoLoginKey, rememberMe);
                    if (rememberMe)
                    {
                        Preferences.Default.Set(SavedEmailKey, EmailEntry.Text);
                    }
                    else
                    {
                        Preferences.Default.Remove(SavedEmailKey);
                        Preferences.Default.Remove(AutoLoginKey);
                    }

                    // Navigate to Home if login is successful
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    await DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            var email = EmailEntry.Text?.Trim();

            if (string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Input Required", "Please enter your email address first.", "OK");
                return;
            }

            try
            {
                var success = await _authService.ResetPasswordAsync(email);
                if (success)
                {
                    await DisplayAlert("Success", "Password reset email has been sent.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to send password reset email.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SignUpPage");
        }

        private async void OnGoogleSignInClicked(object sender, EventArgs e)
        {
#if ANDROID
            try
            {
                var clientId = "729481955530-hvrcg9nra2n171h09s32i3jngug0jhsm.apps.googleusercontent.com";
                var activity = Platform.CurrentActivity as Activity;
                if (activity == null)
                {
                    await DisplayAlert("Error", "Could not get current Android activity.", "OK");
                    return;
                }

                var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(clientId)
                    .RequestEmail()
                    .Build();

                var googleSignInClient = GoogleSignIn.GetClient(activity, gso);
                var signInIntent = googleSignInClient.SignInIntent;
                activity.StartActivityForResult(signInIntent, GoogleSignInService.RC_SIGN_IN);

                try
                {
                    var idToken = await GoogleSignInService.SignInTcs.Task;
                    if (string.IsNullOrEmpty(idToken))
                    {
                        await DisplayAlert("Error", "Google Sign-In failed: No ID token returned.", "OK");
                        return;
                    }
                    var success = await _authService.SignInWithGoogleAsync(idToken);
                    if (success)
                    {
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Google Sign-In failed.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Google Sign-In failed: {ex.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Google Sign-In failed: {ex.Message}", "OK");
            }
#else
            await DisplayAlert("Error", "Google Sign-In is only supported on Android.", "OK");
#endif
        }
    }
}

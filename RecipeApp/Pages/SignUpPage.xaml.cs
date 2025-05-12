using RecipeApp.Services;
#if ANDROID
using Android.App;
using RecipeApp.Platforms.Android;
using Plugin.Firebase.Auth.Google;
using Plugin.Firebase.Auth;
#endif
using Microsoft.Maui.Authentication;

namespace RecipeApp.Pages
{
    public partial class SignUpPage : ContentPage
    {
        private readonly IAuthService _authService;

        public SignUpPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordEntry == null || PasswordHintLabel == null || PasswordValidationIcon == null) return;

            string password = PasswordEntry.Text ?? string.Empty;

            bool isValid = password.Length >= 8 &&
                           password.Any(char.IsUpper) &&
                           password.Any(char.IsLower) &&
                           password.Any(char.IsDigit);

            PasswordHintLabel.IsVisible = !isValid;

            PasswordValidationIcon.Source = isValid ? "check.svg" : "wrong.svg";
            PasswordValidationIcon.IsVisible = !string.IsNullOrEmpty(password);
        }

        private void OnConfirmPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordEntry == null || ConfirmPasswordEntry == null || ConfirmPasswordValidationIcon == null) return;

            bool passwordsMatch = ConfirmPasswordEntry.Text == PasswordEntry.Text && !string.IsNullOrEmpty(ConfirmPasswordEntry.Text);

            ConfirmPasswordValidationIcon.Source = passwordsMatch ? "check.svg" : "wrong.svg";
            ConfirmPasswordValidationIcon.IsVisible = !string.IsNullOrEmpty(ConfirmPasswordEntry.Text);
        }

        private async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailEntry.Text) || 
                    string.IsNullOrWhiteSpace(PasswordEntry.Text) || 
                    string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text))
                {
                    await DisplayAlert("Error", "Please fill in all fields", "OK");
                    return;
                }

                if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
                {
                    await DisplayAlert("Error", "Passwords do not match", "OK");
                    return;
                }

                var success = await _authService.SignUpAsync(EmailEntry.Text, PasswordEntry.Text);
                if (success)
                {
                    await DisplayAlert("Success", "Account created successfully! Please check your email to verify your account.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create account", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async void OnGoogleSignUpClicked(object sender, EventArgs e)
        {
#if ANDROID
            await DisplayAlert("Debug", "Google Sign-Up Clicked", "OK");
            try
            {
                var clientId = "729481955530-hvrcg9nra2n171h09s32i3jngug0jhsm.apps.googleusercontent.com"; // Web Client ID
                var activity = Platform.CurrentActivity as Activity;
                if (activity == null)
                {
                    await DisplayAlert("Error", "Could not get current Android activity.", "OK");
                    return;
                }
                GoogleSignInService.SignIn(activity, clientId);
                try
                {
                    var idToken = await GoogleSignInService.SignInTcs.Task;
                    await DisplayAlert("Debug", $"ID Token: {idToken}", "OK");
                    if (string.IsNullOrEmpty(idToken))
                    {
                        await DisplayAlert("Error", "Google Sign-Up failed: No ID token returned.", "OK");
                        return;
                    }
                    var success = await _authService.SignInWithGoogleAsync(idToken);
                    await DisplayAlert("Debug", $"SignInWithGoogleAsync success: {success}", "OK");
                    if (success)
                    {
                        await DisplayAlert("Success", "Signed up with Google successfully", "OK");
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Google Sign-Up failed.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Google Sign-Up failed: {ex.Message}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Google Sign-Up failed: {ex.Message}", "OK");
            }
#else
            await DisplayAlert("Error", "Google Sign-Up is only supported on Android.", "OK");
#endif
        }
    }
}

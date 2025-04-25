namespace RecipeApp.Pages
{
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
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



    }
}

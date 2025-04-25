namespace RecipeApp.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnBackButtonTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..", true);
        }

        private void OnConfirmPasswordTextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConfirmPasswordEntry == null || ConfirmPasswordValidationIcon == null || PasswordEntry == null) return;

            bool isMatch = ConfirmPasswordEntry.Text == PasswordEntry.Text;

            ConfirmPasswordValidationIcon.Source = isMatch ? "check.svg" : "wrong.svg";
            ConfirmPasswordValidationIcon.IsVisible = true;
        }
    }
}

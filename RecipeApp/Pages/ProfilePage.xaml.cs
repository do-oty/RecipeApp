namespace RecipeApp.Pages
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
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

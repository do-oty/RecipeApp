namespace RecipeApp.Pages
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnLoginTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("LoginPage");
        }

        private async void OnSignUpTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("SignUpPage");
        }

        private async void OnHomePageTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("HomePage");
        }




    }

}

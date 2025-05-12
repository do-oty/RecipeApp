using System.Threading.Tasks;

namespace RecipeApp.Services
{
    public interface IAuthService
    {
        Task<bool> SignUpAsync(string email, string password);
        Task<bool> SignInAsync(string email, string password);
        Task<bool> SignInWithGoogleAsync(string idToken);
        Task<bool> SignOutAsync();
        Task<bool> ResetPasswordAsync(string email);
        Task<bool> SendEmailVerificationAsync();
        Task<bool> ChangeEmailAsync(string newEmail);
        Task<bool> DeleteAccountAsync();
        bool IsUserSignedIn();
        string GetCurrentUserId();
        string GetCurrentUserEmail();
    }

    public class UserProfile
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PhotoUrl { get; set; }
    }
} 
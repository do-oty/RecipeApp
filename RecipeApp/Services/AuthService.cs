using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace RecipeApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private string _currentUserId;
        private string _idToken;
        private string _currentUserEmail;
        private const string ApiKey = "AIzaSyCzG5FTSuD2lHB_RAoEM2WMjk1LihO_GQ4";
        private const string FirebaseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:";
        private const string GoogleClientId = "729481955530-hvrcg9nra2n171h09s32i3jngug0jhsm.apps.googleusercontent.com"; // Actual Google Client ID

        public AuthService()
        {
            _httpClient = new HttpClient();
            // Restore persisted session
            _currentUserId = Preferences.Default.Get<string>("Firebase_UserId", null);
            _idToken = Preferences.Default.Get<string>("Firebase_IdToken", null);
            _currentUserEmail = Preferences.Default.Get<string>("Firebase_UserEmail", null);
        }

        public async Task<bool> SignUpAsync(string email, string password)
        {
            try
            {
                var request = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}signUp?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var authResult = JsonConvert.DeserializeObject<FirebaseAuthResponse>(result);
                    _currentUserId = authResult.LocalId;
                    _idToken = authResult.IdToken;
                    _currentUserEmail = authResult.Email;
                    // Persist session
                    Preferences.Default.Set("Firebase_UserId", _currentUserId);
                    Preferences.Default.Set("Firebase_IdToken", _idToken);
                    Preferences.Default.Set("Firebase_UserEmail", _currentUserEmail);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignInAsync(string email, string password)
        {
            try
            {
                var request = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}signInWithPassword?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var authResult = JsonConvert.DeserializeObject<FirebaseAuthResponse>(result);
                    _currentUserId = authResult.LocalId;
                    _idToken = authResult.IdToken;
                    _currentUserEmail = authResult.Email;
                    // Persist session
                    Preferences.Default.Set("Firebase_UserId", _currentUserId);
                    Preferences.Default.Set("Firebase_IdToken", _idToken);
                    Preferences.Default.Set("Firebase_UserEmail", _currentUserEmail);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignInWithGoogleAsync(string idToken)
        {
            try
            {
                var request = new
                {
                    postBody = $"id_token={idToken}&providerId=google.com",
                    requestUri = "http://localhost",
                    returnSecureToken = true
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}signInWithIdp?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var authResult = JsonConvert.DeserializeObject<FirebaseAuthResponse>(result);
                    _currentUserId = authResult.LocalId;
                    _idToken = authResult.IdToken;
                    // Persist session
                    Preferences.Default.Set("Firebase_UserId", _currentUserId);
                    Preferences.Default.Set("Firebase_IdToken", _idToken);
                    // Note: Google login may not always return email, so only set if available
                    if (!string.IsNullOrEmpty(authResult.Email))
                        Preferences.Default.Set("Firebase_UserEmail", authResult.Email);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Google Sign-In Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SignOutAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(_idToken))
                {
                    var request = new
                    {
                        idToken = _idToken
                    };

                    var response = await _httpClient.PostAsync(
                        $"{FirebaseAuthUrl}signOut?key={ApiKey}",
                        new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                    _currentUserId = null;
                    _idToken = null;
                    _currentUserEmail = null;
                    // Clear persisted session
                    Preferences.Default.Remove("Firebase_UserId");
                    Preferences.Default.Remove("Firebase_IdToken");
                    Preferences.Default.Remove("Firebase_UserEmail");
                    return response.IsSuccessStatusCode;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                var request = new
                {
                    requestType = "PASSWORD_RESET",
                    email = email
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}sendOobCode?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public bool IsUserSignedIn()
        {
            return !string.IsNullOrEmpty(_currentUserId) && !string.IsNullOrEmpty(_idToken);
        }

        public string GetCurrentUserId()
        {
            return _currentUserId;
        }

        public async Task<bool> SendEmailVerificationAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_idToken))
                    return false;

                var request = new
                {
                    requestType = "VERIFY_EMAIL",
                    idToken = _idToken
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}sendOobCode?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ChangeEmailAsync(string newEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(_idToken))
                    return false;

                var request = new
                {
                    idToken = _idToken,
                    email = newEmail,
                    returnSecureToken = true
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}update?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var authResult = JsonConvert.DeserializeObject<FirebaseAuthResponse>(result);
                    _idToken = authResult.IdToken;
                    _currentUserEmail = authResult.Email;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAccountAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_idToken))
                    return false;

                var request = new
                {
                    idToken = _idToken
                };

                var response = await _httpClient.PostAsync(
                    $"{FirebaseAuthUrl}delete?key={ApiKey}",
                    new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    _currentUserId = null;
                    _idToken = null;
                    _currentUserEmail = null;
                    // Clear persisted session
                    Preferences.Default.Remove("Firebase_UserId");
                    Preferences.Default.Remove("Firebase_IdToken");
                    Preferences.Default.Remove("Firebase_UserEmail");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Firebase Auth Error: {ex.Message}");
                return false;
            }
        }

        public string GetCurrentUserEmail()
        {
            return _currentUserEmail;
        }
    }

    public class FirebaseAuthResponse
    {
        [JsonProperty("localId")]
        public string LocalId { get; set; }

        [JsonProperty("idToken")]
        public string IdToken { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
} 
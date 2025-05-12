using Android.App;
using Android.Content;
using Android.Gms.Auth.Api;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using System.Threading.Tasks;
using Android.Widget;
using System;
using Android.Gms.Tasks;
using Android.Util;
using Task = Android.Gms.Tasks.Task;

namespace RecipeApp.Platforms.Android
{
    public class GoogleSignInService
    {
        public static TaskCompletionSource<string> SignInTcs;
        public static string ClientId;
        public const int RC_SIGN_IN = 9001;

        public static void SignIn(Activity activity, string clientId)
        {
            try
            {
                Log.Debug("GoogleSignInService", "Starting Google Sign-In process");
                Toast.MakeText(activity, "Starting Google Sign-In", ToastLength.Short).Show();

                ClientId = clientId;
                SignInTcs = new TaskCompletionSource<string>();

                var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(clientId)
                    .RequestEmail()
                    .Build();

                var googleSignInClient = GoogleSignIn.GetClient(activity, gso);

                // Sign out first to ensure a fresh sign-in
                googleSignInClient.SignOut().AddOnCompleteListener(new OnCompleteListener(() =>
                {
                    try
                    {
                        Log.Debug("GoogleSignInService", "Sign out completed, starting sign-in");
                        // Then start the sign-in process
                        var signInIntent = googleSignInClient.SignInIntent;
                        activity.StartActivityForResult(signInIntent, RC_SIGN_IN);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("GoogleSignInService", $"Error starting sign-in activity: {ex}");
                        Toast.MakeText(activity, $"Error starting sign-in: {ex.Message}", ToastLength.Long).Show();
                        SignInTcs?.TrySetException(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Log.Error("GoogleSignInService", $"Error in SignIn: {ex}");
                Toast.MakeText(activity, $"Error starting sign-in: {ex.Message}", ToastLength.Long).Show();
                SignInTcs?.TrySetException(ex);
            }
        }

        public static async Task<string> GetIdTokenAsync(Activity activity)
        {
            try
            {
                Log.Debug("GoogleSignInService", "Getting ID token");
                Toast.MakeText(activity, "Getting ID token", ToastLength.Short).Show();

                var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestIdToken(ClientId)
                    .RequestEmail()
                    .Build();

                var googleSignInClient = GoogleSignIn.GetClient(activity, gso);
                var signInIntent = googleSignInClient.SignInIntent;
                activity.StartActivityForResult(signInIntent, RC_SIGN_IN);

                var result = await SignInTcs.Task;
                Log.Debug("GoogleSignInService", $"Successfully got ID token: {result?.Substring(0, Math.Min(20, result?.Length ?? 0))}...");
                Toast.MakeText(activity, "Successfully got ID token", ToastLength.Short).Show();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("GoogleSignInService", $"Error in GetIdTokenAsync: {ex}");
                Toast.MakeText(activity, $"Error getting ID token: {ex.Message}", ToastLength.Long).Show();
                throw;
            }
        }

        public static void HandleSignInResult(Activity activity, Result resultCode, Intent data)
        {
            try
            {
                Log.Debug("GoogleSignInService", $"HandleSignInResult called with ResultCode: {resultCode}, Data null: {data == null}");

                if (resultCode == Result.Canceled)
                {
                    // Check if we have data despite the cancelled result
                    if (data != null)
                    {
                        Log.Debug("GoogleSignInService", "Result was cancelled but we have data, attempting to process");
                        var task = GoogleSignIn.GetSignedInAccountFromIntent(data);
                        task.AddOnSuccessListener(new OnSuccessListener());
                        task.AddOnFailureListener(new OnFailureListener(activity, null));
                        return;
                    }

                    var message = "Sign-in was cancelled. Please try again.";
                    Log.Debug("GoogleSignInService", $"Sign-in cancelled. ResultCode: {resultCode}, Data null: {data == null}");
                    Toast.MakeText(activity, message, ToastLength.Short).Show();
                    SignInTcs?.TrySetException(new Exception(message));
                    return;
                }

                if (data == null)
                {
                    var message = "No data received from Google Sign-In. Please try again.";
                    Log.Error("GoogleSignInService", $"No data received. ResultCode: {resultCode}");
                    Toast.MakeText(activity, message, ToastLength.Long).Show();
                    SignInTcs?.TrySetException(new Exception(message));
                    return;
                }

                if (resultCode == Result.Ok)
                {
                    Log.Debug("GoogleSignInService", "Result is OK, processing sign-in data");
                    var task = GoogleSignIn.GetSignedInAccountFromIntent(data);
                    task.AddOnSuccessListener(new OnSuccessListener());
                    task.AddOnFailureListener(new OnFailureListener(activity, null));
                }
                else
                {
                    var message = $"Sign-in failed with result code: {resultCode}. Please try again.";
                    Log.Error("GoogleSignInService", $"Sign-in failed. ResultCode: {resultCode}, Data null: {data == null}");
                    Toast.MakeText(activity, message, ToastLength.Long).Show();
                    SignInTcs?.TrySetException(new Exception(message));
                }
            }
            catch (Exception ex)
            {
                var error = $"Error during sign-in: {ex.Message}";
                Log.Error("GoogleSignInService", $"Exception in HandleSignInResult: {ex}");
                Toast.MakeText(activity, error, ToastLength.Long).Show();
                SignInTcs?.TrySetException(ex);
            }
        }

        private class OnCompleteListener : Java.Lang.Object, IOnCompleteListener
        {
            private readonly Action _onComplete;

            public OnCompleteListener(Action onComplete)
            {
                _onComplete = onComplete;
            }

            public void OnComplete(Task task)
            {
                if (task.IsSuccessful)
                {
                    _onComplete?.Invoke();
                }
                else
                {
                    Log.Error("GoogleSignInService", $"Sign out failed: {task.Exception}");
                }
            }
        }

        private class OnSuccessListener : Java.Lang.Object, IOnSuccessListener
        {
            public void OnSuccess(Java.Lang.Object result)
            {
                try
                {
                    Log.Debug("GoogleSignInService", "OnSuccess called");
                    var account = result as GoogleSignInAccount;
                    if (account == null)
                    {
                        var message = "Could not get Google account information. Please try again.";
                        Log.Error("GoogleSignInService", "GoogleSignInAccount is null");
                        Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Long).Show();
                        SignInTcs?.TrySetException(new Exception(message));
                        return;
                    }

                    var idToken = account?.IdToken;
                    if (string.IsNullOrEmpty(idToken))
                    {
                        var message = "Could not get ID token from Google. Please try again.";
                        Log.Error("GoogleSignInService", "ID Token is null or empty");
                        Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Long).Show();
                        SignInTcs?.TrySetException(new Exception(message));
                        return;
                    }
                    
                    Log.Debug("GoogleSignInService", $"Sign-in successful. Token: {idToken.Substring(0, Math.Min(20, idToken.Length))}...");
                    Toast.MakeText(Platform.CurrentActivity, "Sign-in successful", ToastLength.Short).Show();
                    
                    SignInTcs?.TrySetResult(idToken);
                }
                catch (Exception ex)
                {
                    var message = $"Error processing sign-in: {ex.Message}";
                    Log.Error("GoogleSignInService", $"Exception in OnSuccess: {ex}");
                    Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Long).Show();
                    SignInTcs?.TrySetException(ex);
                }
            }
        }

        private class OnFailureListener : Java.Lang.Object, IOnFailureListener
        {
            private readonly Activity _activity;
            private readonly GoogleSignInClient _googleSignInClient;

            public OnFailureListener(Activity activity, GoogleSignInClient googleSignInClient)
            {
                _activity = activity;
                _googleSignInClient = googleSignInClient;
            }

            public void OnFailure(Java.Lang.Exception e)
            {
                try
                {
                    Log.Debug("GoogleSignInService", $"OnFailure called: {e.Message}");
                    
                    // If we have a GoogleSignInClient, try interactive sign-in
                    if (_googleSignInClient != null)
                    {
                        Log.Debug("GoogleSignInService", "Attempting interactive sign-in");
                        var signInIntent = _googleSignInClient.SignInIntent;
                        _activity.StartActivityForResult(signInIntent, RC_SIGN_IN);
                        return;
                    }

                    // Otherwise, report the failure
                    var message = $"Sign-in failed: {e.Message}";
                    Log.Error("GoogleSignInService", $"OnFailure: {e}");
                    Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Long).Show();
                    SignInTcs?.TrySetException(new Exception(message));
                }
                catch (Exception ex)
                {
                    Log.Error("GoogleSignInService", $"Error in OnFailure: {ex}");
                    Toast.MakeText(Platform.CurrentActivity, $"Error during sign-in: {ex.Message}", ToastLength.Long).Show();
                    SignInTcs?.TrySetException(ex);
                }
            }
        }
    }
} 
using Newtonsoft.Json;
using RESTApp.Models;
using RESTApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Xamarin.Auth;

namespace RESTApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoggingInPage : ContentPage
	{

        public LoggingInPage ()
		{
			InitializeComponent ();

            button_Register.Clicked += RegisterMe;
            button_LogIn.Clicked += LogMeIn;
            button_LogInWithRefreshToken.Clicked += LogMeInUsingRefreshToken;
            button_ExternalLogin_Google.Clicked += LogMeInUsingGoogle;

            string refreshTokenString = null;
            string usernameString = null;
            try
            {
                refreshTokenString = SecureStorage.GetAsync(App.refreshTokenStorageName).GetAwaiter().GetResult();
                usernameString = SecureStorage.GetAsync(App.usernameStorageName).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (refreshTokenString != null)
            {
                button_LogInWithRefreshToken.IsEnabled = true;
                label_LastLogged.Text = "Last logged user: "+usernameString;
            }
        }

        private void RegisterMe(object sender, EventArgs e)
        {
            string requestURL = App.apiPath + App.usersApiPath + "/Register";
            UserModel userData = new UserModel(entry_Username.Text, entry_Password.Text);

            HttpResponseMessage response = 
                HttpRequestSender.SendHttpRequest(requestURL, userData, HttpMethod.Post).GetAwaiter().GetResult();

            if(response.IsSuccessStatusCode)
            {
                label_InfoForUser.Text = "User account created.";
                label_InfoForUser.TextColor = Color.Green;
            }
            else
            {
                label_InfoForUser.Text = "This username is already taken.";
                label_InfoForUser.TextColor = Color.Red;
            }

            return;
        }

        private void LogMeIn(object sender, EventArgs e)
        {
            string requestForRefreshTokenURL = App.apiPath + App.usersApiPath + "/LogIn";
            UserModel userData = new UserModel(entry_Username.Text, entry_Password.Text);

            HttpResponseMessage responseForRefreshToken = 
                HttpRequestSender.SendHttpRequest(requestForRefreshTokenURL, userData, HttpMethod.Post).GetAwaiter().GetResult();

            string refreshTokenString = responseForRefreshToken.Content.ReadAsStringAsync()
                .GetAwaiter().GetResult();

            if (refreshTokenString == "null")
            {
                label_InfoForUser.Text = "There is not such user with that password.";
                label_InfoForUser.TextColor = Color.Red;
                return;
            }

            TokenModel refreshToken = JsonConvert.DeserializeObject<TokenModel>(refreshTokenString);

            try
            {
                SecureStorage.SetAsync(App.refreshTokenStorageName, refreshToken.Token);
                SecureStorage.SetAsync(App.usernameStorageName, entry_Username.Text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            bool result = StartSession(refreshToken);

            if (!result)
            {
                label_InfoForUser.Text = "Server could not start new session.";
                label_InfoForUser.TextColor = Color.Red;
            }
            else
                App.Current.MainPage = new MainPage();
        }

        private void LogMeInUsingRefreshToken(object sender, EventArgs e)
        {
            string refreshTokenString = null;
            try
            {
                refreshTokenString = SecureStorage.GetAsync(App.refreshTokenStorageName).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            TokenModel refreshToken = new TokenModel(refreshTokenString);

            bool result = StartSession(refreshToken);

            if (!result)
            {
                label_InfoForUser.Text = "Previous credentials is no longer valid.";
                label_InfoForUser.TextColor = Color.Red;
            }
            else
                App.Current.MainPage = new MainPage();
        }

        private bool StartSession(TokenModel refreshToken)
        {
            string requestForBearerTokenURL = App.apiPath + App.usersApiPath + "/StartSession";

            HttpResponseMessage responseForBearerToken = 
                HttpRequestSender.SendHttpRequest(requestForBearerTokenURL, refreshToken, HttpMethod.Post).GetAwaiter().GetResult();

            string bearerTokenString = responseForBearerToken.Content.ReadAsStringAsync()
                .GetAwaiter().GetResult();

            if (bearerTokenString == "null")
            {
                return false;
            }

            TokenModel bearerToken = JsonConvert.DeserializeObject<TokenModel>(bearerTokenString);

            BearerTokenModel.Instance.Token = bearerToken.Token;

            return true;
        }

        private void LogMeInUsingGoogle(object sender, EventArgs e)
        {
            string clientId = "77566157603-5llv064c5ukhvh0m4fs011fde4chi4h4.apps.googleusercontent.com";
            string Scope = "https://www.googleapis.com/auth/userinfo.email";
            string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
            string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";

            string RedirectUrl = "com.googleusercontent.apps.77566157603-5llv064c5ukhvh0m4fs011fde4chi4h4:/oauth2redirect";//:/oauth2redirect";

            var authenticator = new OAuth2Authenticator(
                clientId,
                null,
                Scope,
                new Uri(AuthorizeUrl),
                new Uri(RedirectUrl),
                new Uri(AccessTokenUrl),
                null,
                true);

            authenticator.Completed += OnAuthCompletedAsync;
            authenticator.Error += OnAuthErrorAsync;

            AuthenticationState.Authenticator = authenticator;

            var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
            presenter.Login(authenticator);
        }

        private async void OnAuthCompletedAsync(object sender, AuthenticatorCompletedEventArgs e)
        {
            string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompletedAsync;
                authenticator.Error -= OnAuthErrorAsync;
            }

            GoogleUserModel user = null;
            if (e.IsAuthenticated)
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(UserInfoUrl), null, e.Account);
                var response = request.GetResponseAsync().GetAwaiter().GetResult();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    string userJson = response.GetResponseTextAsync().GetAwaiter().GetResult();
                    user = JsonConvert.DeserializeObject<GoogleUserModel>(userJson);

                    await DisplayAlert("Successful authorization", "Google have granted your credentials.", "OK");

                    string requestForBearerTokenURL = App.apiPath + App.usersApiPath + "/LogInUsingGoogleAccount?idToken="
                        + e.Account.Properties["id_token"];

                    HttpResponseMessage responseForBearerToken =
                        HttpRequestSender.SendHttpRequest(requestForBearerTokenURL, user, HttpMethod.Post).GetAwaiter().GetResult();

                    string bearerTokenString = responseForBearerToken.Content.ReadAsStringAsync()
                        .GetAwaiter().GetResult();

                    if (bearerTokenString == "null")
                    {
                        label_InfoForUser.Text = "Service could not start session.";
                        label_InfoForUser.TextColor = Color.Red;
                        return;
                    }

                    TokenModel bearerToken = JsonConvert.DeserializeObject<TokenModel>(bearerTokenString);

                    BearerTokenModel.Instance.Token = bearerToken.Token;
                    App.Current.MainPage = new MainPage();
                    return;
                }
            }

            await DisplayAlert("Unsuccessful authorization", "Google have not granted your credentials", "OK");
        }

        private async void OnAuthErrorAsync(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompletedAsync;
                authenticator.Error -= OnAuthErrorAsync;
            }

            Debug.WriteLine("Authentication error: " + e.Message);
            await DisplayAlert("You can't be logged in", "Logging in using Google identity provider finished with failure", "OK");
        }
    }
}
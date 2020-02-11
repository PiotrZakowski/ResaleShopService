using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RESTApp.Views;
using RESTApp.Services;
using System.Collections.Generic;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RESTApp
{
    public partial class App : Application
    {

        public static string apiPath = "http://192.168.8.101:55129/api"; //"http://153.19.220.85:55129/api"; //"http://192.168.8.100:55129/api";
        public static string usersApiPath = "/users";
        public static string productsApiPath = "/products";
        public static string countriesApiPath = "/countries";
        public static string refreshTokenStorageName = "RefreshToken";
        public static string usernameStorageName = "Username";
        public static ConnectionType currentConnectionType = ConnectionType.Online;
        public static OfflineSynchronizer offlineSync;
        public static string currentAppContextTag = "PLN";
        public static string countryContextPathSuffix = $"?CountryContext={currentAppContextTag}";

        public App()
        {
            InitializeComponent();

            MainPage = new LoggingInPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static void ChangeConnectionMode()
        {
            if (currentConnectionType == ConnectionType.Online)
            {
                currentConnectionType = ConnectionType.Offline;
                offlineSync = new OfflineSynchronizer();
            }
            else
            {
                currentConnectionType = ConnectionType.Online;
                offlineSync.Synchronize();
            }
        }

        public static void SetCurrentAppContextTag(string contextTag)
        {
            currentAppContextTag = contextTag;
            countryContextPathSuffix = $"?CountryContext={currentAppContextTag}";
        }
    }

    public enum ConnectionType
    {
        Online,
        Offline
    }
}

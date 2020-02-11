using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RESTApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoggingOutPage : ContentPage
	{
		public LoggingOutPage ()
		{
			InitializeComponent ();

            try
            {
                SecureStorage.Remove(App.refreshTokenStorageName);
                SecureStorage.Remove(App.usernameStorageName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            App.Current.MainPage = new LoggingInPage();
        }
	}
}
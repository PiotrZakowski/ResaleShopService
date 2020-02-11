using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RESTApp.Models;
using RESTApp.Views;
using RESTApp.ViewModels;
using System.ComponentModel;
using RESTApp.Services;

namespace RESTApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            if(App.currentConnectionType == ConnectionType.Online)
            {
                label_CurrentConnectionMode.Text = "Online mode";
                switch_CurrentConntectionMode.IsToggled = true;
            }
            else
            {
                label_CurrentConnectionMode.Text = "Offline mode";
                switch_CurrentConntectionMode.IsToggled = false;
            }

            switch_CurrentConntectionMode.Toggled += ChangeConnectionMode;

            BindingContext = viewModel = new ItemsViewModel();
        }

        private void ChangeConnectionMode(object sender, ToggledEventArgs e)
        {
            App.ChangeConnectionMode();

            if (App.currentConnectionType == ConnectionType.Online)
            {
                label_CurrentConnectionMode.Text = "Online mode";
                DependencyService.Get<IMessage>().LongAlert("Entered online mode. Each change will be send to server.");
                viewModel.LoadItemsCommand.Execute(null);
                List<String> errorMessages = App.offlineSync.errorMessages;
                foreach (String errorMessage in errorMessages)
                    DisplayAlert("Synchronization error", errorMessage, "Got it");
            }
            else
            {
                label_CurrentConnectionMode.Text = "Offline mode";
                DependencyService.Get<IMessage>().LongAlert("Entered offline mode. Each change will be stored locally.");
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.LoadItemsCommand.Execute(null);

            if (viewModel.Items != null)
                if (viewModel.Items.Count == 0)
                    viewModel.LoadItemsCommand.Execute(null);
        }
    }
}
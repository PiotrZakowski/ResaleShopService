using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RESTApp.Models;
using RESTApp.ViewModels;
using System.Net.Http;
using RESTApp.Services;
using Newtonsoft.Json;

namespace RESTApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                ModelName = "Model name",
                ManufacturerName = "Manufacturer name.",
                Price = 0.0f,
                OriginCountry = "-",
                Quantity = 0
            };

            viewModel = new ItemDetailViewModel(item);
            label_CountryTag.Text = App.currentAppContextTag;
            BindingContext = viewModel;
        }

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            if (App.currentConnectionType == ConnectionType.Online)
            {
                button_ChangeQuantity.Clicked += ChangeQuantity;
                button_ChangeDetails.Clicked += ChangeDetailsEditMode;
                button_UpdateDetails.Clicked += UpdateDetails;
                button_DeleteItem.Clicked += DeleteItemAsync;
            }
            else
            {
                button_ChangeQuantity.Clicked += ChangeQuantity_Offline;
                button_ChangeDetails.Clicked += ChangeDetailsEditMode;
                button_UpdateDetails.Clicked += UpdateDetails_Offline;
                button_DeleteItem.Clicked += DeleteItemAsync_Offline;
            }

            label_CountryTag.Text = App.currentAppContextTag;
            BindingContext = this.viewModel = viewModel;
        }

        private void UpdatePageDetails()
        {
            entry_ModelName.Text = viewModel.Item.ModelName;
            entry_ManufacturerName.Text = viewModel.Item.ManufacturerName;
            entry_Price.Text = viewModel.Item.Price.ToString();
            entry_OriginCountry.Text = viewModel.Item.OriginCountry.ToString();
            label_Quantity.Text = viewModel.Item.Quantity.ToString();
        }

        private void ChangeQuantity(object sender, EventArgs e)
        {
            int quantityChange;
            if (!int.TryParse(entry_QuantityChange.Text, out quantityChange))
            {
                label_InfoForUser.Text = "Quantity change must have only number.";
                label_InfoForUser.TextColor = Color.Red;
                return;
            }

            string requestURL = App.apiPath + App.productsApiPath + "/" + viewModel.Item.Id + "?quantityChange=" + entry_QuantityChange.Text;
            HttpResponseMessage response =
                HttpRequestSender.SendHttpRequest(requestURL, null, HttpMethod.Put, true).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                label_InfoForUser.Text = "Quantity change failed.";
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                    label_InfoForUser.Text += " Quantity can not be negative.";
                label_InfoForUser.TextColor = Color.Red;
                return;
            }

            requestURL = App.apiPath + App.productsApiPath + "/" + viewModel.Item.Id + App.countryContextPathSuffix;
            response = HttpRequestSender.SendHttpRequest(requestURL, null, HttpMethod.Get, true).GetAwaiter().GetResult();

            string editedItemStream = response.Content.ReadAsStringAsync()
               .GetAwaiter().GetResult();

            Item editedItem = JsonConvert.DeserializeObject<Item>(editedItemStream);
            viewModel.Item = editedItem;
            UpdatePageDetails();
            label_InfoForUser.Text = "Quantity change succeded.";
            label_InfoForUser.TextColor = Color.Green;
            //App.Current.MainPage = new MainPage();
        }

        private void ChangeQuantity_Offline(object sender, EventArgs e)
        {
            int quantityChange;
            if (!int.TryParse(entry_QuantityChange.Text, out quantityChange))
            {
                label_InfoForUser.Text = "Quantity change must have only number.";
                label_InfoForUser.TextColor = Color.Red;
                return;
            }

            OfflineRequestModel request = new OfflineRequestModel()
            {
                requestURL = App.apiPath + App.productsApiPath + "/" + viewModel.Item.Id + "?quantityChange=" + entry_QuantityChange.Text,
                data = viewModel.Item,
                requestType = HttpMethod.Put,
                useAuth = true,
                changeType = OfflineChangeType.ChangeQuantity,
                commentary = "Change quantity of product: " +  viewModel.Item.ManufacturerName + " " + viewModel.Item.ModelName + " by: " + entry_QuantityChange.Text
            };
            App.offlineSync.AddToSynchronizeList(request);

            viewModel.Item.Quantity += Int32.Parse(entry_QuantityChange.Text);
            MessagingCenter.Send(this, "ChangeItem", viewModel.Item);

            UpdatePageDetails();
            label_InfoForUser.Text = "Quantity change succeded.";
            label_InfoForUser.TextColor = Color.Green;
            //App.Current.MainPage = new MainPage();
        }

        private void ChangeDetailsEditMode(object sender, EventArgs e)
        {
            entry_ModelName.IsReadOnly = !entry_ModelName.IsReadOnly;
            entry_ManufacturerName.IsReadOnly = !entry_ManufacturerName.IsReadOnly;
            entry_Price.IsReadOnly = !entry_Price.IsReadOnly;
            entry_OriginCountry.IsReadOnly = !entry_OriginCountry.IsReadOnly;

            button_UpdateDetails.IsVisible = !button_UpdateDetails.IsVisible;
            button_ChangeDetails.IsVisible = !button_ChangeDetails.IsVisible;

            label_InfoForUser.Text = "Product details can be changed now";
            label_InfoForUser.TextColor = Color.Black;
        }

        private void UpdateDetails(object sender, EventArgs e)
        {
            string requestURL = App.apiPath + App.productsApiPath + App.countryContextPathSuffix;
            HttpResponseMessage response =
                HttpRequestSender.SendHttpRequest(requestURL, viewModel.Item, HttpMethod.Put, true).GetAwaiter().GetResult();

            requestURL = App.apiPath + App.productsApiPath + "/" + viewModel.Item.Id + App.countryContextPathSuffix;
            response = HttpRequestSender.SendHttpRequest(requestURL, null, HttpMethod.Get, true).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                label_InfoForUser.Text = "Product details update failed.";
                label_InfoForUser.TextColor = Color.Red;
                return;
            }

            string editedItemStream = response.Content.ReadAsStringAsync()
               .GetAwaiter().GetResult();

            Item editedItem = JsonConvert.DeserializeObject<Item>(editedItemStream);
            viewModel.Item = editedItem;
            UpdatePageDetails();
            ChangeDetailsEditMode(sender, e);
            label_InfoForUser.Text = "Product details update succeded.";
            label_InfoForUser.TextColor = Color.Green;
            //App.Current.MainPage = new MainPage();
        }

        private void UpdateDetails_Offline(object sender, EventArgs e)
        {
            OfflineRequestModel request = new OfflineRequestModel()
            {
                requestURL = App.apiPath + App.productsApiPath + App.countryContextPathSuffix,
                data = viewModel.Item,
                requestType = HttpMethod.Put,
                useAuth = true,
                changeType = OfflineChangeType.Update,
                commentary = "Update details of product: " + viewModel.Item.ManufacturerName + " " + viewModel.Item.ModelName
            };
            App.offlineSync.AddToSynchronizeList(request);

            MessagingCenter.Send(this, "ChangeItem", viewModel.Item);

            UpdatePageDetails();
            ChangeDetailsEditMode(sender, e);
            label_InfoForUser.Text = "Product details update succeded.";
            label_InfoForUser.TextColor = Color.Green;
            //App.Current.MainPage = new MainPage();
        }

        private async void DeleteItemAsync(object sender, EventArgs e)
        {
            var userWantToDeleteProduct = await DisplayAlert("Delete product?", "Deletion of this product will be permanent", "Yes", "No");
            if (!userWantToDeleteProduct)
            {
                return;
            }

            string requestURL = App.apiPath + App.productsApiPath + "/" + viewModel.Item.Id;
            HttpResponseMessage response =
                HttpRequestSender.SendHttpRequest(requestURL, null, HttpMethod.Delete, true).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
            {
                label_InfoForUser.Text = "Product deletion failed. You have necessary permitions?";
                label_InfoForUser.TextColor = Color.Red;
                return;
            }

            App.Current.MainPage = new MainPage();
        }

        private async void DeleteItemAsync_Offline(object sender, EventArgs e)
        {
            var userWantToDeleteProduct = await DisplayAlert("Delete product?", "Deletion of this product will be permanent", "Yes", "No");
            if (!userWantToDeleteProduct)
            {
                return;
            }

            OfflineRequestModel request = new OfflineRequestModel()
            {
                requestURL = App.apiPath + App.productsApiPath + "/" + viewModel.Item.Id,
                data = viewModel.Item,
                requestType = HttpMethod.Delete,
                useAuth = true,
                changeType = OfflineChangeType.Delete,
                commentary = "Delete product: " + viewModel.Item.ManufacturerName + " " + viewModel.Item.ModelName
            };
            App.offlineSync.AddToSynchronizeList(request);

            MessagingCenter.Send(this, "DeleteItem", viewModel.Item);

            //App.Current.MainPage = new MainPage();
            await Navigation.PopAsync();
        }
    }
}
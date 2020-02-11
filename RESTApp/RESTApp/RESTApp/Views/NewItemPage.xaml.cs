using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using RESTApp.Models;
using RESTApp.Services;
using System.Net.Http;

namespace RESTApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();

            toolbarItem_Cancel.Clicked += Cancel_Clicked;
            if(App.currentConnectionType == ConnectionType.Online)
                toolbarItem_Save.Clicked += Save_Clicked;
            else
                toolbarItem_Save.Clicked += Save_Clicked_Offline;

            Item = new Item
            {
                ModelName = "",
                ManufacturerName = "",
                OriginCountry = "",
                Price = 0.0f,
                Quantity = 0
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            string requestURL = App.apiPath + App.productsApiPath + App.countryContextPathSuffix;
            HttpResponseMessage response =
                HttpRequestSender.SendHttpRequest(requestURL, Item, HttpMethod.Post, true).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
                return;

            MessagingCenter.Send(this, "AddItem", Item);
            await Navigation.PopModalAsync();
        }

        async void Save_Clicked_Offline(object sender, EventArgs e)
        {
            Item.Id = Guid.NewGuid().GetHashCode();
            OfflineRequestModel request = new OfflineRequestModel()
            {
                requestURL = App.apiPath + App.productsApiPath + App.countryContextPathSuffix,
                data = Item,
                requestType = HttpMethod.Post,
                useAuth = true,
                changeType = OfflineChangeType.Create,
                commentary = "Add new product: " + Item.ManufacturerName + " " + Item.ModelName
            };

            App.offlineSync.AddToSynchronizeList(request);

            MessagingCenter.Send(this, "AddItem", Item);
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
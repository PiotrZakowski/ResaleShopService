﻿using RESTApp.Models;
using RESTApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RESTApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CountryContextsPage : ContentPage
    {
        CountryContextViewModel viewModel;

        public CountryContextsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new CountryContextViewModel();
            //MyListView.ItemsSource = viewModel.Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            CountryContextModel item = e.Item as CountryContextModel;
            App.SetCurrentAppContextTag(item.CountryTag);
            await DisplayAlert("Context changed", "Changed context of the application for "+item.CountryTag+" currency.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
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

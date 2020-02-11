using Newtonsoft.Json;
using RESTApp.Models;
using RESTApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace RESTApp.ViewModels
{
    class CountryContextViewModel : BaseViewModel
    {
        public ObservableCollection<CountryContextModel> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public CountryContextViewModel()
        {
            Title = "Change context";
            Items = new ObservableCollection<CountryContextModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                if (Items == null)
                    Items = new ObservableCollection<CountryContextModel>();
                Items.Clear();

                string requestURL = App.apiPath + App.countriesApiPath;
                HttpResponseMessage response =
                    HttpRequestSender.SendHttpRequest(requestURL, null, HttpMethod.Get, true).GetAwaiter().GetResult();

                string responseString = response.Content.ReadAsStringAsync()
                    .GetAwaiter().GetResult();
                var items = JsonConvert.DeserializeObject<IEnumerable<CountryContextModel>>(responseString);

                foreach (var item in items)
                {
                    Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

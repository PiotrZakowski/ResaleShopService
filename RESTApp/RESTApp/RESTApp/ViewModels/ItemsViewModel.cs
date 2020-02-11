using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using RESTApp.Models;
using RESTApp.Views;
using System.Net.Http;
using RESTApp.Services;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace RESTApp.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as Item;
                try
                {
                    Items.Add(newItem);
                    if (App.currentConnectionType == ConnectionType.Offline)
                        App.offlineSync.OfflineItems = Items;
                    await DataStore.AddItemAsync(newItem);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            MessagingCenter.Subscribe<ItemDetailPage, Item>(this, "ChangeItem", async (obj, item) =>
            {
                var changedItem = item as Item;
                try
                {
                    var oldItem = Items.FirstOrDefault(i => i.Id == changedItem.Id);
                    int oldItemIndex = Items.IndexOf(oldItem);
                    Items[oldItemIndex] = changedItem;
                    if (App.currentConnectionType == ConnectionType.Offline)
                        App.offlineSync.OfflineItems = Items;
                    await DataStore.UpdateItemAsync(changedItem);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });

            MessagingCenter.Subscribe<ItemDetailPage, Item>(this, "DeleteItem", async (obj, item) =>
            {
                try
                {
                    var deletedItem = item as Item;
                    //var oldItem = Items.FirstOrDefault(i => i.Id == deletedItem.Id);
                    //int deletedItemIndex = Items.IndexOf(oldItem);
                    Items.Remove(deletedItem);
                    if (App.currentConnectionType == ConnectionType.Offline)
                        App.offlineSync.OfflineItems = Items;
                    await DataStore.DeleteItemAsync(deletedItem.Id.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {

                if (App.currentConnectionType == ConnectionType.Offline)
                {
                    Items = App.offlineSync.OfflineItems;
                }
                else
                {
                    if (Items == null)
                        Items = new ObservableCollection<Item>();
                    Items.Clear();
                    //var items = await DataStore.GetItemsAsync(true);

                    string requestURL = App.apiPath + App.productsApiPath + App.countryContextPathSuffix;
                    HttpResponseMessage response =
                        HttpRequestSender.SendHttpRequest(requestURL, null, HttpMethod.Get, true).GetAwaiter().GetResult();

                    string responseString = response.Content.ReadAsStringAsync()
                        .GetAwaiter().GetResult();
                    var items = JsonConvert.DeserializeObject<IEnumerable<Item>>(responseString);

                    foreach (var item in items)
                    {
                        Items.Add(item);
                    }
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
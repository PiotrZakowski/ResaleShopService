using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RESTApp.Models;

namespace RESTApp.Services
{
    public class MockDataStore : IDataStore<Item>
    {
        List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>();
            var mockItems = new List<Item>
            {
                new Item { Id = 1, ManufacturerName = "Man1", ModelName="Mod1", Price=11.99f, Quantity=2},
                new Item { Id = 2, ManufacturerName = "Man2", ModelName="Mod2", Price=12.99f, Quantity=3},
                new Item { Id = 3, ManufacturerName = "Man3", ModelName="Mod3", Price=13.99f, Quantity=4},
                new Item { Id = 4, ManufacturerName = "Man4", ModelName="Mod4", Price=14.99f, Quantity=5},
                new Item { Id = 5, ManufacturerName = "Man5", ModelName="Mod5", Price=15.99f, Quantity=6},
                new Item { Id = 6, ManufacturerName = "Man6", ModelName="Mod6", Price=16.99f, Quantity=7},
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == Convert.ToInt32(id)).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == Convert.ToInt32(id)));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}
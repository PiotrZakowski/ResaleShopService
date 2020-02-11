using System;

using RESTApp.Models;

namespace RESTApp.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public ItemDetailViewModel(Item item = null)
        {
            Title = item?.ModelName;
            Item = item;
        }
    }
}

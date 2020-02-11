using System;
using System.Collections.Generic;
using System.Text;

namespace RESTApp.Models
{
    public enum MenuItemType
    {
        Browse,
        About,
        ChangeContext,
        LogOut
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}

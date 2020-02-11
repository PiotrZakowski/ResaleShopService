using System;
using System.Collections.Generic;
using System.Text;

namespace RESTApp.Models
{
    public class UserModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public UserModel()
        {
            Username = "";
            Password = "";
        }

        public UserModel(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return $"{Username}: {Password}";
        }
    }
}

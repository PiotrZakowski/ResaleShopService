using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTService.Models
{
    public class TokenModel
    {
        public string Token { get; set; }

        public TokenModel()
        {
            Token = "";
        }

        public TokenModel(string token)
        {
            this.Token = token;
        }
    }
}
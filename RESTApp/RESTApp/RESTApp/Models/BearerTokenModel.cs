using System;
using System.Collections.Generic;
using System.Text;

namespace RESTApp.Models
{
    public class BearerTokenModel : TokenModel
    {
        private static BearerTokenModel _instance = null;

        public static BearerTokenModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BearerTokenModel();
                return _instance;
            }
        }
    }
}

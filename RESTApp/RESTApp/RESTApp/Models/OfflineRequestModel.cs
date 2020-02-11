using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RESTApp.Models
{
    public enum OfflineChangeType
    {
        Create,
        Update,
        ChangeQuantity,
        Delete
    }

    public class OfflineRequestModel
    {
        public string requestURL { get; set; }
        public Item data { get; set; }
        public HttpMethod requestType { get; set; }
        public bool useAuth { get; set; }
        public OfflineChangeType changeType { get; set; }
        public string commentary { get; set; }

        public OfflineRequestModel()
        { }
    }
}

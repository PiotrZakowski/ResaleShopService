using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace RESTService.Models
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
        public Products data { get; set; }
        public HttpMethod requestType { get; set; }
        public bool useAuth { get; set; }
        public OfflineChangeType changeType { get; set; }
        public string commentary { get; set; }

        public OfflineRequestModel()
        { }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RESTApp.Models
{
    public class OfflineResponseModel
    {
        public bool IsSuccessStatusCode { get; set; }
        public String ReasonPhrase { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public OfflineChangeType changeType { get; set; }
        public string commentary { get; set; }

        public OfflineResponseModel()
        { }
    }
}

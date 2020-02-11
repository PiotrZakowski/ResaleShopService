using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace RESTService.Models
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
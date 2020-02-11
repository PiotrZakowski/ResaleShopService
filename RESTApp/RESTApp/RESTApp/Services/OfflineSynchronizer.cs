using Newtonsoft.Json;
using RESTApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace RESTApp.Services
{
    public class OfflineSynchronizer
    {
        private List<OfflineRequestModel> OfflineRequests;
        public ObservableCollection<Item> OfflineItems { get; set; }
        public List<String> errorMessages;

        public OfflineSynchronizer()
        {
            OfflineRequests = new List<OfflineRequestModel>();
            errorMessages = new List<string>();
        }

        public void AddToSynchronizeList(OfflineRequestModel request)
        {
            OfflineRequests.Add(request);
        }

        public void Synchronize()
        {
            string requestURL = App.apiPath + App.productsApiPath + "/SynchronizeOffline" + App.countryContextPathSuffix;

            HttpResponseMessage response = HttpRequestSender.SendHttpRequest(requestURL, OfflineRequests, HttpMethod.Post, true)
                .GetAwaiter().GetResult();

            if (response.IsSuccessStatusCode)
            {
                string responseStream = response.Content.ReadAsStringAsync()
                   .GetAwaiter().GetResult();

                IEnumerable<OfflineResponseModel> syncResponses = JsonConvert.DeserializeObject<IEnumerable<OfflineResponseModel>>(responseStream);

                foreach(OfflineResponseModel syncResponse in syncResponses)
                {
                    if(!syncResponse.IsSuccessStatusCode)
                    {
                        string message = "Synchronization failed for '" + syncResponse.commentary + "'.\n"+ 
                            "Details: " + (int)syncResponse.StatusCode + " " + syncResponse.ReasonPhrase;
                        errorMessages.Add(message);
                    }
                }
            }
            else
            {
                string message = "Synchronization failed. All changes are lost.\n" +
                    "Details: " + (int)response.StatusCode + " " + response.ReasonPhrase;
                errorMessages.Add(message);
            }
        }
    }
}

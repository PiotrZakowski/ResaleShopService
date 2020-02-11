using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using RESTApp.Models;

namespace RESTApp.Services
{
    public static class HttpRequestSender
    {
        public static async Task<HttpResponseMessage> SendHttpRequest(string requestURL, object userData, HttpMethod requestType, bool useAuth=false)
        {
            HttpClient client = new HttpClient();

            if (useAuth)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", BearerTokenModel.Instance.Token);

            string json;
            StringContent data = null;
            if (userData != null)
            {
                json = JsonConvert.SerializeObject(userData);
                data = new StringContent(json, Encoding.UTF8, "application/json");
            }

            Task<HttpResponseMessage> requestTask;
            if (requestType == HttpMethod.Get)
                requestTask = client.GetAsync(requestURL);
            else if (requestType == HttpMethod.Post)
                requestTask = client.PostAsync(requestURL, data);
            else if (requestType == HttpMethod.Put)
                requestTask = client.PutAsync(requestURL, data);
            else if (requestType == HttpMethod.Delete)
                requestTask = client.DeleteAsync(requestURL);
            else
                return null;

            var response = Task.Run(() => requestTask).Result;

            return response;
        }

    }
}

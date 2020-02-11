using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RESTService.Models
{
    [JsonObject]
    public class GoogleIdTokenClaimsModel
    {
        [JsonProperty("iss")]
        public string iss { get; set; }

        [JsonProperty("azp")]
        public string azp { get; set; }

        [JsonProperty("aud")]
        public string aud { get; set; }

        [JsonProperty("sub")]
        public string sub { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("email_verified")]
        public string email_verified { get; set; }

        [JsonProperty("at_hash")]
        public string at_hash { get; set; }

        [JsonProperty("iat")]
        public string iat { get; set; }

        [JsonProperty("exp")]
        public string exp { get; set; }

        [JsonProperty("alg")]
        public string alg { get; set; }

        [JsonProperty("kid")]
        public string kid { get; set; }

        [JsonProperty("typ")]
        public string typ { get; set; }
    }
}
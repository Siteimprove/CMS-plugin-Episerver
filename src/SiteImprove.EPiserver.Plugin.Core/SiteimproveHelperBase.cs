using System;
using System.Net.Http;
using System.Security;
using System.Text;
using Newtonsoft.Json;

namespace SiteImprove.EPiserver.Plugin.Core
{
    public abstract class SiteimproveHelperBase
    {
        public abstract string GetVersion();
        public string RequestToken()
        {
            using (var client = new HttpClient())
            {
                // Request a token from Siteimprove
                var version = GetVersion();
                string response = client.GetStringAsync(string.Format("{0}?cms=Episerver-{1}", Constants.SiteImproveTokenUrl, version)).Result;
                return JsonConvert.DeserializeObject<dynamic>(response)["token"];
            }
        }

        public void PassEvent(string type, string url, string token)
        {
            var data = new { url, type, token };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = client.PostAsync(Constants.SiteImproveRecheckUrl, content).Result;
            }
        }
    }
}
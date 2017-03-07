using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using SiteImprove.EPiserver.Plugin.Repositories;
using System.Net.Http;

namespace SiteImprove.EPiserver.Plugin
{
    public class SiteimproveHelper
    {
        public static string RequestToken()
        {
            using (var client = new HttpClient())
            {
                // Request a token from Siteimprove
                string response = client.GetStringAsync(Constants.SiteImproveTokenUrl).Result;
                return JsonConvert.DeserializeObject<dynamic>(response)["token"];
            }
        }
    }
}

using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web.Routing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;

namespace SiteImprove.EPiserver.Plugin
{
    public class SiteimproveHelper
    {
        public static string RequestToken()
        {
            using (var client = new HttpClient())
            {
                // Request a token from Siteimprove
                var version = System.Reflection.Assembly.GetAssembly(typeof(SiteimproveHelper)).GetName().Version;
                string response = client.GetStringAsync(string.Format("{0}?cms=Episerver-{1}", Constants.SiteImproveTokenUrl, version)).Result;
                return JsonConvert.DeserializeObject<dynamic>(response)["token"];
            }
        }

        public static void PassEvent(string type, string url, string token)
        {
            var data = new { url, type, token };

            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = client.PostAsync(Constants.SiteImproveRecheckUrl, content).Result;
            }
        }

        public static string GetExternalUrl(PageData page)
        {
            var internalUrl = UrlResolver.Current.GetUrl(page.ContentLink);

            var url = new UrlBuilder(internalUrl);
            Global.UrlRewriteProvider.ConvertToExternal(url, null, System.Text.Encoding.UTF8);

            var friendlyUrl = UriSupport.AbsoluteUrlBySettings(url.ToString());
            return friendlyUrl;
        }
    }
}

using System;
using System.Net.Http;
using System.Text;
using EPiServer;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.Web.Routing;
using Newtonsoft.Json;

namespace SiteImprove.EPiserver.Plugin.Core
{
    public class SiteimproveHelper
    {
        private static ILogger _log = LogManager.GetLogger(typeof(SiteimproveHelper));

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

        public static string GetAdminViewPath(string viewName)
        {
            return "~/modules/_protected/siteimprove/Views/Admin/" + viewName + ".cshtml";
            //return Paths.ToClientResource(typeof(SiteimproveAdminController), "Views/Admin/" + viewName + ".cshtml");
        }

        public static string GetExternalUrl(PageData page)
        {
            try
            {
                var internalUrl = UrlResolver.Current.GetUrl(page.ContentLink);

                if (internalUrl != null) //can be null for special pages like settings
                {
                    var url = new UrlBuilder(internalUrl);
                    Global.UrlRewriteProvider.ConvertToExternal(url, null, System.Text.Encoding.UTF8);

                    var friendlyUrl = UriSupport.AbsoluteUrlBySettings(url.ToString());
                    return friendlyUrl;
                }
                return null;
            }
            catch (Exception ex)
            {
                _log.Error("Could not resolve pageUrl. Perhaps SiteDefinition.Current cannot be resolved? Scheduled jobs requires a * binding to handle SiteDefinition.Current", ex);
                return null;
            }
        }
    }
}

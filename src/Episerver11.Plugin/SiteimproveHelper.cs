using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Newtonsoft.Json;
using SiteImprove.EPiserver.Plugin.Core;

namespace SiteImprove.EPiserver11.Plugin
{
    [ServiceConfiguration(typeof(ISiteimproveHelper))]
    public class SiteimproveHelper : SiteimproveHelperBase, ISiteimproveHelper
    {
        private static readonly ILogger _log = LogManager.GetLogger(typeof(SiteimproveHelper));

        public override string GetVersion()
        {
            var version = System.Reflection.Assembly.GetAssembly(typeof(SiteimproveHelper)).GetName().Version;
            return Settings.Instance.Version + "-" + version;
        }
        public string GetAdminViewPath(string viewName)
        {
            return "~/modules/_protected/siteimprove/Views/Admin/" + viewName + ".cshtml";
            //return Paths.ToClientResource(typeof(SiteimproveAdminController), "Views/Admin/" + viewName + ".cshtml");
        }

        public string GetExternalUrl(PageData page)
        {
            try
            {
                var internalUrl = ServiceLocator.Current.GetInstance<IUrlResolver>().GetUrl(page.ContentLink);

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

        public bool GetPrepublishCheckEnabled(string apiUser, string apiKey)
        {
            using (var client = new HttpClient())
            {
                bool enabled = false;

                var byteArray = Encoding.ASCII.GetBytes($"{apiUser}:{apiKey}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = client.GetAsync($"{Constants.SiteImproveApiUrl}/settings/content_checking").Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    enabled = JsonConvert.DeserializeObject<dynamic>(content)["is_ready"];
                }
                catch (Exception ex)
                {
                    _log.Error("Could not get prepublish check status.", ex);
                }

                return enabled;
            }
        }

        public bool EnablePrepublishCheck(string apiUser, string apiKey)
        {
            using (var client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes($"{apiUser}:{apiKey}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    var response = client.PostAsync($"{Constants.SiteImproveApiUrl}/settings/content_checking", null).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _log.Error("Could not enable prepublish check.", ex);
                    return false;
                }
            }

            return true;
        }
    }
}

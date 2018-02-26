using System;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using SiteImprove.EPiserver.Plugin.Core;

namespace SiteImprove.EPiserver.Plugin
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

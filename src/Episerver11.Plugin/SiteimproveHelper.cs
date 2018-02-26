using System;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Core;

namespace SiteImprove.EPiserver11.Plugin
{
    [ServiceConfiguration(typeof(ISiteimproveHelper))]
    public class SiteimproveHelper : SiteimproveHelperBase, ISiteimproveHelper
    {
        private static ILogger _log = LogManager.GetLogger(typeof(SiteimproveHelper));

        public override string GetVersion()
        {
             var version = System.Reflection.Assembly.GetAssembly(typeof(SiteimproveHelperBase)).GetName().Version;
            return Settings.Instance.Version + "+" + version;
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
                if (page.LinkURL != null) //can be null for special pages like links
                {
                    var url = new UrlBuilder(page.LinkURL);
                    Global.UrlRewriteProvider.ConvertToExternal(url, page.PageLink, System.Text.Encoding.UTF8);

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

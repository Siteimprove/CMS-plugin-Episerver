using System;
using System.Web;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using SiteImprove.EPiserver.Plugin.Core;
using SiteImprove.EPiserver.Plugin.Core.Repositories;

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

                    var externalDomain = ServiceLocator.Current.GetInstance<ISettingsRepository>().GetSetting().ExternalDomain;
                    if (!string.IsNullOrWhiteSpace(externalDomain))
                    {
                        var urlBuilder = new UrlBuilder(externalDomain);
                        if (urlBuilder.Uri.IsAbsoluteUri)
                        {
                            var uri = UriSupport.Combine(urlBuilder.Uri, url.Uri);
                            return uri.ToString();
                        }
                    }

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

        public string GetSiteUrl()
        {
            var externalDomain = ServiceLocator.Current.GetInstance<ISettingsRepository>().GetSetting().ExternalDomain;
            if (!string.IsNullOrWhiteSpace(externalDomain))
            {
                try
                {
                    var urlBuilder = new UrlBuilder(externalDomain);
                    return VirtualPathUtility.AppendTrailingSlash(urlBuilder.ToString());
                }
                catch (Exception ex)
                {
                    _log.Error("Could not resolve External Domain from settings.", ex);
                }
            }

            return SiteDefinition.Current.SiteUrl.ToString();
        }
    }
}

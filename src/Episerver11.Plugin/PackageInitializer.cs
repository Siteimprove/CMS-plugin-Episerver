using System;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Core;
using SiteImprove.EPiserver.Plugin.Core.Repositories;

namespace SiteImprove.EPiserver11.Plugin
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class PackageInitializer : IInitializableModule
    {
        private static bool _installed = false;
        public void AfterInstall(object sender, EventArgs eventArgs)
        {
            if (_installed) return;
            var repo = ServiceLocator.Current.GetInstance<ISettingsRepository>();
            var siteimproveHelper = ServiceLocator.Current.GetInstance<ISiteimproveHelper>();
            string token = siteimproveHelper.RequestToken();

            // Save the token in the repository
            repo.SaveToken(token);
            _installed = true;
        }

        public void Initialize(InitializationEngine context)
        {
            context.InitComplete += AfterInstall;
            RouteTable.Routes.MapRoute(
                "Siteimprove",
                "siteimprove/{action}",
                new { controller = "Siteimprove" });

            RouteTable.Routes.MapRoute(
                "SiteimproveAdmin",
                "siteimproveAdmin",
                new { controller = "SiteimproveAdmin", action = "Index" });
        }

        public void Uninitialize(InitializationEngine context)
        {
            context.InitComplete -= AfterInstall;
        }
    }
}
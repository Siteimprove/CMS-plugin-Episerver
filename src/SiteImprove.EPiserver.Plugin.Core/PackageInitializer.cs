using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Packaging;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Repositories;
using System.Web.Mvc;
using System.Web.Routing;

namespace SiteImprove.EPiserver.Plugin
{
    [ModuleDependency(typeof(PackagingInitialization))]
    [InitializableModule]
    public class PackageInitializer : IInitializableModule, IPackageNotification
    {
        public void AfterInstall()
        {
            string token = SiteimproveHelper.RequestToken();

            // Save the token in the repository
            var repo = ServiceLocator.Current.GetInstance<ISettingsRepository>();
            repo.saveToken(token);
        }

        public void AfterUpdate() { }
        public void BeforeUninstall() { }

        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                "Siteimprove",
                "siteimprove/{action}",
                new {controller = "Siteimprove"});

            RouteTable.Routes.MapRoute(
                "SiteimproveAdmin",
                "siteimproveAdmin",
                new {controller = "SiteimproveAdmin", action = "Index"});
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
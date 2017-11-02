using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Repositories;
using System;

namespace SiteImprove.EPiserver.Plugin
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class EventModule : IInitializableModule
    {
        private ISettingsRepository _settingsRepository;
        private bool _homeIsUnPublished = false;

        public void Initialize(InitializationEngine context)
        {
            this._settingsRepository = ServiceLocator.Current.GetInstance<ISettingsRepository>();

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishedContent += ContentEvents_PublishedContent;
        }

        private void ContentEvents_PublishedContent(object sender, ContentEventArgs e)
        {
            PageData page = e.Content as PageData;

            if (page == null)
                return;

            // Page is home page
            if (page.ContentLink.ID == ContentReference.StartPage.ID)
            {
                if (page.StopPublish.HasValue)
                    this._homeIsUnPublished = page.StopPublish <= DateTime.Now;

                // In event "Publishing", homeIsPublished was false, now it is. Send a recrawl
                if (this._homeIsUnPublished && page.CheckPublishedStatus(PagePublishedStatus.Published))
                {
                    string url = SiteimproveHelper.GetExternalUrl(page);
                    if (url != null) SiteimproveHelper.PassEvent("recrawl", url, this._settingsRepository.getToken());
                    this._homeIsUnPublished = false;
                    return;
                }
            }

            if (page.CheckPublishedStatus(PagePublishedStatus.Published))
            {
                string url = SiteimproveHelper.GetExternalUrl(page);
                if (url != null) SiteimproveHelper.PassEvent("recheck", url, this._settingsRepository.getToken());
            }
            else
            {
                SiteimproveHelper.PassEvent("recheck", "", this._settingsRepository.getToken());
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
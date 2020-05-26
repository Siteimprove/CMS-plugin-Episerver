using System;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Core.Repositories;

namespace SiteImprove.EPiserver.Plugin.Core
{
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class EventModule : IInitializableModule
    {
        private ISettingsRepository _settingsRepository;
        private bool _homeIsUnPublished = false;
        private ISiteimproveHelper _siteimproveHelper;

        public void Initialize(InitializationEngine context)
        {
            this._settingsRepository = ServiceLocator.Current.GetInstance<ISettingsRepository>();
            _siteimproveHelper = ServiceLocator.Current.GetInstance<ISiteimproveHelper>();

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
                    string url = _siteimproveHelper.GetExternalUrl(page);
                    if (url != null) _siteimproveHelper.PassEvent("recrawl", url, this._settingsRepository.GetToken());
                    this._homeIsUnPublished = false;
                    return;
                }
            }

            if (!this._settingsRepository.GetSetting().NoRecheck)
            { 
                if (page.CheckPublishedStatus(PagePublishedStatus.Published))
                {
                    string url = _siteimproveHelper.GetExternalUrl(page);
                    if (url != null) _siteimproveHelper.PassEvent("recheck", url, this._settingsRepository.GetToken());
                }
                else
                {
                    _siteimproveHelper.PassEvent("recheck", "", this._settingsRepository.GetToken());
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
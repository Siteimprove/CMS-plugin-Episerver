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
        private IContentRepository contentRepository;
        private ISettingsRepository settingsRepository;
        private bool homeIsUnPublished = false;

        public void Initialize(InitializationEngine context)
        {
            this.contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            this.settingsRepository = ServiceLocator.Current.GetInstance<ISettingsRepository>();

            var contentEvents = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvents.PublishedContent += ContentEvents_PublishedContent;
        }
        
        private void ContentEvents_PublishedContent(object sender, ContentEventArgs e)
        {
            PageData page = e.Content as PageData;

            if(page == null)
                return;

            string url = SiteimproveHelper.GetExternalUrl(page);

            // Page is home page
            if (page.ContentLink.ID == PageReference.StartPage.ID)
            {
                if (page.StopPublish.HasValue)
                    this.homeIsUnPublished = page.StopPublish <= DateTime.Now;
                
                // In event "Publishing", homeIsPublished was false, now it is. Send a recrawl
                if (this.homeIsUnPublished && page.CheckPublishedStatus(PagePublishedStatus.Published))
                {
                    SiteimproveHelper.PassEvent("recrawl", url, this.settingsRepository.getToken());
                    this.homeIsUnPublished = false;
                    return;
                }
            }
            
            if (page.CheckPublishedStatus(PagePublishedStatus.Published))
            {
                SiteimproveHelper.PassEvent("recheck", url, this.settingsRepository.getToken());
            }
            else if(!page.CheckPublishedStatus(PagePublishedStatus.Published))
            {
                SiteimproveHelper.PassEvent("recheck", "", this.settingsRepository.getToken());
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
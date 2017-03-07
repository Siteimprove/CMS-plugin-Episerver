using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using SiteImprove.EPiserver.Plugin.Models;
using System.Linq;

namespace SiteImprove.EPiserver.Plugin.Repositories
{
    [ServiceConfiguration(typeof(ISettingsRepository))]
    public class SettingsRepository : ISettingsRepository
    {
        private static DynamicDataStore SettingStore
        {
            get
            {
                return typeof(Settings).GetOrCreateStore();
            }
        }

        public string getToken()
        {
            var settings = SettingStore.LoadAll<Settings>().ToArray().FirstOrDefault();

            if(settings == null || string.IsNullOrWhiteSpace(settings.Token))
            {
                string token = SiteimproveHelper.RequestToken();
                saveToken(token);

                return token;
            }

            return settings.Token;
        }

        public void saveToken(string token)
        {
            var current = SettingStore.LoadAll<Settings>().ToArray().FirstOrDefault();
            if(current != null)
            {
                current.Token = token;
                SettingStore.Save(current, current.GetIdentity());
                return;
            }

            SettingStore.Save(new Settings { Token = token });
        }
    }
}

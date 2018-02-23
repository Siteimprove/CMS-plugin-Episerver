using System;

namespace SiteImprove.EPiserver.Plugin.Repositories
{
    public interface ISettingsRepository
    {
        string getToken();
        void saveToken(string token);
    }
}

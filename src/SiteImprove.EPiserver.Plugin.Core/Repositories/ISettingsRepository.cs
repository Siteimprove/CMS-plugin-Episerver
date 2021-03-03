using SiteImprove.EPiserver.Plugin.Core.Models;
namespace SiteImprove.EPiserver.Plugin.Core.Repositories
{
    public interface ISettingsRepository
    {
        string GetToken();
        void SaveToken(string token, bool noRecheck = false, string externalDomain = null);
        Settings GetSetting();
    }
}

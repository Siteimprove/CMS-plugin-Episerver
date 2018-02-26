namespace SiteImprove.EPiserver.Plugin.Core.Repositories
{
    public interface ISettingsRepository
    {
        string GetToken();
        void SaveToken(string token);
    }
}

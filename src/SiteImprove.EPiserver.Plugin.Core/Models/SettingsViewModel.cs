namespace SiteImprove.EPiserver.Plugin.Core.Models
{
    public class SettingsViewModel
    {
        public string Token { get; set; }
        
        public bool NoRecheck { get; set; }

        public string ApiUser { get; set; }

        public string ApiKey { get; set; }

        public bool PrepublishCheckEnabled { get; set; }

        public bool PrepublishError { get; set; }
    }
}

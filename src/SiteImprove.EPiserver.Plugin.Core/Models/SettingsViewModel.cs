namespace SiteImprove.EPiserver.Plugin.Core.Models
{
    public class SettingsViewModel
    {
        public string Token { get; set; }
        
        public bool NoRecheck { get; set; }

        public string ExternalDomain { get; set; }
    }
}

using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace SiteImprove.EPiserver.Plugin.Core.Models
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class Settings
    {
        public Identity Id { get; set; }

        public string Token { get; set; }

        public bool NoRecheck { get; set; }

        public string ApiUser { get; set; }

        public string ApiKey { get; set; }
    }
}

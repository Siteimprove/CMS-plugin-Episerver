using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace SiteImprove.EPiserver.Plugin.Models
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class Settings
    {
        public Identity Id { get; set; }

        public string Token { get; set; }
    }
}

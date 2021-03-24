using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteImprove.EPiserver.Plugin.Core.Models
{
    public class SettingsViewModel
    {
        public string Token { get; set; }
        
        public bool NoRecheck { get; set; }
    }
}

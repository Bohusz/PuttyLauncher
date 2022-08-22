using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Win32;

namespace PuttyLauncher {
    public class PuttyConfigurationsSource {
        public List<PuttyConfigurationItem> Configurations() {
            return GetConfigurations()
                   .Select(WebUtility.UrlDecode)
                   .Select(confName => new PuttyConfigurationItem(LookupIcon(confName), confName))
                   .ToList()
                   ;
        }

        private object LookupIcon(string configurationName) {
            return null;
        }

        public int ConfigurationCount() {
            return Configurations().Count;
        }

        private List<string> GetConfigurations() {
            var sessions = Registry.CurrentUser.OpenSubKey(@"Software\SimonTatham\PuTTY\Sessions");
            return new List<string>(sessions.GetSubKeyNames());
        }
    }

    public struct PuttyConfigurationItem {
        public readonly object icon;
        public readonly string label;
        
        public PuttyConfigurationItem(object icon, string label) {
            this.icon  = icon;
            this.label = label;
        }
        
    }
}

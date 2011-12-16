using System.IO;
using System.Management.Automation;

namespace PsGet {
    public class Settings {
        internal static readonly string TheDefaultSource = "http://packages.psget.org";
        private string _moduleBase;

        public Settings(string moduleBase) {
            _moduleBase = moduleBase;
        }

        public string InstallationRoot {
            get {
                return Path.GetFullPath(Path.Combine(_moduleBase, ".."));
            }
        }

        public string DefaultSource {
            get {
                return TheDefaultSource;
            }
        }
    }
}

using System.IO;
using System.Management.Automation;

namespace PsGet {
    public class Settings {
        private string _moduleBase;

        public Settings(string moduleBase) {
            _moduleBase = moduleBase;
        }

        public string HelperPath {
            get {
                return Path.Combine(_moduleBase, "PsGet.Helper.exe");
            }
        }

        public string InstallationRoot {
            get {
                return Path.GetFullPath(Path.Combine(_moduleBase, ".."));
            }
        }

        public string DefaultSource {
            get {
                return "https://go.microsoft.com/fwlink/?LinkID=206669";
            }
        }
    }
}

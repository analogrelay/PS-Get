using System.IO;
using System.Management.Automation;

namespace PsGet {
    public class Settings {
        private PSModuleInfo _module;

        public Settings(PSModuleInfo module) {
            _module = module;
        }

        public string HelperPath {
            get {
                return Path.Combine(_module.ModuleBase, "PsGet.Helper.exe");
            }
        }

        public string InstallationRoot {
            get {
                return Path.GetFullPath(Path.Combine(_module.ModuleBase, ".."));
            }
        }

        public string DefaultSource {
            get {
                return "https://go.microsoft.com/fwlink/?LinkID=206669";
            }
        }
    }
}

using System.IO;
using System.Management.Automation;

namespace PsGet {
    class Settings {
        private PSModuleInfo _module;

        public Settings(PSModuleInfo module) {
            _module = module;
        }

        public string HelperPath {
            get {
                return Path.Combine(_module.Path, "PsGetHelper.exe");
            }
        }

        public string InstallationRoot {
            get {
                return Path.GetFullPath(Path.Combine(_module.Path, ".."));
            }
        }

        public string DefaultSource {
            get {
                return "https://go.microsoft.com/fwlink/?LinkID=206669";
            }
        }
    }
}

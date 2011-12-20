using System.IO;
using System.Management.Automation;

namespace PsGet {
    public class Settings{
        private string _moduleBase;

        public Settings(string moduleBase) {
            _moduleBase = moduleBase;
        }

        public string InstallationRoot {
            get {
                return Path.GetFullPath(Path.Combine(_moduleBase, ".."));
            }
        }
    }
}

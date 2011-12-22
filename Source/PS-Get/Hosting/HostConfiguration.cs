using System.IO;
using System.Management.Automation;

namespace PsGet.Hosting
{
    public class HostConfiguration
    {
        private string _moduleBase;

        public HostConfiguration(string moduleBase)
        {
            _moduleBase = moduleBase;
        }

        public string InstallationRoot
        {
            get
            {
                return Path.GetFullPath(Path.Combine(_moduleBase, ".."));
            }
        }
    }
}

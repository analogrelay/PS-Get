using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Installer.Models {
    public class InstallPath {
        public string Path { get; set; }
        public bool IsSystemPathIn64BitOs { get; set; }

        public InstallPath(string path)
            : this(path, CheckPath(path)) {
        }

        public InstallPath(string path, bool isSystemPathIn64BitOs) {
            Path = System.IO.Path.GetFullPath(path);
            IsSystemPathIn64BitOs = isSystemPathIn64BitOs;
        }

        private static bool CheckPath(string path) {
            path = System.IO.Path.GetFullPath(path);
            if (Environment.Is64BitOperatingSystem) {
                string systemdir = Environment.GetFolderPath(Environment.SpecialFolder.System);
                string systemx86dir = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
                return
                    path.StartsWith(systemdir) ||
                    path.StartsWith(systemx86dir);
            }
            return false;
        }
    }
}

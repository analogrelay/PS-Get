using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Security.AccessControl;

namespace PsGet.Installer.Models {
    public class InstallPath {
        public string Path { get; set; }
        public bool IsSystemPathIn64BitOs { get; set; }
        public bool RequiresElevation { get; set; }

        public InstallPath(string path)
            : this(path, CheckPath(path), CheckElevate(path)) {
        }

        public InstallPath(string path, bool isSystemPathIn64BitOs, bool requiresElevation) {
            Path = System.IO.Path.GetFullPath(path);
            IsSystemPathIn64BitOs = isSystemPathIn64BitOs;
            RequiresElevation = requiresElevation;
        }

        private static bool CheckElevate(string path) {
            path = System.IO.Path.GetFullPath(path);

            while (!Directory.Exists(path)) {
                path = Directory.GetParent(path).FullName;
            }

            var id = WindowsIdentity.GetCurrent();
            var rules = Directory.GetAccessControl(path).GetAccessRules(
                true,
                true,
                typeof(SecurityIdentifier))
               .OfType<FileSystemAccessRule>()
               .Where(r => (String.Equals(r.IdentityReference.Value, id.User.Value, StringComparison.OrdinalIgnoreCase) ||
                                          id.Groups.Contains(r.IdentityReference)))
               .ToArray();

            var denied = rules.Any(r => r.AccessControlType == AccessControlType.Deny && 
                                        r.FileSystemRights.HasFlag(FileSystemRights.WriteData));

            if (denied) {
                return true;
            }

            var allowed = rules.Any(r => r.AccessControlType == AccessControlType.Allow &&
                                         r.FileSystemRights.HasFlag(FileSystemRights.WriteData));
            if (allowed) {
                try {
                    string testDir = System.IO.Path.Combine(path, "___DeleteMe");
                    Directory.CreateDirectory(testDir);
                    Directory.Delete(testDir);
                    return false;
                }
                catch (UnauthorizedAccessException) {
                    return true;
                }
            }
            return true;
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

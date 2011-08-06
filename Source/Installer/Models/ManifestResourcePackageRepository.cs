using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using System.Reflection;
using System.Security;

namespace PsGet.Installer.Models {
    public class ManifestResourcePackageRepository : IPackageRepository {
        public IQueryable<IPackage> GetPackages() {
            return Assembly.GetExecutingAssembly()
                           .GetManifestResourceNames()
                           .Where(s => s.StartsWith("PsGet.Installer.Packages") && s.EndsWith(".nupkg"))
                           .Select(s => LoadPackage(s))
                           .AsQueryable();
        }

        private IPackage LoadPackage(string resourceName) {
            return new ZipPackage(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
        }

        public void AddPackage(IPackage package) {
            throw new NotSupportedException("ManifestResourcePackageRepository is read-only");
        }

        public void RemovePackage(IPackage package) {
            throw new NotSupportedException("ManifestResourcePackageRepository is read-only");
        }

        public string Source {
            get { return String.Format("res:///{0}", Assembly.GetExecutingAssembly().GetName().ToString()); }
        }
    }
}

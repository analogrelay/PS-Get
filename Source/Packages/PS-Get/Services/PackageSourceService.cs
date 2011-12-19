using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace PsGet.Services
{
    public class PackageSourceService
    {
        private IPackageSourceStore _machineStore;
        private IPackageSourceStore _userStore;
        private IPackageSourceStore _sessionStore;

        public PackageSourceService(IPackageSourceStore machineStore, IPackageSourceStore userStore, IPackageSourceStore sessionStore)
        {
            _machineStore = machineStore;
            _userStore = userStore;
            _sessionStore = sessionStore;
        }

        public IEnumerable<PackageSource> Sources
        {
            get
            {
                return _sessionStore.Sources
                                    .Union(_userStore.Sources, new PackageSourceNameComparer())
                                    .Union(_machineStore.Sources, new PackageSourceNameComparer());
            }
        }

        public void AddSource(PackageSource source, PackageSourceScope scope)
        {
            switch (scope)
            {
                case PackageSourceScope.Machine:
                    _machineStore.AddSource(source);
                    break;
                case PackageSourceScope.User:
                    _userStore.AddSource(source);
                    break;
                case PackageSourceScope.Session:
                    _sessionStore.AddSource(source);
                    break;
            }
        }

        public void RemoveSource(string name, PackageSourceScope scope)
        {
            switch (scope)
            {
                case PackageSourceScope.Machine:
                    _machineStore.RemoveSource(name);
                    break;
                case PackageSourceScope.User:
                    _userStore.RemoveSource(name);
                    break;
                case PackageSourceScope.Session:
                    _sessionStore.RemoveSource(name);
                    break;
            }
        }
    }
}

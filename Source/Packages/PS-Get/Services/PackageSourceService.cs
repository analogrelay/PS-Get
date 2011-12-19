using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace PsGet.Services
{
    public class PackageSourceService
    {
        public virtual IPackageSourceStore MachineStore { get; set; }
        public virtual IPackageSourceStore UserStore { get; set; }
        public virtual IPackageSourceStore SessionStore { get; set; }

        protected internal PackageSourceService() { }

        public PackageSourceService(IPackageSourceStore machineStore, IPackageSourceStore userStore, IPackageSourceStore sessionStore)
        {
            MachineStore = machineStore;
            UserStore = userStore;
            SessionStore = sessionStore;
        }

        public virtual IEnumerable<PackageSource> AllSources
        {
            get
            {
                return SessionStore.Sources
                                   .Union(UserStore.Sources, new PackageSourceNameComparer())
                                   .Union(MachineStore.Sources, new PackageSourceNameComparer());
            }
        }
    }
}

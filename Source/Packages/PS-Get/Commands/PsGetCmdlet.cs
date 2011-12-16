using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading;
using System.Diagnostics;
using NuGet;
using PsGet.Hosting;

namespace PsGet.Cmdlets {
    public abstract class PsGetCmdlet : CommandBase {
        protected internal Settings Settings { get; set; }
        protected internal IPackageRepositoryFactory RepositoryFactory { get; set; }
        
        protected PsGetCmdlet()
        {
            RepositoryFactory = PackageRepositoryFactory.Default;
        }

        protected internal override void BeginProcessingCore() {
            Settings = new Settings(HostEnvironment.ModuleBase);
        }

        protected internal virtual IPackageRepository OpenRepository(string source) {
            return RepositoryFactory.CreateRepository(source);
        }

        protected internal virtual IPackageManager CreatePackageManager(string source, string destination) {
            IPackageRepository repo = RepositoryFactory.CreateRepository(source);
            return new PackageManager(
                repo,
                CreatePackagePathResolver(destination),
                CreateFileSystem(destination)
            );
        }

        protected internal virtual IPackagePathResolver CreatePackagePathResolver(string root) {
            return new DefaultPackagePathResolver(root, useSideBySidePaths: false);
        }

        protected internal virtual IFileSystem CreateFileSystem(string root)
        {
            return new PhysicalFileSystem(root);
        }
    }
}

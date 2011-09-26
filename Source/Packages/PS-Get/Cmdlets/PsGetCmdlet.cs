using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading;
using System.Diagnostics;
using NuGet;

namespace PsGet.Cmdlets {
    public abstract class PsGetCmdlet : PSCmdlet {
        protected Settings Settings { get; set; }

        protected sealed override void BeginProcessing() {
            base.BeginProcessing();
            Settings = new Settings(MyInvocation.MyCommand.Module.ModuleBase);

            // Call Begin Processing
            BeginProcessingCore();
        }

        protected virtual void BeginProcessingCore() {
        }

        protected virtual IPackageRepository OpenRepository(string source) {
            return PackageRepositoryFactory.Default.CreateRepository(source);
        }

        protected PackageManager CreatePackageManager(string source, string destination) {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(source);
            return new PackageManager(
                repo,
                new DefaultPackagePathResolver(destination, useSideBySidePaths: false),
                new PhysicalFileSystem(destination)
            );
        }
    }
}

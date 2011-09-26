using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading;
using System.Diagnostics;
using NuGet;
using PsGet.Utils;

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

        protected Operation StartOperation(string name) {
            return new Operation(name, WriteProgress);
        }

        protected Operation StartOperation(string name, Operation parent) {
            return new Operation(name, parent, WriteProgress);
        }

        protected PackageManager CreatePackageManager(string source, string destination) {
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(source);
            return new PackageManager(
                repo,
                new DefaultPackagePathResolver(destination, useSideBySidePaths: false),
                new PhysicalFileSystem(destination)
            );
        }

        protected void BindOperationToManager(Operation op, PackageManager manager) {
            manager.PackageInstalling += (_, e) => {
                StartOperation(String.Format("Installing {0}", e.Package.GetFullName()), Operation.Current);
            };
            manager.PackageInstalled += (_, e) => {
                Operation.Current.Dispose();
            };
            manager.PackageUninstalling += (_, e) => {
                StartOperation(String.Format("Removing {0}", e.Package.GetFullName()), Operation.Current);
            };
            manager.PackageUninstalled += (_, e) => {
                Operation.Current.Dispose();
            };
            manager.SourceRepository.OnProgressAvailable(e => {
                op.WriteProgress(e.Operation, e.PercentComplete);
            });
        }
    }
}

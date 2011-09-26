using System;
using System.Diagnostics;
using System.Management.Automation;
using NuGet;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsData.Update, "PSPackage")]
    public class UpdatePSPackageCommand : InstallPSPackageCommand {
        protected override string OperationNameTemplate {
            get {
                return "Updating {0}";
            }
        }

        protected override void PerformInstall(PackageManager manager) {
            manager.UpdatePackage(Id, Version != null ? new VersionSpec(Version) : null, !IgnoreDependencies.IsPresent);
        }
    }
}

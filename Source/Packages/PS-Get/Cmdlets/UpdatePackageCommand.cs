using System;
using System.Diagnostics;
using System.Management.Automation;
using NuGet;

namespace PsGet.Cmdlets
{
    [Cmdlet(VerbsData.Update, "Package")]
    public class UpdatePackageCommand : InstallPackageCommand
    {
        protected override string OperationNameTemplate
        {
            get
            {
                return "Updating {0}";
            }
        }

        protected override void PerformInstall(PackageManager manager, string id, Version version, bool ignoreDependencies)
        {
            manager.UpdatePackage(id, version != null ? new VersionSpec(version) : null, !ignoreDependencies);
        }
    }
}

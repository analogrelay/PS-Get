using System;
using System.Diagnostics;
using System.Management.Automation;
using NuGet;

namespace PsGet.Commands
{
    [Cmdlet(VerbsData.Update, "Package")]
    public class UpdatePackageCommand : InstallPackageCommand
    {
        protected internal override void PerformInstall(IPackageManager manager, string id, Version version, bool ignoreDependencies)
        {
            manager.UpdatePackage(id, version, !ignoreDependencies);
        }
    }
}

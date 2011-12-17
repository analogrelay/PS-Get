using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.IO;
using PsGet.Abstractions;

namespace PsGet.Cmdlets
{
    [Cmdlet(VerbsData.Import, "Package")]
    public class ImportPackageCommand : InstallPackageCommand
    {
        protected internal override void ProcessRecordCore()
        {
            // First, check if the module exists
            var module = Invoker.InvokeScript<IModuleMetadata>(
                "Get-Module -ListAvailable " + Id,
                ModuleMetadata.Adapt).FirstOrDefault();
            if (module == null)
            {
                // Install the module
                WriteDebug("Module is not installed, installing");
                DoInstall(Id, Version, IgnoreDependencies.IsPresent, Source, Destination);
            }
            else if (Version != null && !module.Version.Equals(Version))
            {
                // Update the module
                WriteDebug("Module is not installed, installing");
                CreatePackageManager(Source, Destination).UpdatePackage(Id, Version, !IgnoreDependencies.IsPresent);
            }

            // Now import the module
            Invoker.InvokeScript("Import-Module " + Id);
        }
    }
}

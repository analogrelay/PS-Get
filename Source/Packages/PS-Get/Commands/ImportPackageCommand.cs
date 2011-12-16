using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.IO;

namespace PsGet.Cmdlets
{
    [Cmdlet(VerbsData.Import, "Package")]
    public class ImportPackageCommand : InstallPackageCommand
    {
        protected internal override void ProcessRecordCore()
        {
            ImportPackage(Id, Version);
        }

        private void ImportPackage(string id, Version version)
        {
            // First, check if the module exists
            var module = SessionState.InvokeCommand.InvokeScript("Get-Module -ListAvailable " + id).Select(p => p.ImmediateBaseObject).OfType<PSModuleInfo>().FirstOrDefault();
            if (module == null || (version != null && !module.Version.Equals(version)))
            {
                // Install/Update the module
                WriteDebug("Module is not installed or is not the specified version, installing");
                DoInstall(id, version);
            }

            // Now import the module
            SessionState.InvokeCommand.InvokeScript("Import-Module " + Id);
        }
    }
}

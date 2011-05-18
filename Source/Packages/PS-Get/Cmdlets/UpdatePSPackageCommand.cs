using System;
using System.Diagnostics;
using System.Management.Automation;
using PsGet.Communications;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsData.Update, "PSPackage")]
    public class UpdatePSPackageCommand : InstallPSPackageCommand {
        [Parameter(Position = 4)]
        public SwitchParameter DoNotUpdateDependencies { get; set; }

        protected override void InvokeService() {
            Client.Update(Id, Version, !DoNotUpdateDependencies.IsPresent, Source, Destination);
        }
    }
}

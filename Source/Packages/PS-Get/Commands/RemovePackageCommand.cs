using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using NuGet;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsCommon.Remove, "Package")]
    public class RemovePackageCommand : PsGetCmdlet {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        [Parameter]
        public SwitchParameter Force { get; set; }

        [Parameter]
        public SwitchParameter RemoveDependencies { get; set; }

        protected internal override void BeginProcessingCore() {
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }
            if (String.IsNullOrEmpty(Destination)) {
                Destination = Settings.InstallationRoot;
            }
        }

        protected internal override void ProcessRecordCore() {
            WriteDebug(String.Format("Using Source: ", Source));
            WriteDebug(String.Format("Removing From: ", Destination));
            IPackageManager manager = CreatePackageManager(Source, Destination);
            manager.UninstallPackage(Id, null, Force.IsPresent, RemoveDependencies.IsPresent);
        }
    }
}

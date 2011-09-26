using System;
using System.Diagnostics;
using System.Management.Automation;
using NuGet;
using PsGet.Utils;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsLifecycle.Install, "PSPackage")]
    public class InstallPSPackageCommand : PsGetCmdlet {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNull]
        public Version Version { get; set; }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter(Position = 3)]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        [Parameter]
        public SwitchParameter IgnoreDependencies { get; set; }

        protected virtual string OperationNameTemplate { get { return "Installing {0}"; } }

        protected override void BeginProcessingCore() {
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }
            
            if(String.IsNullOrEmpty(Destination)) {
                Destination = Settings.InstallationRoot;
            }
        }

        protected override void ProcessRecord() {
            WriteDebug(String.Format("Using Source: ", Source));
            WriteDebug(String.Format("Installing To: ", Destination));
            string idString = Id;
            if (Version != null) {
                idString += " " + Version.ToString();
            }
            using (Operation op = StartOperation(String.Format("Installing {0}", idString))) {
                PackageManager manager = CreatePackageManager(Source, Destination);
                BindOperationToManager(op, manager);
                PerformInstall(manager);
            }
        }

        protected virtual void PerformInstall(PackageManager manager) {
            manager.InstallPackage(Id, Version, IgnoreDependencies.IsPresent);
        }
    }
}

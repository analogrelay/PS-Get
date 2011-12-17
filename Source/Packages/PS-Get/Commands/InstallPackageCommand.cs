using System;
using System.Diagnostics;
using System.Management.Automation;
using NuGet;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsLifecycle.Install, "Package")]
    public class InstallPackageCommand : PsGetCmdlet {
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

        protected internal override void BeginProcessingCore() {
            base.BeginProcessingCore();
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }

            if (String.IsNullOrEmpty(Destination)) {
                Destination = Settings.InstallationRoot;
            }
        }

        protected internal override void ProcessRecordCore() {
            DoInstall(Id, Version, IgnoreDependencies.IsPresent, Source, Destination);
        }

        protected internal virtual void DoInstall(string id, Version version, bool ignoreDependencies, string source, string destination)
        {
            WriteDebug(String.Concat("Using Source: ", source));
            WriteDebug(String.Concat("Installing To: ", destination));
            IPackageManager manager = CreatePackageManager(source, destination);
            PerformInstall(manager, id, version, ignoreDependencies);
        }

        protected internal virtual void PerformInstall(IPackageManager manager, string id, Version version, bool ignoreDependencies) {
            manager.InstallPackage(id, version, ignoreDependencies);
        }
    }
}

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

        protected override void BeginProcessingCore() {
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }

            if (String.IsNullOrEmpty(Destination)) {
                Destination = Settings.InstallationRoot;
            }
        }

        protected override void ProcessRecord() {
            DoInstall();
        }

        protected void DoInstall()
        {
            DoInstall(Id, Version);
        }

        protected void DoInstall(string id, Version version)
        {
            DoInstall(id, version, IgnoreDependencies.IsPresent, Source, Destination);
        }

        protected void DoInstall(string id, Version version, bool ignoreDependencies, string source, string destination)
        {
            WriteDebug(String.Format("Using Source: ", source));
            WriteDebug(String.Format("Installing To: ", destination));
            PackageManager manager = CreatePackageManager(source, destination);
            PerformInstall(manager, id, version, ignoreDependencies);
        }

        protected virtual void PerformInstall(PackageManager manager, string id, Version version, bool ignoreDependencies) {
            manager.InstallPackage(id, version, ignoreDependencies);
        }
    }
}

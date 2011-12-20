using System;
using System.Diagnostics;
using System.Management.Automation;
using NuGet;
using PsGet.Commands;

namespace PsGet.Commands {
    [Cmdlet(VerbsLifecycle.Install, "Package")]
    public class InstallPackageCommand : PackageManagementCommand {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNull]
        public Version Version { get; set; }

        [Parameter]
        public SwitchParameter IgnoreDependencies { get; set; }

        protected internal override void ProcessRecordCore() {
            DoInstall(Id, Version, IgnoreDependencies.IsPresent, Source, Destination);
        }

        protected internal virtual void DoInstall(string id, Version version, bool ignoreDependencies, string source, string destination)
        {
            IPackageManager manager = CreatePackageManager(source, destination);
            PerformInstall(manager, id, version, ignoreDependencies);
        }

        protected internal virtual void PerformInstall(IPackageManager manager, string id, Version version, bool ignoreDependencies) {
            manager.InstallPackage(id, version, ignoreDependencies);
        }
    }
}

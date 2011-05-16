using System;
using System.Diagnostics;
using System.Management.Automation;
using PsGet.Communications;

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

        protected override void BeginProcessingCore() {
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }
            
            if(String.IsNullOrEmpty(Destination)) {
                Destination = Settings.InstallationRoot;
            }
        }

        protected override void InvokeService() {
            WriteDebug(String.Format("Invoking Install(\"{0}\", \"{1}\", \"{2}\", \"{3}\")", Id, Version, Source, Destination));
            Client.Install(Id, Version, Source, Destination);
            WriteDebug("Successfully Executed");
        }
    }
}

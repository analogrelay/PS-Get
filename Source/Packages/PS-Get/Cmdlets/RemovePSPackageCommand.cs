using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsCommon.Remove, "PSPackage")]
    public class RemovePSPackageCommand : PsGetCmdlet {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        protected override void BeginProcessingCore() {
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }
            if (String.IsNullOrEmpty(Destination)) {
                Destination = Settings.InstallationRoot;
            }
        }

        protected override void InvokeService() {
            Client.Remove(Id, Source, Destination);
        }
    }
}

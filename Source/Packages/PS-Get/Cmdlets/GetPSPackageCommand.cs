using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsCommon.Get, "PSPackage")]
    public class GetPSPackageCommand : PsGetCmdlet {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter]
        public SwitchParameter AllVersions { get; set; }

        protected override void BeginProcessingCore() {
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }
        }

        protected override void InvokeService() {
            WriteDebug(String.Format("Invoking GetPackages(\"{0}\", \"{1}\")", Id, Source));
            WriteObject(Client.GetPackages(Source, String.IsNullOrEmpty(Id) ? null : Id, AllVersions.IsPresent), enumerateCollection: true);
            WriteDebug("Successfully Executed");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using PsGet.Commands;
using PsGet.Services;

namespace PsGet.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "PackageSource")]
    public class RemovePackageSourceCommand : PackageSourceCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected internal override void ProcessRecordCore()
        {
            PackageSourceScope scope = (Scope != null) ? Scope.Value : PackageSourceScope.Session;
            switch(scope) {
                case PackageSourceScope.Session:
                    SourceService.SessionStore.RemoveSource(Name);
                    break;
                case PackageSourceScope.User:
                    SourceService.UserStore.RemoveSource(Name);
                    break;
                case PackageSourceScope.Machine:
                    SourceService.MachineStore.RemoveSource(Name);
                    break;
            }
        }
    }
}

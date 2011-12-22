using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using PsGet.Commands;
using PsGet.Services;

namespace PsGet.Commands
{
    [Cmdlet(VerbsCommon.Remove, "PackageSource")]
    public class RemovePackageSourceCommand : PackageSourceCommand
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected internal override void ProcessRecordCore()
        {
            PackageSourceScope scope = (Scope != null) ? Scope.Value : PackageSourceScope.Session;
            IPackageSourceStore store = SourceService.GetSource(scope);
            store.RemoveSource(Name);
            store.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using PsGet.Commands;
using PsGet.Services;

namespace PsGet.Commands
{
    [Cmdlet(VerbsCommon.Add, "PackageSource")]
    public class AddPackageSourceCommand : PackageSourceCommand
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        protected internal override void ProcessRecordCore()
        {
            PackageSourceScope scope = (Scope != null) ? Scope.Value : PackageSourceScope.Session;
            IPackageSourceStore store = SourceService.GetSource(scope);
            store.AddSource(Source, Name);
            store.Save();
        }
    }
}

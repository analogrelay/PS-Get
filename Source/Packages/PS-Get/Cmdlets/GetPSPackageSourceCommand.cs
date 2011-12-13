using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using PsGet.Abstractions;
using PsGet.Sources;

namespace PsGet.Cmdlets
{
    [Cmdlet(VerbsCommon.Get, "PSPackageSource")]
    public class GetPSPackageSourceCommand : PSCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "Scoped")]
        public PackageSourceListScope Scope { get; set; }

        public GetPSPackageSourceCommand()
        {
            Scope = PackageSourceListScope.User;
        }

        protected override void ProcessRecord()
        {
            PSVariableSessionStorage storage = new PSVariableSessionStorage(SessionState.PSVariable);
            IEnumerable<PsPackageSource> sources;
            if (String.Equals(ParameterSetName, "Scoped"))
            {
                sources = PsPackageSourceList.GetList(Scope, storage).Sources;
            }
            else
            {
                sources = PsPackageSourceList.GetList(storage);
            }
            WriteObject(sources, enumerateCollection: true);
        }
    }
}

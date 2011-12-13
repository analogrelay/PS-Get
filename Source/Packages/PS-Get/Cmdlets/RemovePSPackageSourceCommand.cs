using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using NuGet;
using PsGet.Sources;
using PsGet.Abstractions;

namespace PsGet.Cmdlets
{
    [Cmdlet(VerbsCommon.Remove, "PSPackage")]
    public class RemovePSPackageSourceCommand : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Scoped")]
        public PackageSourceListScope Scope { get; set; }

        private PsPackageSourceList[] _lists;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            PSVariableSessionStorage storage = new PSVariableSessionStorage(SessionState.PSVariable);
            if (String.Equals(ParameterSetName, "Scoped", StringComparison.OrdinalIgnoreCase))
            {
                _lists = new[] { PsPackageSourceList.GetList(Scope, storage) };
            }
            else
            {
                _lists = new[] {
                    PsPackageSourceList.GetList(PackageSourceListScope.Machine, storage),
                    PsPackageSourceList.GetList(PackageSourceListScope.User, storage),
                    PsPackageSourceList.GetList(PackageSourceListScope.Session, storage)
                };
            }
        }

        protected override void ProcessRecord()
        {
            foreach (PsPackageSourceList list in _lists)
            {
                PsPackageSource src = list.Sources.Where(s => String.Equals(s.Source, Source, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (src != null)
                {
                    list.Sources.Remove(src);
                }
            }
        }

        protected override void EndProcessing()
        {
            foreach (PsPackageSourceList list in _lists)
            {
                list.Save();
            }
        }
    }
}

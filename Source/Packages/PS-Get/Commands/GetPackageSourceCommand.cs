using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;
using System.Management.Automation;
using PsGet.Services;
using NuGet;

namespace PsGet.Commands
{
    [Cmdlet(VerbsCommon.Get, "PackageSource")]
    public class GetPackageSourceCommand : PackageSourceManagementCommand
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        protected internal override void BeginProcessingCore()
        {
            base.BeginProcessingCore();
            IEnumerable<PackageSource> sources;
            if (Scope == null)
            {
                sources = SourceService.AllSources;
            }
            else
            {
                switch(Scope.Value) {
                    case PackageSourceScope.Machine:
                        sources = SourceService.MachineStore.Sources;
                        break;
                    case PackageSourceScope.User:
                        sources = SourceService.UserStore.Sources;
                        break;
                    case PackageSourceScope.Session:
                        sources = SourceService.SessionStore.Sources;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown Scope: " + Scope.ToString());
                }
            }
            if (!String.IsNullOrEmpty(Name))
            {
                WildcardPattern pattern = new WildcardPattern(Name);
                sources = sources.Where(s => pattern.IsMatch(s.Name));
            }
            WriteObject(sources, enumerateCollection: true);
        }
    }
}

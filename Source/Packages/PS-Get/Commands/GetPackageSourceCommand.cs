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
    public class GetPackageSourceCommand : PackageSourceCommand
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
                IPackageSourceStore store = SourceService.GetSource(Scope.Value);
                sources = store.Sources;
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

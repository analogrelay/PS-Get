using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using NuGet;
using PsGet.Hosting;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsCommon.Get, "Package")]
    public class GetPackageCommand : PackageManagerCmdlet {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, HelpMessage="A filter to apply to the ID of the packages on the server")]
        public string Id { get; set; }

        [Parameter(Position = 1, HelpMessage="The NuGet feed to list packages from")]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter(HelpMessage="Show all versions of packages")]
        public SwitchParameter AllVersions { get; set; }

        protected internal override void BeginProcessingCore() {
            base.BeginProcessingCore();
            if (String.IsNullOrEmpty(Source)) {
                Source = Settings.DefaultSource;
            }
        }

        protected internal override void ProcessRecordCore() {
            IPackageRepository repo = OpenRepository(Source);
            var query = repo.GetPackages();
            if (!String.IsNullOrEmpty(Id)) {
                // Apply the user filter.
                query = query.Where(p => p.Id.Contains(Id));
            }
            if (!AllVersions.IsPresent) {
                query = query.Where(p => p.IsLatestVersion);
            }

            // Retrieve the packages and output them
            List<IPackage> packages = query.ToList();
            WriteObject(packages, enumerateCollection: true);
        }
    }
}

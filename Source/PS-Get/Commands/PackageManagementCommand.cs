using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Commands;
using System.Management.Automation;
using NuGet;

namespace PsGet.Commands
{
    public abstract class PackageManagementCommand : PsGetCommand
    {
        internal Func<IPackageRepository, string, IPackageManager> PackageManagerFactory { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        protected PackageManagementCommand()
        {
            PackageManagerFactory = CreatePackageManagerCore;
        }

        protected internal override void BeginProcessingCore()
        {
            base.BeginProcessingCore();
            if (String.IsNullOrEmpty(Destination))
            {
                Destination = Config.InstallationRoot;
            }
            WriteDebug(String.Concat("Using Source: ", Source ?? "(Default)"));
            WriteDebug(String.Concat("Installing To: ", Destination));
        }

        protected internal virtual IPackageRepository OpenRepository()
        {
            if (String.IsNullOrEmpty(Source))
            {
                if (!SourceService.AllSources.Any())
                {
                    WriteWarning(
@"No package sources have been configured, so PS-Get doesn't have anywhere to look for packages!
Use the Add-PackageSource cmdlet to add a source. For example:
Add-PackageSource -Name ""PS-Get Gallery"" -Source ""http://packages.psget.org"" -Scope Machine");
                }
                return new AggregateRepository(RepositoryFactory, SourceService.AllSources.Select(s => s.Source), ignoreFailingRepositories: true);
            }
            else
            {
                return RepositoryFactory.CreateRepository(Source);
            }
        }

        protected internal virtual IPackageManager CreatePackageManager()
        {
            return PackageManagerFactory(OpenRepository(), Destination);
        }

        private IPackageManager CreatePackageManagerCore(IPackageRepository repo, string destination)
        {
            return new PackageManager(
                repo,
                CreatePackagePathResolver(destination),
                CreateNuGetFileSystem(destination)
            );
        }

        protected internal virtual IPackagePathResolver CreatePackagePathResolver(string root)
        {
            return new DefaultPackagePathResolver(root, useSideBySidePaths: false);
        }

        protected internal virtual IFileSystem CreateNuGetFileSystem(string root)
        {
            return new PhysicalFileSystem(root);
        }
    }
}

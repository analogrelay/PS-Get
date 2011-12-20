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
                Destination = Settings.InstallationRoot;
            }
            WriteDebug(String.Concat("Using Source: ", Source ?? "(Default)"));
            WriteDebug(String.Concat("Installing To: ", Destination));
        }

        protected internal virtual IPackageRepository OpenRepository()
        {
            if (String.IsNullOrEmpty(Source))
            {
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
                CreateFileSystem(destination)
            );
        }

        protected internal virtual IPackagePathResolver CreatePackagePathResolver(string root)
        {
            return new DefaultPackagePathResolver(root, useSideBySidePaths: false);
        }

        protected internal virtual IFileSystem CreateFileSystem(string root)
        {
            return new PhysicalFileSystem(root);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Abstractions;
using PsGet.Hosting;
using NuGet;
using IFileSystem = PsGet.Abstractions.IFileSystem;
using PhysicalFileSystem = PsGet.Abstractions.PhysicalFileSystem;
using System.IO;

namespace PsGet.Commands
{
    [Cmdlet(VerbsData.Export, "Module")]
    public class ExportModuleCommand : PsGetCommand
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNullOrEmpty]
        public string PackageFile { get; set; }

        protected internal override void ProcessRecordCore()
        {
            base.ProcessRecordCore();
            
            // Get the module metadata
            IModuleMetadata module =
                Invoker.InvokeScript("Get-Module -ListAvailable '{0}'", ModuleMetadata.Adapt, Name)
                       .FirstOrDefault<IModuleMetadata>();
            if (module == null)
            {
                throw new KeyNotFoundException(String.Format("Module not found: '{0}'", Name));
            }
            if (!PackageIdValidator.IsValidPackageId(module.Name))
            {
                throw new InvalidOperationException(String.Format(
                    "Module name '{0}' is invalid, names must consist only of letters, numbers, '_' and '-'.",
                    module.Name));
            }

            // First, check for a nuspec
            IFileSystem moduleRoot = CreateFileSystem(module.ModuleBase);
            PackageBuilder builder = BuildManifest(module, moduleRoot);

            // Save the file
            SavePackage(builder);
        }

        protected internal virtual PackageBuilder BuildManifest(IModuleMetadata module, IFileSystem moduleRoot)
        {
            string nuspecName = String.Format("{0}.nuspec", module.Name);
            PackageBuilder builder;
            if (moduleRoot.FileExists(nuspecName))
            {
                using (Stream strm = moduleRoot.OpenFile(nuspecName))
                {
                    builder = OpenManifest(module.ModuleBase, strm);
                }
            }
            else
            {
                // Create the package
                builder = new PackageBuilder();
                builder.Id = module.Name;
                builder.Version = new SemanticVersion(module.Version);
                builder.Authors.AddRange(module.Author.Split(',').Select(s => s.Trim()));
                builder.Description = module.Description;
            }

            // Add all files under the module base if there aren't any already
            if (!builder.Files.Any())
            {
                foreach (var file in moduleRoot.GetAllFiles())
                {
                    builder.Files.Add(new PhysicalPackageFile()
                    {
                        SourcePath = moduleRoot.GetFullPath(file),
                        TargetPath = file
                    });
                }
            }
            return builder;
        }

        protected internal virtual PackageBuilder OpenManifest(string moduleBase, Stream strm)
        {
            return new PackageBuilder(strm, moduleBase);
        }

        protected internal virtual void SavePackage(PackageBuilder builder)
        {
            if (String.IsNullOrEmpty(PackageFile))
            {
                PackageFile = Path.Combine(PathService.CurrentPath, String.Format("{0}.nupkg", builder.Id));
            }
            SavePackage(builder, PathService.ResolvePath(PackageFile));
        }

        protected internal virtual void SavePackage(PackageBuilder builder, string path)
        {
            IFileSystem root = CreateFileSystem();
            using (Stream strm = root.OpenFile(path))
            {
                builder.Save(strm);
            }
        }
    }
}   
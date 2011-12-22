using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Management.Automation;
using NuGet;
using PsGet.Abstractions;
using PsGet.Hosting;
using IFileSystem = PsGet.Abstractions.IFileSystem;

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

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Description { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string PackageId { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public SemanticVersion Version { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string[] Authors { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Copyright { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string IconUrl { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Language { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string LicenseUrl { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string[] Owners { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string ProjectUrl { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string ReleaseNotes { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public SwitchParameter RequireLicenseAcceptance { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Summary { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string[] Tags { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Title { get; set; }

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
                if (!String.IsNullOrEmpty(module.Author))
                {
                    builder.Authors.AddRange(module.Author.Split(',').Select(s => s.Trim()));
                }
                else
                {
                    builder.Authors.Add(Environment.UserName);
                }
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

            // Set overrides
            builder.Description = Description ?? builder.Description;
            builder.Id = PackageId ?? builder.Id;
            builder.Version = Version ?? builder.Version;
            if (Authors != null)
            {
                builder.Authors.Clear();
                builder.Authors.AddRange(Authors);
            }
            builder.Copyright = Copyright;
            builder.IconUrl = String.IsNullOrEmpty(IconUrl) ? null : new Uri(IconUrl);
            builder.Language = Language;
            builder.LicenseUrl = String.IsNullOrEmpty(LicenseUrl) ? null : new Uri(LicenseUrl);
            if (Owners != null)
            {
                builder.Owners.Clear();
                builder.Owners.AddRange(Owners);
            }
            builder.ProjectUrl = String.IsNullOrEmpty(ProjectUrl) ? null : new Uri(ProjectUrl);
            builder.ReleaseNotes = ReleaseNotes;
            builder.RequireLicenseAcceptance = RequireLicenseAcceptance.IsPresent;
            builder.Summary = Summary;
            if (Tags != null)
            {
                builder.Tags.Clear();
                builder.Tags.AddRange(Tags);
            }
            builder.Title = Title;

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
            try
            {
                IFileSystem root = CreateFileSystem();
                using (Stream strm = root.OpenFile(path))
                {
                    builder.Save(strm);
                }
            }
            catch (ValidationException vex)
            {
                throw new InvalidDataException(
                    String.Format(
                        "Manifest is missing some data required by NuGet. " +
                        "Add the data to a Module Manifest or use the parameters of " +
                        "Export-Module to specify it: {0}", vex.Message), vex);
            }
        }
    }
}   
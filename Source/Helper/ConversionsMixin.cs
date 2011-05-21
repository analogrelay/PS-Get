using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ser = PsGet.Helper.Serializables;
using NuGet;
using System.IO;
using System.Runtime.Versioning;

namespace PsGet.Helper {
    public static class ConversionsMixin {
        public static PackageBuilder ToNuGet(this Ser.PackageSpec self) {
            PackageBuilder builder = new PackageBuilder() {
                Description = self.Description,
                IconUrl = String.IsNullOrEmpty(self.IconUrl) ? null : new Uri(self.IconUrl),
                Id = self.Id,
                Language = self.Language,
                LicenseUrl = String.IsNullOrEmpty(self.LicenseUrl) ? null : new Uri(self.LicenseUrl),
                ProjectUrl = String.IsNullOrEmpty(self.ProjectUrl) ? null : new Uri(self.ProjectUrl),
                RequireLicenseAcceptance = self.RequireLicenseAcceptance,
                Summary = self.Summary,
                Title = self.Title,
                Version = self.Version
            };

            builder.Authors.AddRange(self.Authors);
            builder.Dependencies.AddRange(self.Dependencies.Select(d => d.ToNuGet()));
            builder.Files.AddRange(CreateFiles(self.RootFolder));
            builder.FrameworkReferences.AddRange(self.FrameworkReferences.Select(f => f.ToNuGet()));
            builder.Owners.AddRange(self.Owners);
            builder.Tags.AddRange(self.Tags);
            
            return builder;
        }

        public static NuGet.FrameworkAssemblyReference ToNuGet(this Ser.FrameworkAssemblyReference self) {
            return new NuGet.FrameworkAssemblyReference(
                self.AssemblyName,
                self.SupportedFrameworks.Select(f => f.ToNetFx()));
        }

        public static FrameworkName ToNetFx(this Ser.FrameworkName name) {
            return new FrameworkName(name.Identifier, name.Version, name.Profile);
        }

        public static NuGet.PackageDependency ToNuGet(this Ser.PackageDependency self) {
            return new NuGet.PackageDependency(self.Id, self.VersionSpec.ToNuGet());
        }

        public static NuGet.VersionSpec ToNuGet(this Ser.VersionSpec self) {
            return new NuGet.VersionSpec() {
                IsMaxInclusive = self.IsMaxInclusive,
                IsMinInclusive = self.IsMinInclusive,
                MaxVersion = self.MaxVersion,
                MinVersion = self.MinVersion
            };
        }

        public static IEnumerable<NuGet.IPackageFile> CreateFiles(string basePath) {
            PhysicalFileSystem fs = new PhysicalFileSystem(basePath);
            foreach (string file in fs.GetFilesRecursive()) {
                yield return new PhysicalPackageFile() {
                    SourcePath = Path.Combine(basePath, file),
                    TargetPath = file
                };
            }
        }

        private static IEnumerable<string> GetFilesRecursive(this IFileSystem self) {
            return GetFilesRecursive(self, String.Empty);
        }

        private static IEnumerable<string> GetFilesRecursive(IFileSystem self, string root) {
            foreach (string dir in self.GetDirectories(root)) {
                foreach (string file in GetFilesRecursive(self, Path.Combine(root, dir))) {
                    yield return file;
                }
            }
            foreach (string file in self.GetFiles(root)) {
                yield return file;
            }
        }
    }
}

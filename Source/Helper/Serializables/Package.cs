using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class Package {
        public Package() { }
        
        public Package(NuGet.IPackage pkg) {
            AssemblyReferences = pkg.AssemblyReferences.Select(p => new PackageAssemblyReference(p)).ToList();
            Authors = pkg.Authors.ToList();
            Dependencies = pkg.Dependencies.Select(p => new PackageDependency(p)).ToList();
            Description = pkg.Description;
            FrameworkAssemblies = pkg.FrameworkAssemblies.Select(p => new FrameworkAssemblyReference(p)).ToList();
            IconUrl = pkg.IconUrl;
            Id = pkg.Id;
            Language = pkg.Language;
            LicenseUrl = pkg.LicenseUrl;
            Owners = pkg.Owners.ToList();
            ProjectUrl = pkg.ProjectUrl;
            RequireLicenseAcceptance = pkg.RequireLicenseAcceptance;
            Summary = pkg.Summary;
            Tags = pkg.Tags;
            Title = pkg.Title;
            Version = pkg.Version;
            DownloadCount = pkg.DownloadCount;
            Rating = pkg.Rating;
            RatingsCount = pkg.RatingsCount;
            ReportAbuseUrl = pkg.ReportAbuseUrl;
        }

        [DataMember]
        public ICollection<PackageAssemblyReference> AssemblyReferences { get; set; }
        [DataMember]
        public ICollection<string> Authors { get; set; }
        [DataMember]
        public ICollection<PackageDependency> Dependencies { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public ICollection<FrameworkAssemblyReference> FrameworkAssemblies { get; set; }
        [DataMember]
        public Uri IconUrl { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public Uri LicenseUrl { get; set; }
        [DataMember]
        public ICollection<string> Owners { get; set; }
        [DataMember]
        public Uri ProjectUrl { get; set; }
        [DataMember]
        public bool RequireLicenseAcceptance { get; set; }
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public string Tags { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public Version Version { get; set; }
        [DataMember]
        public int DownloadCount { get; set; }
        [DataMember]
        public double Rating { get; set; }
        [DataMember]
        public int RatingsCount { get; set; }
        [DataMember]
        public Uri ReportAbuseUrl { get; set; }
    }
}

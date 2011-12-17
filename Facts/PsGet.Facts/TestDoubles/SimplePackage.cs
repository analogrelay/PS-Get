using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Facts.TestDoubles
{
    // Test Doubles are tested via their use in unit tests
    [ExcludeFromCodeCoverage]
    public class SimplePackage : IPackage
    {
        public string Id { get; set; }
        public Version Version { get; set; }
        public bool IsLatestVersion { get; set; }
        
        public SimplePackage(string id, Version version, bool latest)
        {
            Id = id;
            Version = version;
            IsLatestVersion = latest;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}{2}", Id, Version, IsLatestVersion ? "*" : String.Empty);
        }

        public override bool Equals(object obj)
        {
            IPackage other = obj as IPackage;
            return String.Equals(other.Id, Id) && Equals(other.Version, Version) && other.IsLatestVersion == IsLatestVersion;
        }

        public override int GetHashCode()
        {
            // Don't care
            return base.GetHashCode();
        }
        
        public IEnumerable<IPackageAssemblyReference> AssemblyReferences
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IPackageFile> GetFiles()
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetStream()
        {
            throw new NotImplementedException();
        }

        public DateTimeOffset? Published
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> Authors
        {
            get { throw new NotImplementedException(); }
        }

        public string Copyright
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<PackageDependency> Dependencies
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies
        {
            get { throw new NotImplementedException(); }
        }

        public Uri IconUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string Language
        {
            get { throw new NotImplementedException(); }
        }

        public Uri LicenseUrl
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<string> Owners
        {
            get { throw new NotImplementedException(); }
        }

        public Uri ProjectUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string ReleaseNotes
        {
            get { throw new NotImplementedException(); }
        }

        public bool RequireLicenseAcceptance
        {
            get { throw new NotImplementedException(); }
        }

        public string Summary
        {
            get { throw new NotImplementedException(); }
        }

        public string Tags
        {
            get { throw new NotImplementedException(); }
        }

        public string Title
        {
            get { throw new NotImplementedException(); }
        }

        public int DownloadCount
        {
            get { throw new NotImplementedException(); }
        }

        public double Rating
        {
            get { throw new NotImplementedException(); }
        }

        public int RatingsCount
        {
            get { throw new NotImplementedException(); }
        }

        public Uri ReportAbuseUrl
        {
            get { throw new NotImplementedException(); }
        }
    }
}

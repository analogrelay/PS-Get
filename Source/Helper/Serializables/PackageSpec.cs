using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class PackageSpec {
        public PackageSpec()
            : base() {
        }

        [DataMember]
        public ICollection<string> Authors { get; set; }
        [DataMember]
        public ICollection<PackageDependency> Dependencies { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string RootFolder { get; set; }
        [DataMember]
        public ICollection<FrameworkAssemblyReference> FrameworkReferences { get; set; }
        [DataMember]
        public string IconUrl { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string LicenseUrl { get; set; }
        [DataMember]
        public ICollection<string> Owners { get; set; }
        [DataMember]
        public string ProjectUrl { get; set; }
        [DataMember]
        public bool RequireLicenseAcceptance { get; set; }
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public ICollection<string> Tags { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public Version Version { get; set; }
    }
}

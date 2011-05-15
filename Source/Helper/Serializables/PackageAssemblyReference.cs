using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Versioning;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class PackageAssemblyReference {
        public PackageAssemblyReference() { }
        public PackageAssemblyReference(NuGet.IPackageAssemblyReference @ref) {
            SupportedFrameworks = @ref.SupportedFrameworks.ToList();
            Path = @ref.Path;
            Name = @ref.Name;
            TargetFramework = @ref.TargetFramework;
        }

        [DataMember]
        public ICollection<FrameworkName> SupportedFrameworks { get; set; }
        [DataMember]
        public string Path { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public FrameworkName TargetFramework { get; set; }
    }
}

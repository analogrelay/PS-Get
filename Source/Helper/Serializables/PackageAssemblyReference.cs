using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class PackageAssemblyReference {
        public PackageAssemblyReference() { }
        public PackageAssemblyReference(NuGet.IPackageAssemblyReference @ref) {
            SupportedFrameworks = @ref.SupportedFrameworks.Select(fn => new FrameworkName(fn)).ToArray();
            Path = @ref.Path;
            Name = @ref.Name;
            TargetFramework = new FrameworkName(@ref.TargetFramework);
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

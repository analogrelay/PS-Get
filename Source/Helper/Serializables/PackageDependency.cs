using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class PackageDependency {
        public PackageDependency() { }
        public PackageDependency(NuGet.PackageDependency dep) {
            Id = dep.Id;
            VersionSpec = new VersionSpec(dep.VersionSpec);
        }

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public VersionSpec VersionSpec { get; set; }
    }
}

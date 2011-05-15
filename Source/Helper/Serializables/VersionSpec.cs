using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class VersionSpec {
        public VersionSpec() { }
        public VersionSpec(NuGet.IVersionSpec spec) {
            IsMaxInclusive = spec.IsMaxInclusive;
            IsMinInclusive = spec.IsMinInclusive;
            MaxVersion = spec.MaxVersion;
            MinVersion = spec.MinVersion;
        }

        [DataMember]
        public bool IsMaxInclusive { get; set; }
        [DataMember]
        public bool IsMinInclusive { get; set; }
        [DataMember]
        public Version MaxVersion { get; set; }
        [DataMember]
        public Version MinVersion { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class FrameworkName {
        public FrameworkName() { }
        public FrameworkName(System.Runtime.Versioning.FrameworkName fn) {
            FullName = fn.FullName;
            Identifier = fn.Identifier;
            Profile = fn.Profile;
            Version = fn.Version;
        }

        [DataMember]
        public string FullName { get; set; }
        [DataMember]
        public string Identifier { get; set; }
        [DataMember]
        public string Profile { get; set; }
        [DataMember]
        public Version Version { get; set; }
    }
}

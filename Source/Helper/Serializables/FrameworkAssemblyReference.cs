using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PsGet.Helper.Serializables {
    [DataContract]
    public class FrameworkAssemblyReference {
        public FrameworkAssemblyReference() { }
        public FrameworkAssemblyReference(NuGet.FrameworkAssemblyReference @ref) {
            AssemblyName = @ref.AssemblyName;
            SupportedFrameworks = @ref.SupportedFrameworks.Select(fn => new FrameworkName(fn)).ToList();
        }

        [DataMember]
        public string AssemblyName { get; set; }
        [DataMember]
        public ICollection<FrameworkName> SupportedFrameworks { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace PsGet.Utils {
    internal static class PSObjectMixin {
        public static T As<T>(this PSObject self) where T : class {
            return self == null ? default(T) : self.ImmediateBaseObject as T;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Abstractions
{
    // Can't do unit tests here because of the dependency on PowerShell
    [ExcludeFromCodeCoverage]
    internal class PSModuleInfoAdapter : IModuleMetadata
    {
        internal PSModuleInfo Module { get; private set; }

        public Version Version
        {
            get { return Module.Version; }
        }

        public PSModuleInfoAdapter(PSModuleInfo module)
        {
            Module = module;
        }
    }
}

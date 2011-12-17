using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Abstractions
{
    internal interface IModuleMetadata
    {
        Version Version { get; }
    }

    // Can't do unit tests here because of the dependency on PowerShell
    [ExcludeFromCodeCoverage]
    internal static class ModuleMetadata
    {
        internal static IModuleMetadata Adapt(object o)
        {
            IModuleMetadata converted = o as IModuleMetadata;
            if (converted != null)
            {
                return converted;
            }
            return AdaptPowerShellModule(o);
        }

        private static IModuleMetadata AdaptPowerShellModule(object o)
        {
            PSModuleInfo module = o as PSModuleInfo;
            if (module != null)
            {
                return new PSModuleInfoAdapter(module);
            }

            // Force an InvalidCastException with the proper messaging.
            return (IModuleMetadata)o;
        }
    }
}

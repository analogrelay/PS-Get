using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Hosting
{
    // Can't do unit tests here because of the dependency on PowerShell
    [ExcludeFromCodeCoverage]
    internal class PowerShellHostEnvironment : IHostEnvironment
    {
        public InvocationInfo Invocation { get; private set; }

        public PowerShellHostEnvironment(InvocationInfo invocation)
        {
            Invocation = invocation;
        }

        public string ModuleBase
        {
            get { return Invocation.MyCommand.Module.ModuleBase; }
        }
    }
}

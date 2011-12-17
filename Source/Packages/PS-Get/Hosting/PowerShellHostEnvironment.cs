using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace PsGet.Hosting
{
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

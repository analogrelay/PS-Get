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
        private PSCmdlet _cmdlet;

        public PowerShellHostEnvironment(PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public string ModuleBase
        {
            get { return _cmdlet.MyInvocation.MyCommand.Module.ModuleBase; }
        }
    }
}

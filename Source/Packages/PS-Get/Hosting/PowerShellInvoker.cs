using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Hosting
{
    internal class PowerShellInvoker : ICommandInvoker
    {
        private System.Management.Automation.CommandInvocationIntrinsics InvokeCommand;

        public PowerShellInvoker(System.Management.Automation.CommandInvocationIntrinsics InvokeCommand)
        {
            // TODO: Complete member initialization
            this.InvokeCommand = InvokeCommand;
        }
    }
}

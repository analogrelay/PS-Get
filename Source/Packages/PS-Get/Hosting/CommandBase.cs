using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace PsGet.Hosting
{
    public abstract class CommandBase : PSCmdlet
    {
        public ISessionStorage Session { get; set; }
        public ICommandInvoker Invoker { get; set; }
        public IHostEnvironment HostEnvironment { get; set; }

        // Test helper methods
        internal void FireBeginProcessing()
        {
            BeginProcessing();
        }

        internal void FireEndProcessing()
        {
            EndProcessing();
        }

        internal void FireProcessRecord()
        {
            ProcessRecord();
        }

        internal void FireStopProcessing()
        {
            StopProcessing();
        }
    }
}

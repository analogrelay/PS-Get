using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Abstractions;
using PsGet.Hosting;

namespace PsGet.Commands
{
    public abstract class PsGetCommandBase : PSCmdlet
    {
        protected internal Settings Settings { get; set; }
        protected internal ICommandInvoker Invoker { get; internal set; }
        protected internal IHostEnvironment HostEnvironment { get; internal set; }
        protected internal ISessionStore Session { get; internal set; }

        protected PsGetCommandBase()
        {
            Invoker = new PowerShellInvoker(InvokeCommand);
            HostEnvironment = new PowerShellHostEnvironment(MyInvocation);
            Session = new PowerShellSession(SessionState);
        }

        // Disable warning about obsolete overrides of non-obsolete methods because that's exactly what we want!
        protected override sealed void BeginProcessing()
        {
            BeginProcessingCore();
        }

        protected internal virtual void BeginProcessingCore() {
            Settings = new Settings(HostEnvironment.ModuleBase);
        }

        protected override sealed void ProcessRecord()
        {
            ProcessRecordCore();
        }

        protected internal virtual void ProcessRecordCore() { }

        protected override sealed void EndProcessing()
        {
            EndProcessingCore();
        }

        protected internal virtual void EndProcessingCore() { }

        protected override sealed void StopProcessing()
        {
            StopProcessingCore();
        }

        protected internal virtual void StopProcessingCore() { }
    }
}

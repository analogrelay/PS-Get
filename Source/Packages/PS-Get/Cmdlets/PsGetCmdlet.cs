using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading;
using PsGet.Communications;
using System.Diagnostics;

namespace PsGet.Cmdlets {
    public abstract class PsGetCmdlet : PSCmdlet {
#if DEBUG
        [Parameter]
        [ValidateNotNull]
        public string PipeName { get; set; }
#endif

        protected ShimManager Shim { get; set; }
        protected NuGetProxy Client { get; set; }
        protected Settings Settings { get; set; }

        protected sealed override void BeginProcessing() {
            base.BeginProcessing();

            Settings = new Settings(MyInvocation.MyCommand.Module);
            
            // Initialize Current Module if necessary
            Interlocked.CompareExchange(ref PsGetModule.Current, MyInvocation.MyCommand.Module, null);

#if DEBUG
            if (!String.IsNullOrEmpty(PipeName)) {
                Shim = new ShimManager(PipeName);
            }
            else {
#endif
                Shim = ShimManager.Global;
#if DEBUG
            }
#endif

            Trace.WriteLine(String.Format("PSGET Using Pipe: {0}", Shim.PipeName));

            // Open the client
            Client = Shim.Open(this);

            // Call Begin Processing
            BeginProcessingCore();
        }

        protected virtual void BeginProcessingCore() {
        }

        protected override void ProcessRecord() {
            try {
                InvokeService();
            }
            catch (Exception ex) {
                WriteError(new ErrorRecord(ex, "PsGet.Error", ErrorCategory.InvalidOperation, null));
            }
        }

        protected virtual void InvokeService() {
        }

        protected override void EndProcessing() {
            Trace.WriteLine("PSGET Disposing Client");
            Client.Dispose();
        }
    }
}

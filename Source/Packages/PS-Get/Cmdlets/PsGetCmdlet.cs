using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Threading;

namespace PsGet.Cmdlets {
    public abstract class PsGetCmdlet : PSCmdlet {
        protected sealed override void BeginProcessing() {
            base.BeginProcessing();

            // Initialize Current Module if necessary
            Interlocked.CompareExchange(ref PsGetModule.Current, MyInvocation.MyCommand.Module, null);

            // Call Begin Processing
            BeginProcessingCore();
        }

        protected virtual void BeginProcessingCore() {

        }
    }
}

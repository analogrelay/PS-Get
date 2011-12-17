using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Facts.TestDoubles
{
    // Test Doubles are tested via their use in unit tests
    [ExcludeFromCodeCoverage]
    public class TestCommandRuntime : ICommandRuntime
    {
        public CommandOutput Output { get; private set; }
        
        public TestCommandRuntime()
        {
            Output = new CommandOutput();
        }

        public void WriteCommandDetail(string text) { Output.WriteCommandDetail(text); }
        public void WriteDebug(string text) { Output.WriteDebug(text); }
        public void WriteError(ErrorRecord errorRecord) { Output.WriteError(errorRecord); }
        public void WriteObject(object sendToPipeline) { Output.WriteObject(sendToPipeline);  }
        public void WriteProgress(long sourceId, ProgressRecord progressRecord) { Output.WriteProgress(sourceId, progressRecord); }
        public void WriteProgress(ProgressRecord progressRecord) { Output.WriteProgress(progressRecord);  }
        public void WriteVerbose(string text) { Output.WriteVerbose(text); }
        public void WriteWarning(string text) { Output.WriteWarning(text); }
        public void WriteObject(object sendToPipeline, bool enumerateCollection) { Output.WriteObject(sendToPipeline, enumerateCollection); }

        public PSTransactionContext CurrentPSTransaction
        {
            get { throw new NotImplementedException(); }
        }

        public PSHost Host
        {
            get { throw new NotImplementedException(); }
        }

        public bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll)
        {
            throw new NotImplementedException();
        }

        public bool ShouldContinue(string query, string caption)
        {
            throw new NotImplementedException();
        }

        public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out ShouldProcessReason shouldProcessReason)
        {
            throw new NotImplementedException();
        }

        public bool ShouldProcess(string verboseDescription, string verboseWarning, string caption)
        {
            throw new NotImplementedException();
        }

        public bool ShouldProcess(string target, string action)
        {
            throw new NotImplementedException();
        }

        public bool ShouldProcess(string target)
        {
            throw new NotImplementedException();
        }

        public void ThrowTerminatingError(ErrorRecord errorRecord)
        {
            throw new NotImplementedException();
        }

        public bool TransactionAvailable()
        {
            throw new NotImplementedException();
        }
    }
}

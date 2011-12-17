using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Collections;

namespace PsGet.Facts
{
    public class CommandOutput
    {
        public IList<string> CommandDetailStream { get; private set; }
        public IList<string> DebugStream { get; private set; }
        public IList<ErrorRecord> ErrorStream { get; private set; }
        public IList<object> ObjectStream { get; private set; }
        public IList<Tuple<int?, ProgressRecord>> ProgressStream { get; private set; }
        public IList<string> VerboseStream { get; private set; }
        public IList<string> WarningStream { get; private set; }

        public CommandOutput()
        {
            CommandDetailStream = new List<string>();
            DebugStream = new List<string>();
            ErrorStream = new List<ErrorRecord>();
            ObjectStream = new List<object>();
            ProgressStream = new List<Tuple<int?, ProgressRecord>>();
            VerboseStream = new List<string>();
            WarningStream = new List<string>();
        }

        public void WriteCommandDetail(string text) { CommandDetailStream.Add(text); }
        public void WriteDebug(string text) { DebugStream.Add(text); }
        public void WriteError(ErrorRecord errorRecord) { ErrorStream.Add(errorRecord); }
        public void WriteObject(object sendToPipeline) { ObjectStream.Add(sendToPipeline); }
        public void WriteProgress(long sourceId, ProgressRecord progressRecord) { ProgressStream.Add(Tuple.Create((int?)sourceId, progressRecord)); }
        public void WriteProgress(ProgressRecord progressRecord) { ProgressStream.Add(Tuple.Create((int?)null, progressRecord)); }
        public void WriteVerbose(string text) { VerboseStream.Add(text); }
        public void WriteWarning(string text) { WarningStream.Add(text); }
        public void WriteObject(object sendToPipeline, bool enumerateCollection)
        {
            IEnumerable enumerable = sendToPipeline as IEnumerable;
            if (enumerable != null && enumerateCollection)
            {
                foreach (object obj in enumerable)
                {
                    WriteObject(obj);
                }
            }
            else
            {
                WriteObject(sendToPipeline);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Helper;
using PsGet.Converters;
using Psh = System.Management.Automation;

namespace PsGet.Communications {
    abstract class HelperClientBase : INuGetShimCallback {
        public abstract void Completed();
        public abstract void ReportProgress(Psh.ProgressRecord record);

        void INuGetShimCallback.ReportProgress(ProgressRecord record) {
            ReportProgress(record.ToActual());
        }
    }
}

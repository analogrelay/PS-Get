using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Helper.Serializables;
using System.Diagnostics;
using System.Threading;
using NuGet;
using System.ServiceModel;

namespace PsGet.Helper {
    public class Operation : IDisposable {
        private static int _nextActivityId = 0;

        private INuGetClient _client;
        private int _activityId;

        private Operation(INuGetClient client, int activityId) {
            _client = client;
            _activityId = activityId;
        }

        public static Operation Start(INuGetClient client) {
            return new Operation(client, Interlocked.Increment(ref _nextActivityId));
        }

        public void TryBindToProgressReporter(IPackageRepository repo, string activity) {
            repo.OnProgressAvailable((args) => {
                TraceUtil.TraceSend("REPORT", "{0} {1}%", args.Operation, args.PercentComplete);
                ReportProgress(new ProgressRecord(
                    activity,
                    args.Operation) {
                        CurrentOperation = args.Operation,
                        PercentComplete = args.PercentComplete,
                        RecordType = ProgressRecordType.Processing
                    });
            });
        }

        public void ReportProgress(ProgressRecord record) {
            record.ActivityId = _activityId;
            _client.ReportProgress(record);
        }

        public void ReportError(FaultException ex) {
            _client.ReportError(ex);
        }

        public void Dispose() {
            _client.Completed();
        }
    }
}

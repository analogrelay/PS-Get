using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace PsGet.Helper {
    static class SessionManager {
        private static int _counter = 0;
        private static ConcurrentDictionary<string, int> _sessions = new ConcurrentDictionary<string, int>();

        public static int GetSessionId() {
            return _sessions.GetOrAdd(OperationContext.Current.SessionId, _ => {
                int id = Interlocked.Increment(ref _counter);
                OperationContext.Current.Channel.Closed += (__, ___) => {
                    Trace.WriteLine(String.Format("[{0}] <- CLOSE", id));
                };
                return id;
            });
        }
    }
}

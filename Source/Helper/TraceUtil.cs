using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PsGet.Helper {
    static class TraceUtil {
        public static void TraceSend(string command, string message, params object[] args) {
            TraceCmd("->", command, message, args);
        }

        public static void TraceRecv(string command, string message, params object[] args) {
            TraceCmd("<-", command, message, args);
        }

        public static void TraceCmd(string direction, string command, string message, params object[] args) {
            Trace.WriteLine(String.Format("{0} {1}: {2}", direction, command, String.Format(message, args)));
        }
    }
}

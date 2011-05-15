using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace PsGet.Helper {
    public static class ProgressProviderMixin {
        public static void OnProgressAvailable(this IPackageRepository self, Action<ProgressEventArgs> act) {
            IProgressProvider prog = self as IProgressProvider;
            if (prog != null) {
                prog.ProgressAvailable += (_, args) => act(args);
            }
        }
    }
}

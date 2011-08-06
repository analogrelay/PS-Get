using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PsGet.Installer.Services {
    public static class SynchronizationContextExtensions {
        public static void Post(this SynchronizationContext self, Action act) {
            self.Post(_ => act(), null);
        }
    }
}

using System;
using System.Linq;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using NuGet;
using PsGet.Helper.Serializables;
using System.Collections.Generic;

namespace PsGet.Helper {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    class NuGetShim : INuGetShim {
        private static int _nextActivityId = 0;

        private static ManualResetEvent _shutdown = new ManualResetEvent(false);

        private INuGetClient Client {
            get {
                return OperationContext.Current.GetCallbackChannel<INuGetClient>();
            }
        }

        public void Install(string id, Version version, string source, string destination) {
            Trace.WriteLine(String.Format("[{0}] <- INSTALL: {1} (v{2}) from {3} to {4}",
                                          SessionManager.GetSessionId(),
                                          id, version, source, destination));

            using (Operation op = Operation.Start(Client)) {
                IPackageRepository repo = OpenRepository(source);
                PackageManager manager = new PackageManager(repo, destination);
                repo.OnProgressAvailable((args) => {
                    Trace.WriteLine(String.Format("[{0}] -> REPORT: {1}, {2}%", SessionManager.GetSessionId(), args.Operation, args.PercentComplete));
                    op.ReportProgress(new ProgressRecord(
                        String.Format("Installing {0} {1}", id, version),
                        "Installing") {
                            CurrentOperation = args.Operation,
                            PercentComplete = args.PercentComplete,
                            RecordType = ProgressRecordType.Processing
                        });
                });
                manager.InstallPackage(id, version);
            }

            Trace.WriteLine(String.Format("[{0}] INSTALL: Complete", SessionManager.GetSessionId()));
        }

        public ICollection<Package> GetPackages(string source, string filter) {
            IPackageRepository repo = OpenRepository(source);
            var query = repo.GetPackages()
                            .Where(p => p.Tags.Contains(" psget "));
            if(!String.IsNullOrEmpty(filter)) {
                query = query.Where(p => p.Id.Contains(filter));
            }
            return query.Select(p => new Package(p)).ToList();
        }

        private static IPackageRepository OpenRepository(string source) {
            PackageSource src = new PackageSource(source);
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(src);
            return repo;
        }

        public void Shutdown() {
            Trace.WriteLine(String.Format("[{0}] <- SHUTDOWN",
                                          SessionManager.GetSessionId()));
            _shutdown.Set();
        }

        internal static void WaitForShutdown(ServiceHost host) {
            _shutdown.WaitOne();
            host.Close();
        }
    }
}

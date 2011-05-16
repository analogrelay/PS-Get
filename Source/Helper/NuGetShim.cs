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
        private static ManualResetEvent _shutdown = new ManualResetEvent(false);

        private INuGetClient Client {
            get {
                return OperationContext.Current.GetCallbackChannel<INuGetClient>();
            }
        }

        private int SessionId {
            get { return SessionManager.GetSessionId(); }
        }

        public void Install(string id, Version version, string source, string destination) {
            TraceRecv("INSTALL", "{0} (v{1}) from {2} to {3}", id, version, source, destination);

            using (Operation op = Operation.Start(Client)) {
                IPackageRepository repo = OpenRepository(source);
                PackageManager manager = CreatePackageManager(destination, repo);
                repo.OnProgressAvailable((args) => {
                    TraceSend("REPORT", "{1} {2}%", args.Operation, args.PercentComplete);
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

            TraceSend("INSTALL", "Complete");
        }

        public ICollection<Package> GetPackages(string source, string filter, bool allPackages) {
            TraceRecv("QUERY", "\'{0}\' from {1}", filter, source);

            IPackageRepository repo = OpenRepository(source);
            var query = repo.GetPackages()
                            .Where(p => p.Tags.Contains(" psget "));
            if(!String.IsNullOrEmpty(filter)) {
                query = query.Where(p => p.Id.Contains(filter));
            }
            IEnumerable<Package> localList = query.ToList().Select(p => new Package(p));
            if(!allPackages) {
                // Filter out the max version
                localList = localList.GroupBy(p => p.Id)
                                     .Select(grp => grp.OrderByDescending(p => p.Version)
                                                       .First());
            }
            return localList.ToList();
        }

        private static PackageManager CreatePackageManager(string destination, IPackageRepository repo) {
            return new PackageManager(repo, new DefaultPackagePathResolver(destination, useSideBySidePaths: false), new PhysicalFileSystem(destination));
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

        private void TraceSend(string command, string message, params object[] args) {
            TraceCmd("->", command, message, args);
        }

        private void TraceRecv(string command, string message, params object[] args) {
            TraceCmd("<-", command, message, args);
        }

        private void TraceCmd(string direction, string command, string message, params object[] args) {
            Trace.WriteLine(String.Format("[{0}] {1} {2}: {3}", SessionId, direction, command, String.Format(message, args)));
        }
    }
}

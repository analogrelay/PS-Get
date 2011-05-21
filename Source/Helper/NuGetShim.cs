using System;
using System.Linq;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using NuGet;
using PsGet.Helper.Serializables;
using System.Collections.Generic;
using System.IO;

namespace PsGet.Helper {
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    class NuGetShim : INuGetShim {
        private static ManualResetEvent _shutdown = new ManualResetEvent(false);

        private INuGetClient Client {
            get {
                return OperationContext.Current.GetCallbackChannel<INuGetClient>();
            }
        }

        public void Install(string id, Version version, string source, string destination) {
            Thread.CurrentThread.Name = String.Format("WCF Session {0}", SessionManager.GetSessionId());
            TraceUtil.TraceRecv("INSTALL", "{0} (v{1})", id, version);

            RunProgressOperation(destination, source, (op, repo, manager) => {
                op.TryBindToProgressReporter(repo, String.Format("Installing {0}", id));
                manager.InstallPackage(id, version);
            });
            TraceUtil.TraceSend("INSTALL", "Complete");
        }

        public void Update(string id, Version version, bool updateDependencies, string source, string destination) {
            Thread.CurrentThread.Name = String.Format("WCF Session {0}", SessionManager.GetSessionId());
            TraceUtil.TraceRecv("UPDATE", "{0} (v{1})", id, version);

            RunProgressOperation(destination, source, (op, repo, manager) => {
                op.TryBindToProgressReporter(repo, String.Format("Updating {0}", id));

                IPackage installed = manager.LocalRepository.FindPackage(id);
                if (installed != null && installed.Version != version) {
                    manager.UninstallPackage(installed);
                }
                if (installed == null || installed.Version != version) {
                    manager.InstallPackage(id, version);
                }
            });

            TraceUtil.TraceSend("UPDATE", "Complete");
        }

        public void Remove(string id, string source, string destination) {
            Thread.CurrentThread.Name = String.Format("WCF Session {0}", SessionManager.GetSessionId());
            TraceUtil.TraceRecv("REMOVE", "{0}", id);

            RunProgressOperation(destination, source, (op, repo, manager) => {
                op.TryBindToProgressReporter(repo, String.Format("Removing {0}", id));
                manager.UninstallPackage(id);
            });
        }

        public ICollection<Package> GetPackages(string source, string filter, bool allPackages) {
            Thread.CurrentThread.Name = String.Format("WCF Session {0}", SessionManager.GetSessionId());
            TraceUtil.TraceRecv("QUERY", "\'{0}\' from {1}", filter, source);

            IPackageRepository repo = OpenRepository(source);
            var query = repo.GetPackages()
                            .Where(p => p.Tags.Contains(" psget "));
            if (!String.IsNullOrEmpty(filter)) {
                query = query.Where(p => p.Id.Contains(filter));
            }
            IEnumerable<Package> localList = query.ToList().Select(p => new Package(p));
            if (!allPackages) {
                // Filter out the max version
                localList = localList.GroupBy(p => p.Id)
                                     .Select(grp => grp.OrderByDescending(p => p.Version)
                                                       .First());
            }
            return localList.ToList();
        }

        public void Pack(PackageSpec package, string destination) {
            PackageBuilder builder = package.ToNuGet();

            using (FileStream strm = File.Open(destination, FileMode.Create, FileAccess.ReadWrite, FileShare.None)) {
                builder.Save(strm);
            }
        }

        private void RunProgressOperation(string destination, string source, Action<Operation, IPackageRepository, PackageManager> action) {
            using (Operation op = Operation.Start(Client)) {
                try {
                    IPackageRepository repo = OpenRepository(source);
                 
                    PackageManager manager = CreatePackageManager(destination, repo);
                    action(op, repo, manager);
                }
                catch (Exception ex) {
                    op.ReportError(new FaultException(new FaultReason(ex.Message), new FaultCode(ex.GetType().Name)));
                }
            }
        }

        private static PackageManager CreatePackageManager(string destination, IPackageRepository repo) {
            return new PackageManager(repo, new DefaultPackagePathResolver(destination, useSideBySidePaths: false), new PhysicalFileSystem(destination));
        }

        private static IPackageRepository OpenRepository(string source) {
            PackageSource src = new PackageSource(source);
            IPackageRepository repo = PackageRepositoryFactory.Default.CreateRepository(src);
            return repo;
        }
    }
}

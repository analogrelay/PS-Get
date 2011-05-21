using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Helper;
using System.ServiceModel;
using System.Threading;
using Psh = System.Management.Automation;
using System.Diagnostics;

namespace PsGet.Communications {
    [CallbackBehavior(IncludeExceptionDetailInFaults=true)]
    public class NuGetProxy : HelperClientBase, IDisposable {
        public static readonly TimeSpan OpenTimeout = TimeSpan.FromSeconds(1);

        private EventQueue _queue = null;
        private PSCmdlet _owner;
        private ShimManager _shimManager;
        private string _pipeName;
        private NuGetShimClient _client;
        
        internal NuGetProxy(string pipeName) : this(pipeName, null, null) {
        }

        internal NuGetProxy(string pipeName, ShimManager shimManager, PSCmdlet owner) {
            _pipeName = pipeName;
            _shimManager = shimManager;
            _owner = owner;
        }

        public void Dispose() {
            _client.Close();
            Trace.WriteLine("CLIENT Closed");
            if (_shimManager != null) {
                _shimManager.Release();
            }
        }

        public void Install(string id, Version version, string source, string destination) {
            EnsureClient();
            using (_queue = new EventQueue()) {
                _client.Install(id, version, source, destination);
                _queue.Run();
            }
        }
        
        public void Update(string id, Version version, bool updateDependencies, string source, string destination) {
            EnsureClient();
            using (_queue = new EventQueue()) {
                _client.Update(id, version, updateDependencies, source, destination);
                _queue.Run();
            }
        }

        public void Remove(string id, string source, string destination) {
            EnsureClient();
            using (_queue = new EventQueue()) {
                _client.Remove(id, source, destination);
                _queue.Run();
            }
        }

        public void Pack(PackageSpec spec, string destination) {
            EnsureClient();
            _client.Pack(spec, destination);
        }

        public ICollection<Package> GetPackages(string source, string filter, bool allVersions) {
            EnsureClient();
            return _client.GetPackages(source, filter, allVersions);
        }

        private void EnsureClient() {
            if (_client == null) {
                _client = new NuGetShimClient(
                    new InstanceContext(this),
                    new NetNamedPipeBinding() { OpenTimeout = OpenTimeout },
                    new EndpointAddress(String.Format("net.pipe://localhost/PsGetHelper/{0}", _pipeName)));
                _client.Open();
                Trace.WriteLine(String.Format("CLIENT Channel Open: {0}", _client.Endpoint.Address.ToString()));
            }
        }

        public override void ReportProgress(Psh.ProgressRecord record) {
            _queue.Enqueue(() => {
                if (_owner != null) {
                    _owner.WriteProgress(record);
                }
            });
        }

        public override void ReportError(FaultException ex) {
            _queue.Enqueue(() => {
                if (_owner != null) {
                    _owner.WriteError(new ErrorRecord(ex, "PsGet.Error", ErrorCategory.InvalidOperation, null));
                }
            });
        }

        public override void Completed() {
            _queue.Enqueue(() => {
                _queue.Close();
            });
        }
    }
}

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
    public class NuGetShim : HelperClientBase, IDisposable {
        public static readonly TimeSpan OpenTimeout = TimeSpan.FromSeconds(1);

        private EventQueue _queue = null;
        private PSCmdlet _owner;
        private ShimManager _shimManager;
        private string _pipeName;
        private NuGetShimClient _client;
        private AutoResetEvent _operationCompleted = new AutoResetEvent(false);

        internal NuGetShim(string pipeName) : this(pipeName, null, null) {
        }

        internal NuGetShim(string pipeName, ShimManager shimManager, PSCmdlet owner) {
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
            _queue = new EventQueue();
            _client.Install(id, version, source, destination);
            _queue.Run();
        }

        public ICollection<Package> GetPackages(string source, string filter, bool allVersions) {
            EnsureClient();
            return _client.GetPackages(source, filter, allVersions);
        }

        public void Shutdown() {
            EnsureClient();
            _client.Shutdown();
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
                if (record.RecordType == Psh.ProgressRecordType.Completed) {
                    _operationCompleted.Set();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Helper;
using System.ServiceModel;
using System.Threading;
using Psh = System.Management.Automation;

namespace PsGet.Communications {
    [CallbackBehavior(IncludeExceptionDetailInFaults=true)]
    class NuGetShim : HelperClientBase, IDisposable, INuGetShim {
        private EventQueue _queue = null;
        private PSCmdlet _owner;
        private ShimManager _shimManager;
        private string _pipeName;
        private NuGetShimClient _client;
        private AutoResetEvent _operationCompleted = new AutoResetEvent(false);

        internal NuGetShim(string pipeName, ShimManager shimManager) : this(pipeName, shimManager, null) {
        }

        internal NuGetShim(string pipeName, ShimManager shimManager, PSCmdlet owner) {
            _pipeName = pipeName;
            _shimManager = shimManager;
            _owner = owner;
        }

        public void Dispose() {
            _client.Close();
            _shimManager.Release();
        }

        public void Install(string id, Version version, string source, string destination) {
            EnsureClient();
            _queue = new EventQueue();
            _client.Install(id, version, source, destination);
            _queue.Run();
        }

        public void Shutdown() {
            EnsureClient();
            _client.Shutdown();
        }

        private void EnsureClient() {
            _client = new NuGetShimClient(
                new InstanceContext(this),
                new NetNamedPipeBinding(),
                new EndpointAddress(String.Format("net.pipe://localhost/PsGetHelper/{0}", _pipeName)));
            _client.Open();
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

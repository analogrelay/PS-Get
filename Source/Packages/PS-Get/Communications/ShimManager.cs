using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace PsGet.Communications {
    class ShimManager {
        private const int ExpireTime = 500;

        private static string _helperLocation = null;
        public static string HelperLocation {
            get {
                if(_helperLocation == null) {
                    _helperLocation = new Settings(PsGetModule.Current).HelperPath;
                }
                return _helperLocation;
            }
        }

        public static readonly ShimManager Global = new ShimManager();

        private Timer _timer;
        private int _refCount = 0;
        private string _pipeName;
        private bool _managed;
        private Process _shimProcess;
        private object _lock = new object();

        private PSHost Host {
            get {
                if (Runspace.DefaultRunspace != null) {
                    return Runspace.DefaultRunspace.SessionStateProxy.PSVariable.GetValue("Host") as PSHost;
                }
                return null;
            }
        }

        private ShimManager() {
            // Setup global shim manager
            _pipeName = String.Format("{0}{1}", Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            _managed = true;
            _timer = new Timer(_ => ProcessExpired(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public ShimManager(string pipeName) {
            // Setup shim manager for non-owned shim
            _pipeName = pipeName;
            _managed = false;
        }

        private string FindHelper() {
            throw new NotImplementedException();
        }

        public NuGetShim Open(PSCmdlet owner) {
            lock (_lock) {
                if (_managed && _shimProcess == null) {
                    StartShimProcess();
                }

                // Add a reference and shut down the timer
                _refCount++;
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            // Return the shim
            return new NuGetShim(_pipeName, this, owner);
        }

        public void Release() {
            lock (_lock) {
                if (--_refCount == 0) {
                    // Start the timer to shut down the process
                    _timer.Change(ExpireTime, Timeout.Infinite);
                }
            }
        }

        private void ProcessExpired() {
            if (_managed) {
                lock (_lock) {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);

                    // Verify there's still no shim clients
                    if (_refCount == 0) {
                        StopShimProcess();
                    }
                }
            }
        }

        private void StartShimProcess() {
            ProcessStartInfo psi = new ProcessStartInfo() {
                FileName = HelperLocation,
                Arguments = _pipeName,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            _shimProcess = Process.Start(psi);
        }

        private void StopShimProcess() {
            using (NuGetShim shim = new NuGetShim(_pipeName, this)) {
                shim.Shutdown();
            }
        }
    }
}

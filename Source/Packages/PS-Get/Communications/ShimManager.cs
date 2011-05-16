using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

namespace PsGet.Communications {
    public class ShimManager {
        private const int ExpireTime = 30 /*min*/ * 60 /* sec */ * 1000 /* ms */;

        private static string _helperLocation = null;
        public static string HelperLocation {
            get {
                if (_helperLocation == null) {
                    _helperLocation = new Settings(PsGetModule.Current).HelperPath;
                }
                return _helperLocation;
            }
        }

        private static Semaphore _startSemaphore;
        public static readonly ShimManager Global = new ShimManager();

        public string PipeName { get; private set; }

        private Timer _timer;
        private int _refCount = 0;
        private bool _managed;
        private Process _shimProcess;
        private object _lock = new object();

        private ShimManager() {
            // Setup global shim manager
            PipeName = String.Format("{0}{1}", Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            _managed = true;
            _timer = new Timer(_ => ProcessExpired(), null, Timeout.Infinite, Timeout.Infinite);
            _startSemaphore = new Semaphore(0, 1, String.Format("psget.{0}", PipeName));
            
            AppDomain.CurrentDomain.ProcessExit += (_, __) => {
                StopShimProcess();
            };
        }

        public ShimManager(string pipeName) {
            // Setup shim manager for non-owned shim
            PipeName = pipeName;
            _managed = false;
        }

        public NuGetShim Open(PSCmdlet owner) {
            lock (_lock) {
                if (_managed && _shimProcess == null) {
                    StartShimProcess();
                }

                // Add a reference
                _refCount++;
                Trace.WriteLine(String.Format("SHIM Acquire - {0} references are open", _refCount));
            }
            // Return the shim
            return new NuGetShim(PipeName, this, owner);
        }

        public void Release() {
            lock (_lock) {
                if (--_refCount == 0 && _timer != null) {
                    Trace.WriteLine(String.Format("SHIM Final Release - Timer: {0}ms", ExpireTime));
                    // Start the timer to shut down the process
                    _timer.Change(ExpireTime, Timeout.Infinite);
                }
                else {
                    Trace.WriteLine(String.Format("SHIM Release - {0} references remain", _refCount));
                }
            }
        }

        private void ProcessExpired() {
            if (_managed) {
                lock (_lock) {
                    Trace.WriteLine("SHIM Expired");
                    // Verify there's still no shim clients
                    if (_refCount == 0) {
                        Trace.WriteLine("SHIM Unused, Terminating");
                        _timer.Change(Timeout.Infinite, Timeout.Infinite);
                        StopShimProcess();
                    }
                }
            }
        }

        private void StartShimProcess() {
            ProcessStartInfo psi = new ProcessStartInfo() {
                FileName = HelperLocation,
                Arguments = PipeName,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Now start the shim
            _shimProcess = Process.Start(psi);
            Trace.WriteLine(String.Format("SHIM Starting, PID:{0}", _shimProcess.Id));
            
            // Wait for the shim to release the semaphore
            _startSemaphore.WaitOne();

            Trace.WriteLine("SHIM Started, Semaphore Released");
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void StopShimProcess() {
            using (NuGetShim shim = new NuGetShim(PipeName)) {
                Trace.WriteLine("SHIM Shutdown");
                shim.Shutdown();

                // Release the semaphore to reset everything
                _startSemaphore.Release();
            }
        }
    }
}

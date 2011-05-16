using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace PsGet.Helper {
    public static class Program {
        public static void Main(string[] args) {
            //while (!Debugger.IsAttached) { }

            if (args.Length < 1 || args.Length > 2) {
                Console.WriteLine("Usage: PsGetHelper [pipename] [-m]");
                Console.WriteLine("Should ONLY be used from within PS-Get package");
                return;
            }
            bool metadata = false;
            if (args.Length == 2 && String.Equals(args[1], "-m", StringComparison.OrdinalIgnoreCase)) {
                metadata = true;
            }

            string hostUri = String.Format("net.pipe://localhost/PsGetHelper/{0}", args[0]);
            string metadataUri = "net.pipe://localhost/PsGetHelper/mex";
            Semaphore startSem = new Semaphore(0, 1, String.Format("psget.{0}", args[0]));
            
            ServiceHost host = new ServiceHost(typeof(NuGetShim));
            if (metadata) {
                host.Description.Behaviors.Add(new ServiceMetadataBehavior());
                host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                        MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                                        metadataUri);
            }
            host.AddServiceEndpoint(typeof(INuGetShim), new NetNamedPipeBinding(), hostUri);
            
            host.Open();
            startSem.Release();

            Trace.WriteLine("HELPER Released Mutex");
            if (metadata) {
                Trace.WriteLine(String.Format("HELPER Hosting Metadata Endpoint at {0}", metadataUri));
            }
            Trace.WriteLine(String.Format("HELPER Hosting NuGetShim at {0}", hostUri));
            if (metadata) {
                Console.WriteLine("Press Enter to Shutdown the Helper");
                Console.ReadLine();
            } else {
                Trace.WriteLine("HELPER Waiting For Shutdown from Client");
                NuGetShim.WaitForShutdown(host);
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace PsGet.Helper {
    public static class Program {
        public static void Main(string[] args) {
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
            
            ServiceHost host = new ServiceHost(typeof(NuGetShim));
            if (metadata) {
                host.Description.Behaviors.Add(new ServiceMetadataBehavior());
                host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName,
                                        MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                                        metadataUri);
            }
            host.AddServiceEndpoint(typeof(INuGetShim), new NetNamedPipeBinding(), hostUri);
            host.Open();
            if (metadata) {
                Trace.WriteLine(String.Format("Hosting Metadata Endpoint at {0}", metadataUri));
            }
            Trace.WriteLine(String.Format("Hosting NuGetShim at {0}", hostUri));
            if (metadata) {
                Console.WriteLine("Press Enter to Shutdown the Helper");
                Console.ReadLine();
            } else {
                Trace.WriteLine("Now Waiting For Shutdown Command from Client");
                NuGetShim.WaitForShutdown(host);
            }
        }
    }
}

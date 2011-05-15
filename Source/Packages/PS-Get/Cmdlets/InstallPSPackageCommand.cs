using System;
using System.Diagnostics;
using System.Management.Automation;
using PsGet.Communications;

namespace PsGet.Cmdlets {
    [Cmdlet(VerbsLifecycle.Install, "PSPackage")]
    public class InstallPSPackageCommand : PsGetCmdlet {
        private ShimManager _manager;
        private Settings _settings;
        private NuGetShim _client;

        [Parameter(Position = 0, Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Id { get; set; }

        [Parameter(Position = 1)]
        [ValidateNotNull]
        public Version Version { get; set; }

        [Parameter(Position = 2)]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter(Position = 3)]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

#if DEBUG
        [Parameter(Position = 4)]
        [ValidateNotNull]
        public string PipeName { get; set; }
#endif

        [Parameter]
        public SwitchParameter IgnoreDependencies { get; set; }

        protected override void BeginProcessingCore() {
            _settings = new Settings(MyInvocation.MyCommand.Module);
#if DEBUG
            if (!String.IsNullOrEmpty(PipeName)) {
                _manager = new ShimManager(PipeName);
            }
            else {
#endif
                _manager = ShimManager.Global;
#if DEBUG
            }
#endif

            // Open the client
            _client = _manager.Open(this);

            if (String.IsNullOrEmpty(Source)) {
                Source = _settings.DefaultSource;
            }
            
            if(String.IsNullOrEmpty(Destination)) {
                Destination = _settings.InstallationRoot;
            }
        }

        protected override void ProcessRecord() {
            WriteDebug(String.Format("Invoking Install(\"{0}\", \"{1}\", \"{2}\", \"{3}\")", Id, Version, Source, Destination));
            try {
                _client.Install(Id, Version, Source, Destination);
            }
            catch (Exception ex) {
                WriteError(new ErrorRecord(ex, "PsGet.Error", ErrorCategory.InvalidOperation, null));
                return;
            }
            WriteDebug("Successfully Executed");
        }

        protected override void EndProcessing() {
            _client.Dispose();
        }

        private string GetPipeName() {
            string name = String.Format("pid{0}", Process.GetCurrentProcess().Id);
#if DEBUG
            name = PipeName ?? name;
#endif
            return name;
        }
    }
}

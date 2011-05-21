using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics;
using System.Collections;
using System.IO;
using PsGet.Utils;

namespace PsGet.Cmdlets {
    public abstract class PsGetModuleCmdlet : PsGetCmdlet {
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByName")]
        [ValidateNotNullOrEmpty]
        public string ModuleName { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByModule")]
        [ValidateNotNull]
        public PSModuleInfo Module { get; set; }

        protected IDictionary<string, object> GetManifest(PSModuleInfo module) {
            if (module.ModuleType != ModuleType.Manifest) {
                return new Dictionary<string, object>();
            }

            // Find the requested manifest
            ProviderInfo provider;
            string path = GetResolvedProviderPathFromPSPath(module.Path, out provider).FirstOrDefault();
            if (path == null) {
                throw new ArgumentException(String.Format("Invalid Path: {0}", ModuleName));
            }
            string manifestPath = Path.Combine(Path.GetDirectoryName(path), module.Name + ".psd1");
            if (!File.Exists(manifestPath)) {
                throw new FileNotFoundException(String.Format("Module Manifest not found at {0}", ModuleName));
            }
            
            // Set up the ScriptBlock and check it meets the restricted language criteria BEFORE executing
            //  (to protect the user and the environment)
            ScriptBlock manifestBlock = ScriptBlock.Create(File.ReadAllText(path));
            RestrictedLanguageModeChecker.CheckModuleManifest(manifestBlock);

            // Capture current PSScriptRoot value
            object currentValue = SessionState.PSVariable.GetValue("PSScriptRoot");

            // Set PSScriptRoot to the parent directory of the manifest
            SessionState.PSVariable.Set("PSScriptRoot", System.IO.Path.GetDirectoryName(path));

            // Invoke.Select the manifest to gather the results
            PSObject result = InvokeCommand.InvokeScript(SessionState, manifestBlock).FirstOrDefault();

            // Restore old PSScriptRoot value
            SessionState.PSVariable.Set("PSScriptRoot", currentValue);

            // Convert the results
            Hashtable manifest = null;
            if (result == null || ((manifest = result.ImmediateBaseObject as Hashtable) == null)) {
                throw new InvalidDataException("Path does not contain a valid PS Module Manifest");
            }
            Debug.Assert(manifest != null);

            // Make it in to a Dictionar<String,Object> for convenience, we know the keys are strings
            return manifest.Cast<DictionaryEntry>().ToDictionary(e => (string)e.Key, e => e.Value);
        }

        protected override void BasePrepare() {
            if (String.Equals(ParameterSetName, "ByName")) {
                Module = FindModule(ModuleName);
                if (Module == null) {
                    WriteError(new ErrorRecord(new ItemNotFoundException(String.Format("Module {0} not found", ModuleName)), "PsGet.Error", ErrorCategory.ObjectNotFound, ModuleName));
                }
            }
        } 

        private PSModuleInfo FindModule(string name) {
            return SessionState.InvokeCommand
                               .InvokeScript("Get-Module " + name)
                               .FirstOrDefault()
                               .As<PSModuleInfo>();
        }
    }
}

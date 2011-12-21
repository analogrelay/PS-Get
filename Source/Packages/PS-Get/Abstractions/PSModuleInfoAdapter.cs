using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.PowerShell.Commands;
using System.Collections;

namespace PsGet.Abstractions
{
    // Can't do unit tests here because of the dependency on PowerShell
    [ExcludeFromCodeCoverage]
    internal class PSModuleInfoAdapter : IModuleMetadata
    {
        internal PSModuleInfo Module { get; private set; }

        public Version Version { get { return Module.Version; } }
        public string Name { get { return Module.Name; } }
        public string Author { get; private set; }
        public string Description { get { return Module.Description; } }
        public string ModuleBase { get { return Module.ModuleBase; } }
        
        public PSModuleInfoAdapter(PSModuleInfo module)
        {
            Module = module;

            if (Module.ModuleType == ModuleType.Manifest)
            {
                LoadManifest(Module.Path);
            }
            else
            {
                Author = Environment.UserName;
            }
        }

        private void LoadManifest(string path)
        {
            // Read the manifest
            string manifest = File.ReadAllText(path);
            ScriptBlock block = ScriptBlock.Create(manifest);
            
            // Create a Parser
            Type parserType = typeof(PowerShell).Assembly.GetType("System.Management.Automation.Parser");
            object parser = Activator.CreateInstance(parserType);

            // Get the restricted language mode checker
            Type checker = typeof(PowerShell).Assembly
                                             .GetType("System.Management.Automation.RestrictedLanguageModeChecker");
            

            // Get the allowed commands
            object allowedCommands = typeof(ModuleCmdletBase).GetField("PermittedCmdlets", BindingFlags.NonPublic | BindingFlags.Static)
                                                             .GetValue(null);

            // Run the restricted language mode checker from PowerShell
            checker.GetMethod("Check", BindingFlags.Public | BindingFlags.Static)
                   .Invoke(null, new[] { parser, block, allowedCommands, /* moduleManifest */ true });

            // Now that we've checked the code, we can run it
            Hashtable manifestData = block.Invoke().Select(o => o.ImmediateBaseObject as Hashtable).FirstOrDefault();
            if (manifestData != null)
            {
                Author = TryGet<string>(manifestData, "Author");
            }
        }

        private T TryGet<T>(Hashtable manifestData, string key)
        {
            if (manifestData.ContainsKey(key))
            {
                return (T)manifestData[key];
            }
            return default(T);
        }
    }
}

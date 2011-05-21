using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;

namespace PsGet.Utils {
    public static class RestrictedLanguageModeChecker {
        private static Type ParserType = typeof(PowerShell).Assembly.GetType("System.Management.Automation.Parser");
        private static Type RLMCType = typeof(PowerShell).Assembly.GetType("System.Management.Automation.RestrictedLanguageModeChecker");
        private static MethodInfo CheckMethod = RLMCType.GetMethod("Check", BindingFlags.Static, Type.DefaultBinder, new[] { ParserType, typeof(ScriptBlock), typeof(IEnumerable<string>), typeof(bool) }, new ParameterModifier[0]);

        private static IList<string> AllowedCmdlets = new List<string>() {
            "Import-LocalizedData",
            "ConvertFrom-StringData",
            "Write-Host",
            "Out-Host",
            "Join-Path"
        };

        public static void CheckModuleManifest(ScriptBlock block) {
            if (ParserType == null) {
                throw new InvalidOperationException("Unable to check Module Manifest, could not find Parser Type");
            }
            object parser = Activator.CreateInstance(ParserType);
            if (parser == null) {
                throw new InvalidOperationException("Unable to check Module Manifest, could not create instance of Parser");
            }

            if (CheckMethod == null) {
                throw new InvalidOperationException("Unable to check Module Manifest, could not find RestrictedLanguageModeChecker.Check Method");
            }

            try {
                CheckMethod.Invoke(null, new[] { parser, block, AllowedCmdlets, true });
            }
            catch (TargetInvocationException tex) {
                throw tex.InnerException;
            }
        }
    }
}

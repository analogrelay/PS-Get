using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Hosting
{
    // Can't do unit tests here because of the dependency on PowerShell
    [ExcludeFromCodeCoverage]
    internal class PowerShellInvoker : ICommandInvoker
    {
        private PSCmdlet _cmdlet;

        public PowerShellInvoker(PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public void InvokeScript(string script)
        {
            var ret = _cmdlet.InvokeCommand.InvokeScript(script);
            Debug.Assert(!ret.Any());
        }

        public IEnumerable<T> InvokeScript<T>(string script)
        {
            return InvokeScript<T>(script, o => (T)o);
        }

        public IEnumerable<T> InvokeScript<T>(string script, Func<object, T> converter)
        {
            return _cmdlet.InvokeCommand
                          .InvokeScript(script)
                          .Select(o => o.ImmediateBaseObject)
                          .Select(converter);
        }
    }
}

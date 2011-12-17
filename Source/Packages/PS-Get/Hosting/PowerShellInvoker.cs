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
        private CommandInvocationIntrinsics _invocationIntrinsics;

        public PowerShellInvoker(CommandInvocationIntrinsics invocationIntrinsics)
        {
            _invocationIntrinsics = invocationIntrinsics;
        }

        public void InvokeScript(string script)
        {
            var ret = _invocationIntrinsics.InvokeScript(script);
            Debug.Assert(!ret.Any());
        }

        public IEnumerable<T> InvokeScript<T>(string script)
        {
            return InvokeScript<T>(script, o => (T)o);
        }

        public IEnumerable<T> InvokeScript<T>(string script, Func<object, T> converter)
        {
            return _invocationIntrinsics.InvokeScript(script)
                                        .Select(o => o.ImmediateBaseObject)
                                        .Select(converter);
        }
    }
}

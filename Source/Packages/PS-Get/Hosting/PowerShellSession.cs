using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Abstractions;

namespace PsGet.Hosting
{
    internal class PowerShellSession : ISessionStore
    {
        private PSCmdlet _cmdlet;

        public PowerShellSession(PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public object Get(string variableName)
        {
            return _cmdlet.SessionState.PSVariable.GetValue(variableName);
        }

        public void Set(string variableName, object value)
        {
            _cmdlet.SessionState.PSVariable.Set(variableName, value);
        }
    }
}

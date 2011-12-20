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
        private SessionState _sessionState;

        public PowerShellSession(SessionState sessionState)
        {
            _sessionState = sessionState;
        }

        public object Get(string variableName)
        {
            return _sessionState.PSVariable.GetValue(variableName);
        }

        public void Set(string variableName, object value)
        {
            _sessionState.PSVariable.Set(variableName, value);
        }
    }
}

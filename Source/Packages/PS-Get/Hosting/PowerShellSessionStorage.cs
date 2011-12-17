using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsGet.Hosting
{
    internal class PowerShellSessionStorage : ISessionStorage
    {
        private System.Management.Automation.SessionState SessionState;

        public PowerShellSessionStorage(System.Management.Automation.SessionState SessionState)
        {
            // TODO: Complete member initialization
            this.SessionState = SessionState;
        }
    }
}

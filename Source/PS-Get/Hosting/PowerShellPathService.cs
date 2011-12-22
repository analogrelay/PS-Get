using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace PsGet.Hosting
{
    internal class PowerShellPathService : IPathService
    {
        private PSCmdlet _cmdlet;

        public PowerShellPathService(PSCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public string CurrentPath
        {
            get { return _cmdlet.SessionState.Path.CurrentFileSystemLocation.Path; }
        }

        public string ResolvePath(string relative)
        {
            string _;
            if (_cmdlet.SessionState.Path.IsPSAbsolute(relative, out _))
            {
                return relative;
            }
            return _cmdlet.SessionState.Path.Combine(
                _cmdlet.SessionState.Path.CurrentFileSystemLocation.Path,
                relative);
        }
    }
}

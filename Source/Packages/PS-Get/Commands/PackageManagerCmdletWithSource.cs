using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Cmdlets;
using System.Management.Automation;

namespace PsGet.Commands
{
    public abstract class PackageManagerCmdletWithSource : PackageManagerCmdlet
    {
        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Source { get; set; }

        [Parameter]
        [ValidateNotNullOrEmpty]
        public string Destination { get; set; }

        protected internal override void BeginProcessingCore()
        {
            base.BeginProcessingCore();
            if (String.IsNullOrEmpty(Destination))
            {
                Destination = Settings.InstallationRoot;
            }
            WriteDebug(String.Concat("Using Source: ", Source ?? "(Default)"));
            WriteDebug(String.Concat("Installing To: ", Destination));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;
using PsGet.Services;
using System.Management.Automation;
using PsGet.Cmdlets;

namespace PsGet.Commands
{
    public abstract class PackageSourceCmdlet : PackageManagerCmdlet
    {
        [Parameter]
        public PackageSourceScope? Scope { get; set; }
    }
}

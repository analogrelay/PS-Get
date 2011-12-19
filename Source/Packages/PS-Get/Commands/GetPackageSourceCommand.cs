using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;
using System.Management.Automation;
using PsGet.Services;

namespace PsGet.Commands
{
    [Cmdlet(VerbsCommon.Get, "PackageSource")]
    public class GetPackageSourceCommand : CommandBase
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Position = 1)]
        public PackageSourceScope Scope { get; set; }
    }
}

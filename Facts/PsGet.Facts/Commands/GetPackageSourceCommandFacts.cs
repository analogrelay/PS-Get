using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using System.Management.Automation;

namespace PsGet.Facts.Commands
{
    public class GetPackageSourceCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(GetPackageSourceCommand), VerbsCommon.Get, "PackageSource");
        }

        [Fact]
        public void VerifyNameParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageSourceCommand().Name,
                new ParameterAttribute()
                {
                    Position = 0,
                    ValueFromPipeline = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyScopeParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageSourceCommand().Scope,
                new ParameterAttribute()
                {
                    Position = 1
                });
        }
    }
}

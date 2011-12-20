using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using Moq;

namespace PsGet.Facts.Commands
{
    public class PackageSourceCmdletFacts
    {
        [Fact]
        public void VerifyScopeParameter()
        {
            CmdletAssert.IsParameter(
                () => new Mock<PackageSourceManagementCommand>().Object.Scope);
        }
    }
}
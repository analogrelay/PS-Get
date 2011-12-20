using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Commands;
using System.Management.Automation;
using Xunit;
using NuGet;
using Moq;

namespace PsGet.Facts.Commands
{
    public class RemovePackageCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(RemovePackageCommand), VerbsCommon.Remove, "Package");
        }

        [Fact]
        public void VerifyIdParameter()
        {
            CmdletAssert.IsParameter(
                () => new RemovePackageCommand().Id,
                new ParameterAttribute()
                {
                    Position = 0,
                    Mandatory = true,
                    ValueFromPipelineByPropertyName = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyForceParameter()
        {
            CmdletAssert.IsParameter(() => new RemovePackageCommand().Force);
        }

        [Fact]
        public void VerifyRemoveDependenciesParameter()
        {
            CmdletAssert.IsParameter(() => new RemovePackageCommand().RemoveDependencies);
        }

        [Fact]
        public void WhenIdSpecified_PackageIsUninstalledIfNothingDependsOnItButDependenciesOfPackageAreNotRemoved()
        {
            // Arrange
            RemovePackageCommand cmd = new RemovePackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.UninstallPackage("Foo", null, false, false));
        }

        [Fact]
        public void WhenIdAndForceSpecified_PackageIsUninstalledEvenIfThereAreDependenciesButDependenciesOfPackageAreNotRemoved()
        {
            // Arrange
            RemovePackageCommand cmd = new RemovePackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";
            cmd.Force = SwitchParameter.Present;

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.UninstallPackage("Foo", null, true, false));
        }

        [Fact]
        public void WhenIdAndRemoveDependenciesSpecified_PackageIsUninstalledIfThereAreNoDependenciesAndDependenciesOfPackageAreRemoved()
        {
            // Arrange
            RemovePackageCommand cmd = new RemovePackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";
            cmd.RemoveDependencies = SwitchParameter.Present;

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.UninstallPackage("Foo", null, false, true));
        }
    }
}

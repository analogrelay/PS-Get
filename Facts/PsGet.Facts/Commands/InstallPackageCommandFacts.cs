using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using System.Management.Automation;
using NuGet;
using Moq;
using PsGet.Facts.TestDoubles;

namespace PsGet.Facts.Commands
{
    public abstract class InstallPackageCommandFactsBase<T> where T : InstallPackageCommand, new()
    {
        public abstract string Verb { get; }

        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(
                typeof(T),
                Verb,
                "Package");
        }

        [Fact]
        public void VerifyIdParameter()
        {
            CmdletAssert.IsParameter(
                () => new T().Id,
                new ParameterAttribute()
                {
                    Position = 0,
                    Mandatory = true,
                    ValueFromPipelineByPropertyName = true
                });
        }

        [Fact]
        public void VerifyVersionParameter()
        {
            CmdletAssert.IsParameter(
                () => new T().Version,
                new ParameterAttribute()
                {
                    Position = 1
                }, new ValidateNotNullAttribute());
        }

        [Fact]
        public void VerifySourceParameter()
        {
            CmdletAssert.IsParameter(
                () => new T().Source,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyDestinationParameter()
        {
            CmdletAssert.IsParameter(
                () => new T().Destination,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyIgnoreDependenciesParameter()
        {
            CmdletAssert.IsParameter(
                () => new T().IgnoreDependencies);
        }
    }

    public class InstallPackageCommandFacts : InstallPackageCommandFactsBase<InstallPackageCommand>
    {
        public override string Verb
        {
            get { return VerbsLifecycle.Install; }
        }

        [Fact]
        public void WhenIdSpecified_PackageManagerIsCalledToInstallLatestVersionAndDependencies()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.InstallPackage("Foo", null, false, false));
        }

        [Fact]
        public void WhenIdAndVersionSpecified_PackageManagerIsCalledToInstallSpecificVersionAndDependencies()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";
            cmd.Version = new Version(1, 0, 0, 0);

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.InstallPackage("Foo", new SemanticVersion(1, 0, 0, 0), false, false));
        }

        [Fact]
        public void WhenIgnoreDependenciesSpecified_PackageManagerIsCalledToInstallPackageAndIgnoreDependencies()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";
            cmd.IgnoreDependencies = SwitchParameter.Present;

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.InstallPackage("Foo", null, true, false));
        }

        [Fact]
        public void CommandOutputsDebugInformation()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand().AutoConfigure();
            Mock<IPackageManager> mockManager = cmd.AttachPackageManager();
            cmd.Source = "http://packages.nuget.org";
            cmd.Destination = @"D:\Foo";

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(new[] {
                @"Using Source: http://packages.nuget.org",
                @"Installing To: D:\Foo"
            }, output.DebugStream.ToArray());
        }
    }
}

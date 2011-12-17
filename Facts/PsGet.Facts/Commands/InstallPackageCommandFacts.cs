using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Cmdlets;
using System.Management.Automation;
using NuGet;
using Moq;

namespace PsGet.Facts.Commands
{
    public class InstallPackageCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            AttributeAssert.Has(typeof(InstallPackageCommand), new CmdletAttribute(VerbsLifecycle.Install, "Package"));
        }

        [Fact]
        public void VerifyIdParameter()
        {
            AttributeAssert.Has(
                () => new InstallPackageCommand().Id,
                new ParameterAttribute() {
                    Position = 0, 
                    Mandatory = true, 
                    ValueFromPipelineByPropertyName = true
                });
        }

        [Fact]
        public void VerifyVersionParameter()
        {
            AttributeAssert.Has(
                () => new InstallPackageCommand().Version,
                new ParameterAttribute()
                {
                    Position = 1
                });
            AttributeAssert.Has(
                () => new InstallPackageCommand().Version,
                new ValidateNotNullAttribute());
        }

        [Fact]
        public void VerifySourceParameter()
        {
            AttributeAssert.Has(
                () => new InstallPackageCommand().Source,
                new ParameterAttribute()
                {
                    Position = 2
                });
            AttributeAssert.Has(
                () => new InstallPackageCommand().Source,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyDestinationParameter()
        {
            AttributeAssert.Has(
                () => new InstallPackageCommand().Destination,
                new ParameterAttribute()
                {
                    Position = 3
                });
            AttributeAssert.Has(
                () => new InstallPackageCommand().Destination,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyIgnoreDependenciesParameter()
        {
            AttributeAssert.Has(
                () => new InstallPackageCommand().IgnoreDependencies,
                new ParameterAttribute());
        }

        [Fact]
        public void SourceIsSetToDefaultSourceIfNotSpecified()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(Settings.TheDefaultSource, cmd.Source);
        }

        [Fact]
        public void SourceIsLeftAloneIfSpecified()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            cmd.Source = "http://packages.nuget.org";

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal("http://packages.nuget.org", cmd.Source);
        }

        [Fact]
        public void DestinationIsSetToInstallationRootIfNotSpecified()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(TestHostEnvironment.DefaultInstallationRoot, cmd.Destination);
        }

        [Fact]
        public void DestinationIsLeftAloneIfSpecified()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            cmd.Source = @"C:\Foo\Bar";

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(@"C:\Foo\Bar", cmd.Source);
        }

        [Fact]
        public void WhenIdSpecified_PackageManagerIsCalledToInstallLatestVersionAndDependencies()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            Mock<IPackageManager> mockManager = new Mock<IPackageManager>();
            cmd.PackageManagerFactory = (source, destination) =>
            {
                Assert.Equal(source, cmd.Source);
                Assert.Equal(destination, cmd.Destination);
                return mockManager.Object;
            };
            cmd.Id = "Foo";

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.InstallPackage("Foo", null, false));
        }

        [Fact]
        public void WhenIdAndVersionSpecified_PackageManagerIsCalledToInstallSpecificVersionAndDependencies()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            Mock<IPackageManager> mockManager = new Mock<IPackageManager>();
            cmd.PackageManagerFactory = (source, destination) =>
            {
                Assert.Equal(source, cmd.Source);
                Assert.Equal(destination, cmd.Destination);
                return mockManager.Object;
            };
            cmd.Id = "Foo";
            cmd.Version = new Version(1, 0, 0, 0);

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.InstallPackage("Foo", new Version(1, 0, 0, 0), false));
        }

        [Fact]
        public void WhenIgnoreDependenciesSpecified_PackageManagerIsCalledToInstallPackageAndIgnoreDependencies()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            Mock<IPackageManager> mockManager = new Mock<IPackageManager>();
            cmd.PackageManagerFactory = (source, destination) =>
            {
                Assert.Equal(source, cmd.Source);
                Assert.Equal(destination, cmd.Destination);
                return mockManager.Object;
            };
            cmd.Id = "Foo";
            cmd.IgnoreDependencies = SwitchParameter.Present;

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.InstallPackage("Foo", null, true));
        }

        [Fact]
        public void CommandOutputsDebugInformation()
        {
            // Arrange
            InstallPackageCommand cmd = new InstallPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            Mock<IPackageManager> mockManager = new Mock<IPackageManager>();
            cmd.PackageManagerFactory = (source, destination) =>
            {
                Assert.Equal(source, cmd.Source);
                Assert.Equal(destination, cmd.Destination);
                return mockManager.Object;
            };
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

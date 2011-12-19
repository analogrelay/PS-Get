using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using Moq;
using PsGet.Facts.TestDoubles;
using System.Management.Automation;

namespace PsGet.Facts.Commands
{
    public class PackageManagerCmdletWithSourceFacts
    {
        [Fact]
        public void VerifySourceParameter()
        {
            CmdletAssert.IsParameter(
                () => new Mock<PackageManagerCmdletWithSource>().Object.Source,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyDestinationParameter()
        {
            CmdletAssert.IsParameter(
                () => new Mock<PackageManagerCmdletWithSource>().Object.Destination,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void SourceIsSetToDefaultSourceIfNotSpecified()
        {
            // Arrange
            PackageManagerCmdletWithSource cmd = new Mock<PackageManagerCmdletWithSource>() { CallBase = true }.Object.AutoConfigure();

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(Settings.TheDefaultSource, cmd.Source);
        }

        [Fact]
        public void SourceIsLeftAloneIfSpecified()
        {
            // Arrange
            PackageManagerCmdletWithSource cmd = new Mock<PackageManagerCmdletWithSource>() { CallBase = true }.Object.AutoConfigure();
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
            PackageManagerCmdletWithSource cmd = new Mock<PackageManagerCmdletWithSource>() { CallBase = true }.Object.AutoConfigure();

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(TestHostEnvironment.DefaultInstallationRoot, cmd.Destination);
        }

        [Fact]
        public void DestinationIsLeftAloneIfSpecified()
        {
            // Arrange
            PackageManagerCmdletWithSource cmd = new Mock<PackageManagerCmdletWithSource>() { CallBase = true }.Object.AutoConfigure();
            cmd.Source = @"C:\Foo\Bar";

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(@"C:\Foo\Bar", cmd.Source);
        }
    }
}

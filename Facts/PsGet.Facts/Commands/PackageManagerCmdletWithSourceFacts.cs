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
                () => new Mock<PackageManagementCommand>().Object.Source,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyDestinationParameter()
        {
            CmdletAssert.IsParameter(
                () => new Mock<PackageManagementCommand>().Object.Destination,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void SourceIsAggregateFromPackageSourceServiceWhenNotSpecified()
        {
            // Arrange
            PackageManagementCommand cmd = new Mock<PackageManagementCommand>() { CallBase = true }.Object.AutoConfigure();
            
        }

        [Fact]
        public void SourceIsLeftAloneIfSpecified()
        {
            // Arrange
            PackageManagementCommand cmd = new Mock<PackageManagementCommand>() { CallBase = true }.Object.AutoConfigure();
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
            PackageManagementCommand cmd = new Mock<PackageManagementCommand>() { CallBase = true }.Object.AutoConfigure();

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(TestHostEnvironment.DefaultInstallationRoot, cmd.Destination);
        }

        [Fact]
        public void DestinationIsLeftAloneIfSpecified()
        {
            // Arrange
            PackageManagementCommand cmd = new Mock<PackageManagementCommand>() { CallBase = true }.Object.AutoConfigure();
            cmd.Source = @"C:\Foo\Bar";

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(@"C:\Foo\Bar", cmd.Source);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using NuGet;
using Moq;
using PsGet.Commands;
using PsGet.Facts.TestDoubles;

namespace PsGet.Facts.Commands
{
    public class PsGetCommandFacts
    {
        [Fact]
        public void ConstructorUsesDefaultRepositoryFactory()
        {
            Assert.Same(PackageRepositoryFactory.Default, new Mock<PsGetCommand>() { CallBase = true }.Object.RepositoryFactory);
        }

        [Fact]
        public void BeginProcessingInitializesSettingsAndCallsCoreProcessor()
        {
            // Arrange
            PsGetCommand cmdlet = new Mock<PsGetCommand>() { CallBase = true }.Object;
            cmdlet.HostEnvironment = new TestHostEnvironment(@"C:\Foo");
            
            // Act
            cmdlet.BeginProcessingCore();

            // Assert
            Assert.Equal(@"C:\", cmdlet.Config.InstallationRoot);
        }
    }
}

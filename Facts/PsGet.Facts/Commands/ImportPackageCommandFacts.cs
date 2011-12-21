using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Commands;
using System.Management.Automation;
using Xunit;
using NuGet;
using Moq;
using PsGet.Facts.TestDoubles;

namespace PsGet.Facts.Commands
{
    public class ImportPackageCommandFacts : InstallPackageCommandFactsBase<ImportPackageCommand>
    {
        private SimpleModuleMetadata[] testModules = new[] { 
            new SimpleModuleMetadata("Foo", new Version(2, 0, 0, 0))
        };

        public override string Verb
        {
            get { return VerbsData.Import; }
        }

        [Fact]
        public void WhenModuleAvailable_ModuleIsNotInstalled()
        {
            // Arrange
            ImportPackageCommand cmd = new ImportPackageCommand().AutoConfigure();
            TestCommandInvoker invoker = cmd.AttachInvoker();
            var mockManager = cmd.AttachPackageManager();

            invoker.RegisterScript("Get-Module -ListAvailable Foo", () => testModules);

            cmd.Id = "Foo";

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(
                m => m.InstallPackage(It.IsAny<string>(), It.IsAny<SemanticVersion>(), It.IsAny<bool>(), It.IsAny<bool>()),
                Times.Never());
            Assert.Equal(1, invoker.GetInvokeCount("Import-Module Foo"));
        }

        [Fact]
        public void WhenModuleNotAvailable_ModuleIsInstalledAndImported()
        {
            // Arrange
            ImportPackageCommand cmd = new ImportPackageCommand().AutoConfigure();
            TestCommandInvoker invoker = cmd.AttachInvoker();
            var mockManager = cmd.AttachPackageManager();

            invoker.RegisterScript("Get-Module -ListAvailable Foo", () => Enumerable.Empty<SimpleModuleMetadata>());

            cmd.Id = "Foo";
            
            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(
                m => m.InstallPackage("Foo", null, false, false));
            Assert.Equal(1, invoker.GetInvokeCount("Import-Module Foo"));
        }

        [Fact]
        public void WhenVersionSpecifiedAndInstalledVersionDoesNotMatch_ModuleIsUpdated()
        {
            // Arrange
            ImportPackageCommand cmd = new ImportPackageCommand().AutoConfigure();
            TestCommandInvoker invoker = cmd.AttachInvoker();
            var mockManager = cmd.AttachPackageManager();

            invoker.RegisterScript("Get-Module -ListAvailable Foo", () => testModules);

            cmd.Id = "Foo";
            cmd.Version = new Version(3, 0, 0, 0);

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(
                m => m.UpdatePackage("Foo", new SemanticVersion(3, 0, 0, 0), true, false));
            Assert.Equal(1, invoker.GetInvokeCount("Import-Module Foo"));
        }
    }
}

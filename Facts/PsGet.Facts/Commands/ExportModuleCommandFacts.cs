using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Management.Automation;
using PsGet.Commands;
using PsGet.Facts.TestDoubles;
using PsGet.Abstractions;
using Moq;
using NuGet;
using System.IO;
using IFileSystem = PsGet.Abstractions.IFileSystem;

namespace PsGet.Facts.Commands
{
    public class ExportModuleCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(ExportModuleCommand), VerbsData.Export, "Module");
        }

        [Fact]
        public void VerifyNameParameter()
        {
            CmdletAssert.IsParameter(
                () => new ExportModuleCommand().Name,
                new ParameterAttribute()
                {
                    Position = 0,
                    Mandatory = true,
                    ValueFromPipeline = true,
                    ValueFromPipelineByPropertyName = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void WhenNameSpecified_ThrowsIfModuleDoesNotExist()
        {
            // Arrange
            ExportModuleCommand cmd = new ExportModuleCommand().AutoConfigure();
            TestCommandInvoker invoker = cmd.AttachInvoker();
            cmd.Name = "Foo";
            invoker.RegisterScript("Get-Module -ListAvailable 'Foo'", () => Enumerable.Empty<IModuleMetadata>());

            // Act/Assert
            KeyNotFoundException ex = Assert.Throws<KeyNotFoundException>(() => cmd.Execute());
            Assert.Equal("Module not found: 'Foo'", ex.Message);
        }

        [Fact]
        public void WhenNameSpecified_ThrowsIfModuleNameIsInvalid()
        {
            // Arrange
            ExportModuleCommand cmd = new ExportModuleCommand().AutoConfigure();
            TestCommandInvoker invoker = cmd.AttachInvoker();
            cmd.Name = "Foo";

            invoker.RegisterScript("Get-Module -ListAvailable 'Foo'", () => new[] { 
                new SimpleModuleMetadata("Foo Bar", new Version(1, 0, 0, 0)) 
            });

            // Act/Assert
            InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => cmd.Execute());
            Assert.Equal("Module name 'Foo Bar' is invalid, names must consist only of letters, numbers, '_' and '-'.", ex.Message);
        }

        [Fact]
        public void ProcessRecordBuildsAndSavesPackage()
        {
            // Arrange
            Mock<ExportModuleCommand> mockCmd = new Mock<ExportModuleCommand>() { CallBase = true };
            mockCmd.Object.AutoConfigure();
            TestCommandInvoker invoker = mockCmd.Object.AttachInvoker();
            mockCmd.Object.Name = "Foo";
            mockCmd.Object.AttachPathService(@"D:\Current\Path", @"E:\Pre|");

            // - First, configure the module to be returned
            invoker.RegisterScript("Get-Module -ListAvailable 'Foo'", () => new[] {
                new SimpleModuleMetadata("Foo", new Version(1, 0, 0, 0)) {
                    ModuleBase = @"D:\Modules\Foo"
                }
            });

            // - Configure the package builder
            IPackageFile file = new Mock<IPackageFile>().Object;
            PackageBuilder expected = new PackageBuilder() { Id = "Bar" };
            expected.Files.Add(file);
            mockCmd.Setup(c => c.BuildManifest(It.Is<IModuleMetadata>(m => m.ModuleBase == @"D:\Modules\Foo"), It.IsAny<IFileSystem>()))
                   .Returns(expected);

            // - Set up to capture PackageBuilder
            PackageBuilder actual = null;
            string path = null;
            mockCmd.Setup(c => c.SavePackage(It.IsAny<PackageBuilder>(), It.IsAny<string>()))
                   .Callback<PackageBuilder, string>((b, p) => { path = p; actual = b; });

            // Act
            mockCmd.Object.Execute();

            // Assert
            Assert.Equal(@"E:\Pre|D:\Current\Path\Bar.nupkg", path);
            Assert.Equal(file, actual.Files.Single());
            Assert.Same(expected, actual);
        }

        [Fact]
        public void WhenNoPackageFileSpecified_SavePackageUsesModuleNameAndCurrentPathToGeneratePackageFileName()
        {
            // Arrange
            Mock<ExportModuleCommand> mockCmd = new Mock<ExportModuleCommand>() { CallBase = true };
            mockCmd.Object.AutoConfigure();
            mockCmd.Object.AttachPathService(@"D:\Modules", @"D:\Foo|");

            mockCmd.Setup(s => s.SavePackage(It.IsAny<PackageBuilder>(), @"D:\Foo|D:\Modules\Baz.nupkg"))
                   .Callback<PackageBuilder, string>((_, __) => { })
                   .Verifiable();
            PackageBuilder builder = new PackageBuilder() {
                Id = "Baz"
            };
            
            // Act
            mockCmd.Object.SavePackage(builder);

            // Assert
            mockCmd.VerifyAll();
        }
    }
}

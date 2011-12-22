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
using Xunit.Extensions;

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
        public void VerifyPackageFileParameter()
        {
            CmdletAssert.IsParameter(
                () => new ExportModuleCommand().PackageFile,
                new ParameterAttribute()
                {
                    Position = 1,
                    Mandatory = false
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyDescriptionParameter()
        {
            CmdletAssert.IsParameter(
                () => new ExportModuleCommand().Description,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyPackageIdParameter()
        {
            CmdletAssert.IsParameter(
                () => new ExportModuleCommand().PackageId,
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyVersionParameter()
        {
            CmdletAssert.IsParameter(
                () => new ExportModuleCommand().Version,
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

        [Fact]
        public void WhenSpecFileExistsWithFilesSection_BuildManifestUsesSpecAndSpecifiedFiles()
        {
            // Arrange
            Mock<ExportModuleCommand> mockCmd = new Mock<ExportModuleCommand>() { CallBase = true };
            PackageBuilder expected = new PackageBuilder();
            IPackageFile file = new Mock<IPackageFile>().Object;
            expected.Files.Add(file);

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>();
            mockFs.Setup(fs => fs.OpenFile(It.IsAny<string>())).Returns(Stream.Null);
            mockFs.Setup(fs => fs.FileExists("Foo.nuspec")).Returns(true);
            mockCmd.Setup(c => c.OpenManifest(@"D:\Foo", Stream.Null)).Returns(expected);
            IModuleMetadata module = new SimpleModuleMetadata("Foo", new Version(1, 0, 0, 0))
            {
                ModuleBase = @"D:\Foo"
            };

            // Act
            PackageBuilder actual = mockCmd.Object.BuildManifest(module, mockFs.Object);

            // Assert
            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData("Someone", new[] { "Someone" })]
        [InlineData("Someone,Someoneelse", new[] { "Someone", "Someoneelse" })]
        [InlineData(null, new string[] { null })]
        public void WhenSpecFileExistsWithoutFilesSection_BuildManifestUsesSpecAndAllFilesUnderModuleBase(string author, string[] expectedAuthors)
        {
            // Arrange
            Mock<ExportModuleCommand> mockCmd = new Mock<ExportModuleCommand>() { CallBase = true };
            
            Mock<IFileSystem> mockFs = new Mock<IFileSystem>();
            mockFs.Setup(fs => fs.FileExists("Foo.nuspec")).Returns(false);
            mockFs.Setup(fs => fs.GetAllFiles()).Returns(new[] {
                "Foo",
                "Bar",
                "Baz"
            });
            mockFs.Setup(fs => fs.GetFullPath(It.IsAny<string>())).Returns<string>(s => String.Concat("Base\\", s));
            IModuleMetadata module = new SimpleModuleMetadata("Foo", new Version(1, 0, 0, 0))
            {
                ModuleBase = @"D:\Foo",
                Description = "Bar Baz",
                Author = author
            };

            // Act
            PackageBuilder actual = mockCmd.Object.BuildManifest(module, mockFs.Object);

            // Assert
            Assert.Equal("Foo", actual.Id);
            Assert.Equal(new SemanticVersion(1, 0, 0, 0), actual.Version);
            Assert.Equal("Bar Baz", actual.Description);
            Assert.Equal(expectedAuthors.Select(h => h ?? Environment.UserName).ToArray(), actual.Authors.ToArray());
            Assert.Equal(new IPackageFile[] {
                new PhysicalPackageFile() { SourcePath = @"Base\Foo", TargetPath = "Foo" },
                new PhysicalPackageFile() { SourcePath = @"Base\Bar", TargetPath = "Bar" },
                new PhysicalPackageFile() { SourcePath = @"Base\Baz", TargetPath = "Baz" },
            }, actual.Files.ToArray());
        }

        [Fact]
        public void ParametersOverrideModuleSpecFile()
        {
            // Arrange
            Mock<ExportModuleCommand> mockCmd = new Mock<ExportModuleCommand>() { CallBase = true };
            mockCmd.Object.Description = "Override";
            mockCmd.Object.PackageId = "Override";
            mockCmd.Object.Version = new SemanticVersion(2, 0, 0, 0);
            mockCmd.Object.Authors = new string[] { "Over", "Ride" };
            mockCmd.Object.Copyright = "Override";
            mockCmd.Object.IconUrl = "http://over.ride";
            mockCmd.Object.Language = "ovr-RIDE";
            mockCmd.Object.LicenseUrl = "http://over.ride";
            mockCmd.Object.Owners = new string[] { "Over", "Ride" };
            mockCmd.Object.ProjectUrl = "http://over.ride";
            mockCmd.Object.ReleaseNotes = "Override";
            mockCmd.Object.RequireLicenseAcceptance = SwitchParameter.Present;
            mockCmd.Object.Summary = "Override";
            mockCmd.Object.Tags = new string[] { "Over", "Ride" };
            mockCmd.Object.Title = "Override";

            PackageBuilder expected = new PackageBuilder()
            {
                Id = "Original",
                Description = "Original",
                Version = new SemanticVersion(1, 0, 0, 0)
            };

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>();
            mockFs.Setup(fs => fs.FileExists("Original.nuspec")).Returns(true);
            mockFs.Setup(fs => fs.GetAllFiles()).Returns(new string[0]);
            mockFs.Setup(fs => fs.OpenFile(It.IsAny<string>())).Returns(Stream.Null);
            mockCmd.Setup(c => c.OpenManifest(@"D:\Foo", Stream.Null)).Returns(expected);
            IModuleMetadata module = new SimpleModuleMetadata("Original", new Version(1, 0, 0, 0))
            {
                ModuleBase = @"D:\Foo"
            };

            // Act
            PackageBuilder actual = mockCmd.Object.BuildManifest(module, mockFs.Object);

            // Assert
            Assert.Equal("Override", actual.Description);
            Assert.Equal("Override", actual.Id);
            Assert.Equal(new SemanticVersion(2, 0, 0, 0), actual.Version);
            Assert.Equal(new string[] { "Over", "Ride" }, actual.Authors.ToArray());
            Assert.Equal("Override", actual.Copyright);
            Assert.Equal("http://over.ride/", actual.IconUrl.ToString());
            Assert.Equal("ovr-RIDE", actual.Language);
            Assert.Equal("http://over.ride/", actual.LicenseUrl.ToString());
            Assert.Equal(new string[] { "Over", "Ride" }, actual.Owners.ToArray());
            Assert.Equal("http://over.ride/", actual.ProjectUrl.ToString());
            Assert.Equal("Override", actual.ReleaseNotes);
            Assert.True(actual.RequireLicenseAcceptance);
            Assert.Equal("Override", actual.Summary);
            Assert.Equal(new string[] { "Over", "Ride" }, actual.Tags.ToArray());
            Assert.Equal("Override", actual.Title);
        }

        [Fact]
        public void ParametersOverrideModuleMetadata()
        {
            // Arrange
            Mock<ExportModuleCommand> mockCmd = new Mock<ExportModuleCommand>() { CallBase = true };
            mockCmd.Object.Description = "Override";
            mockCmd.Object.PackageId = "Override";
            mockCmd.Object.Version = new SemanticVersion(2, 0, 0, 0);

            Mock<IFileSystem> mockFs = new Mock<IFileSystem>();
            mockFs.Setup(fs => fs.FileExists("Original.nuspec")).Returns(false);
            mockFs.Setup(fs => fs.GetAllFiles()).Returns(new string[0]);
            mockFs.Setup(fs => fs.OpenFile(It.IsAny<string>())).Returns(Stream.Null);
            IModuleMetadata module = new SimpleModuleMetadata("Original", new Version(1, 0, 0, 0))
            {
                ModuleBase = @"D:\Foo"
            };

            // Act
            PackageBuilder actual = mockCmd.Object.BuildManifest(module, mockFs.Object);

            // Assert
            Assert.Equal("Override", actual.Description);
            Assert.Equal("Override", actual.Id);
            Assert.Equal(new SemanticVersion(2, 0, 0, 0), actual.Version);
        }
    }
}

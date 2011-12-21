using System;
using System.Management.Automation;
using PsGet.Commands;
using Xunit;
using NuGet;

namespace PsGet.Facts.Commands
{
    public class UpdatePackageCommandFacts : InstallPackageCommandFactsBase<UpdatePackageCommand>
    {
        public override string Verb
        {
            get { return VerbsData.Update; }
        }

        [Fact]
        public void WhenIdSpecified_PackageAndDependenciesAreUpdatedToLatestVersion()
        {
            // Arrange
            UpdatePackageCommand cmd = new UpdatePackageCommand().AutoConfigure();
            var mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.UpdatePackage("Foo", (SemanticVersion)null, true, false));
        }

        [Fact]
        public void WhenVersionSpecified_PackageAndDependenciesAreUpdatedToSpecifiedVersion()
        {
            // Arrange
            UpdatePackageCommand cmd = new UpdatePackageCommand().AutoConfigure();
            var mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";
            cmd.Version = new Version(2, 0, 0, 0);

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.UpdatePackage("Foo", new SemanticVersion(2, 0, 0, 0), true, false));
        }

        [Fact]
        public void WhenIgnoreDependenciesSpecified_PackageDependenciesAreNotUpdated()
        {
            // Arrange
            UpdatePackageCommand cmd = new UpdatePackageCommand().AutoConfigure();
            var mockManager = cmd.AttachPackageManager();
            cmd.Id = "Foo";
            cmd.IgnoreDependencies = SwitchParameter.Present;

            // Act
            cmd.Execute();

            // Assert
            mockManager.Verify(m => m.UpdatePackage("Foo", (SemanticVersion)null, false, false));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Management.Automation;
using PsGet.Cmdlets;
using NuGet;
using PsGet.Facts.TestDoubles;

namespace PsGet.Facts.Commands
{
    public class GetPackageCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(GetPackageCommand), VerbsCommon.Get, "Package");
        }

        [Fact]
        public void VerifyIdParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageCommand().Id,
                new ParameterAttribute() {
                Position = 0,
                ValueFromPipelineByPropertyName = true,
                HelpMessage = "A filter to apply to the ID of the packages on the server"
            });
        }

        [Fact]
        public void VerifySourceParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageCommand().Source, 
                new ParameterAttribute() {
                Position = 1,
                HelpMessage = "The NuGet feed to list packages from"
            }, 
            new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyAllVersionsParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageCommand().AllVersions, 
                new ParameterAttribute() {
                HelpMessage = "Show all versions of packages"
            });
        }

        [Fact]
        public void SourceIsSetToDefaultSourceIfNotSpecified()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal(Settings.TheDefaultSource, cmd.Source);
        }

        [Fact]
        public void SourceIsLeftAloneIfSpecified()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand();
            cmd.HostEnvironment = new TestHostEnvironment();
            cmd.Source = "http://packages.nuget.org";

            // Act
            cmd.BeginProcessingCore();

            // Assert
            Assert.Equal("http://packages.nuget.org", cmd.Source);
        }

        [Fact]
        public void WhenNoArguments_WritesLatestVersionsOfAllPackagesFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages.Latest().ToArray(), output.ObjectStream.Cast<IPackage>().ToArray());
        }

        [Fact]
        public void WhenAllVersionsSpecified_WritesAllVersionsOfAllPackagesFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();
            cmd.AllVersions = SwitchParameter.Present;

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages, output.ObjectStream.Cast<IPackage>().ToArray());
        }

        [Fact]
        public void WhenIdSpecified_WritesLatestVersionOfSpecifiedPackageFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();
            cmd.Id = "Foo";

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages.Latest().IdIsExactly("Foo").ToArray(), 
                         output.ObjectStream.Cast<IPackage>().ToArray());
        }

        [Fact]
        public void WhenIdAndAllVersionsSpecified_WritesAllVersionsOfSpecifiedPackageFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();
            cmd.Id = "Foo";
            cmd.AllVersions = SwitchParameter.Present;

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages.IdIsExactly("Foo").ToArray(),
                         output.ObjectStream.Cast<IPackage>().ToArray());
        }

        [Fact]
        public void WhenIdSpecified_WritesAllPackagesContainingIdFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();
            cmd.Id = "o";

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages.Latest().IdContains("o").ToArray(),
                         output.ObjectStream.Cast<IPackage>().ToArray());
        }

        [Fact]
        public void WhenIdAndAllVersionsSpecified_WritesAllVersionsOfAllPackagesContainingIdFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();
            cmd.Id = "o";
            cmd.AllVersions = SwitchParameter.Present;

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages.IdContains("o").ToArray(),
                         output.ObjectStream.Cast<IPackage>().ToArray());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Management.Automation;
using PsGet.Commands;
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
        public void VerifyAllVersionsParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageCommand().AllVersions, 
                new ParameterAttribute() {
                HelpMessage = "Show all versions of packages"
            });
        }

        [Fact]
        public void WhenNoArguments_WritesLatestVersionsOfAllPackagesFromSource()
        {
            // Assert
            GetPackageCommand cmd = new GetPackageCommand().AutoConfigure();
            cmd.RepositoryFactory = TestRepository.CreateFactory();
            cmd.Source = "http://foo.bar";

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
            cmd.Source = "http://foo.bar";

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
            cmd.Source = "http://foo.bar";

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
            cmd.Source = "http://foo.bar";

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
            cmd.Source = "http://foo.bar";

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
            cmd.Source = "http://foo.bar";

            // Act
            CommandOutput output = cmd.Execute();

            // Assert
            Assert.Equal(TestRepository.DefaultPackages.IdContains("o").ToArray(),
                         output.ObjectStream.Cast<IPackage>().ToArray());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using Moq;
using PsGet.Facts.TestDoubles;
using System.Management.Automation;
using NuGet;
using PsGet.Services;

namespace PsGet.Facts.Commands
{
    public class PackageManagementCommandFacts
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

        [Fact]
        public void OpenRepositoryUsesRepositoryFactory()
        {
            // Arrange
            Mock<IPackageRepositoryFactory> mockFactory = new Mock<IPackageRepositoryFactory>();
            PackageManagementCommand cmdlet = new Mock<PackageManagementCommand>() { CallBase = true }.Object;
            cmdlet.RepositoryFactory = mockFactory.Object;
            cmdlet.Source = "foo";
            IPackageRepository expected = new Mock<IPackageRepository>().Object;
            mockFactory.Setup(f => f.CreateRepository("foo")).Returns(expected);

            // Act
            IPackageRepository actual = cmdlet.OpenRepository();

            // Assert
            Assert.Same(expected, actual);
        }

        [Fact]
        public void CreatePackageManagerUsesProvidedOverrides()
        {
            // Arrange
            Mock<IPackageRepositoryFactory> mockFactory = new Mock<IPackageRepositoryFactory>();
            PackageManagementCommand cmd = new Mock<PackageManagementCommand>() { CallBase = true }.Object;
            cmd.Source = "foo";
            cmd.Destination = @"D:\";
            cmd.RepositoryFactory = mockFactory.Object;
            IPackageRepository expected = new Mock<IPackageRepository>().Object;
            mockFactory.Setup(f => f.CreateRepository("foo")).Returns(expected);

            // Act
            IPackageManager manager = cmd.CreatePackageManager();

            // Assert
            PhysicalFileSystem fs = Assert.IsType<PhysicalFileSystem>(manager.FileSystem);
            DefaultPackagePathResolver resolver = Assert.IsType<DefaultPackagePathResolver>(manager.PathResolver);
            Assert.Same(expected, manager.SourceRepository);
            Assert.Equal(@"D:\", fs.Root);
            Assert.Equal(@"foo", resolver.GetPackageDirectory("foo", new Version(1, 0, 0, 0)));
        }

        [Fact]
        public void SourceIsAggregateFromPackageSourceServiceWhenNotSpecified()
        {
            // Arrange
            PackageManagementCommand cmd = new Mock<PackageManagementCommand>() { CallBase = true }.Object.AutoConfigure();
            Mock<IPackageRepositoryFactory> mockFactory = new Mock<IPackageRepositoryFactory>();
            Mock<PackageSourceService> mockService = new Mock<PackageSourceService>();
            mockService.Setup(s => s.AllSources).Returns(new[] {
                new PackageSource("http://foo.bar", "Foo"),
                new PackageSource("http://bar.baz", "Bar")
            });
            cmd.SourceService = mockService.Object;

            mockFactory.Setup(f => f.CreateRepository(It.IsAny<string>()))
                       .Returns<string>(src =>
                       {
                           Mock<IPackageRepository> mockRepo = new Mock<IPackageRepository>();
                           mockRepo.Setup(r => r.Source).Returns(src);
                           return mockRepo.Object;
                       });
            cmd.RepositoryFactory = mockFactory.Object;

            // Act
            IPackageRepository repo = cmd.OpenRepository();

            // Assert
            AggregateRepository aggrepo = Assert.IsType<AggregateRepository>(repo);
            Assert.Equal(new[] {
                "http://foo.bar",
                "http://bar.baz"
            }, aggrepo.Repositories.Select(r => r.Source).ToArray());
        }
    }
}

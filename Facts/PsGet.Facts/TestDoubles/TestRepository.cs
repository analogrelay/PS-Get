using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Facts.TestDoubles
{
    // Test Doubles are tested via their use in unit tests
    [ExcludeFromCodeCoverage]
    public class TestRepository : IPackageRepository
    {
        public static readonly IPackage[] DefaultPackages = new IPackage[] {
            new SimplePackage("Foo", new Version(1, 0, 0, 0), latest: false),
            new SimplePackage("Foo", new Version(1, 1, 0, 0), latest: false),
            new SimplePackage("Foo", new Version(1, 1, 5, 0), latest: false),
            new SimplePackage("Foo", new Version(2, 0, 0, 0), latest: false),
            new SimplePackage("Foo", new Version(3, 0, 0, 0), latest: true),
            new SimplePackage("Bar", new Version(1, 0, 0, 0), latest: false),
            new SimplePackage("Bar", new Version(1, 1, 0, 0), latest: false),
            new SimplePackage("Bar", new Version(1, 1, 5, 0), latest: false),
            new SimplePackage("Bar", new Version(2, 0, 0, 0), latest: false),
            new SimplePackage("Bar", new Version(3, 0, 0, 0), latest: true),
            new SimplePackage("Bob", new Version(2, 0, 0, 0), latest: false),
            new SimplePackage("Bob", new Version(3, 0, 0, 0), latest: true)
        };

        public IList<IPackage> Packages { get; private set; }
        public string Source { get; private set; }

        public TestRepository(string source, IEnumerable<IPackage> packages) {
            Source = source;
            Packages = new List<IPackage>(packages);
        }

        public static IPackageRepositoryFactory CreateFactory() { return CreateFactory(DefaultPackages); }
        public static IPackageRepositoryFactory CreateFactory(IPackage[] packages) { return CreateFactory((IEnumerable<IPackage>)packages); }
        public static IPackageRepositoryFactory CreateFactory(IEnumerable<IPackage> packages) {
            return CreateFactory(source => new TestRepository(source, packages));
        }

        public static IPackageRepositoryFactory CreateFactory(Func<string, IPackageRepository> factory)
        {
            Mock<IPackageRepositoryFactory> mockFactory = new Mock<IPackageRepositoryFactory>();
            mockFactory.Setup(f => f.CreateRepository(It.IsAny<string>())).Returns<string>(factory);
            return mockFactory.Object;
        }

        public void AddPackage(IPackage package)
        {
            Packages.Add(package);
        }

        public IQueryable<IPackage> GetPackages()
        {
            return Packages.AsQueryable();
        }

        public void RemovePackage(IPackage package)
        {
            Packages.Remove(package);
        }
    }
}

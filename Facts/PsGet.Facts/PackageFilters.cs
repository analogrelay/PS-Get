using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using Xunit;

namespace PsGet.Facts
{
    public static class PackageFilters
    {
        public static IEnumerable<IPackage> IdContains(this IEnumerable<IPackage> self, string id)
        {
            return self.Where(p => p.Id.Contains(id));
        }

        public static IEnumerable<IPackage> IdIsExactly(this IEnumerable<IPackage> self, string id)
        {
            return self.Where(p => p.Id == id);
        }

        public static IEnumerable<IPackage> Latest(this IEnumerable<IPackage> self)
        {
            return self.Where(p => p.IsLatestVersion);
        }
    }

    public class PackageFilterFacts
    {
        IPackage[] testData = new[] {
            new SimplePackage("FooBarBaz", new Version(1, 0, 0, 0), latest: false),
            new SimplePackage("GrueBarGra", new Version(1, 0, 0, 0), latest: true),
            new SimplePackage("DapBarTyz", new Version(1, 0, 0, 0), latest: false),
            new SimplePackage("BizBozBonk", new Version(1, 0, 0, 0), latest: true)
        };

        [Fact]
        public void IdContainsMatchesAnyPackageContainingId()
        {
            Assert.Equal(new IPackage[] {
                new SimplePackage("FooBarBaz", new Version(1, 0, 0, 0), latest: false),
                new SimplePackage("GrueBarGra", new Version(1, 0, 0, 0), latest: true),
                new SimplePackage("DapBarTyz", new Version(1, 0, 0, 0), latest: false)
            }, testData.IdContains("Bar").ToArray());
        }

        [Fact]
        public void IdIsExactlyMatchesPackageWithSpecifiedId()
        {
            Assert.Equal(new IPackage[] {
                new SimplePackage("BizBozBonk", new Version(1, 0, 0, 0), latest: true)
            }, testData.IdIsExactly("BizBozBonk").ToArray());
        }

        [Fact]
        public void LatestMatchesOnlyLatestVersionPackages()
        {
            Assert.Equal(new IPackage[] {
                new SimplePackage("GrueBarGra", new Version(1, 0, 0, 0), latest: true),
                new SimplePackage("BizBozBonk", new Version(1, 0, 0, 0), latest: true)
            }, testData.Latest().ToArray());
        }
    }
}

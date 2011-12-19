using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using System.Management.Automation;
using PsGet.Facts.TestDoubles;
using PsGet.Services;
using Moq;
using NuGet;
using System.Linq.Expressions;
using Xunit.Extensions;

namespace PsGet.Facts.Commands
{
    public class GetPackageSourceCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(GetPackageSourceCommand), VerbsCommon.Get, "PackageSource");
        }

        [Fact]
        public void VerifyNameParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageSourceCommand().Name,
                new ParameterAttribute()
                {
                    Position = 0,
                    ValueFromPipeline = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifyScopeParameter()
        {
            CmdletAssert.IsParameter(
                () => new GetPackageSourceCommand().Scope,
                new ParameterAttribute()
                {
                    Position = 1
                });
        }

        [Fact]
        public void WhenNoParametersSpecified_WritesMergedSourcesList()
        {
            // Arrange
            GetPackageSourceCommand cmd = new GetPackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>() { CallBase = true };
            cmd.SourceService = mockService.Object;

            PackageSource[] expected = new[] {
                new PackageSource("http://foo.bar", "Foo"),
                new PackageSource("http://bar.baz", "Bar"),
                new PackageSource("http://baz.boz", "Baz")
            };

            mockService.Setup(s => s.AllSources).Returns(expected);

            // Act
            PackageSource[] actual = cmd.Execute().ObjectStream.Cast<PackageSource>().ToArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [PropertyData("ScopeSourceData")]
        public void WhenScopeParameterSpecified_WritesSourceList(PackageSourceScope scope, Expression<Func<PackageSourceService, IPackageSourceStore>> storeSelector)
        {
            // Arrange
            GetPackageSourceCommand cmd = new GetPackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>() { CallBase = true };
            cmd.SourceService = mockService.Object;
            cmd.Scope = scope;

            PackageSource[] expected = new[] {
                new PackageSource("http://foo.bar", "Foo"),
                new PackageSource("http://bar.baz", "Bar"),
                new PackageSource("http://baz.boz", "Baz")
            };
            mockService.Setup(storeSelector).Returns(new InMemorySourceStore(expected));

            // Act
            PackageSource[] actual = cmd.Execute().ObjectStream.Cast<PackageSource>().ToArray();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WhenNameSpecified_FiltersListByWildcard()
        {
            // Arrange
            GetPackageSourceCommand cmd = new GetPackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>() { CallBase = true };
            cmd.SourceService = mockService.Object;
            cmd.Name = "*ar";
            
            PackageSource[] expected = new[] {
                new PackageSource("http://foo.bar", "Foo"),
                new PackageSource("http://bar.baz", "Bar"),
                new PackageSource("http://baz.boz", "Qar")
            };
            mockService.Setup(s => s.AllSources).Returns(expected);

            // Act
            PackageSource[] actual = cmd.Execute().ObjectStream.Cast<PackageSource>().ToArray();

            // Assert
            Assert.Equal(expected.Skip(1).ToArray(), actual);
        }

        public static IEnumerable<object[]> ScopeSourceData
        {
            get
            {
                yield return new object[] { PackageSourceScope.User, MakeExpr(svc => svc.UserStore) };
                yield return new object[] { PackageSourceScope.Session, MakeExpr(svc => svc.SessionStore) };
                yield return new object[] { PackageSourceScope.Machine, MakeExpr(svc => svc.MachineStore) };
            }
        }

        private static Expression<Func<PackageSourceService, IPackageSourceStore>> MakeExpr(Expression<Func<PackageSourceService, IPackageSourceStore>> e) {
            return e;
        }
    }
}

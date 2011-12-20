using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Commands;
using System.Management.Automation;
using PsGet.Services;
using Moq;
using NuGet;
using Xunit.Extensions;
using System.Linq.Expressions;

namespace PsGet.Facts.Commands
{
    public class RemovePackageSourceCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(RemovePackageSourceCommand), VerbsCommon.Remove, "PackageSource");
        }

        [Fact]
        public void VerifyNameParameter()
        {
            CmdletAssert.IsParameter(
                () => new RemovePackageSourceCommand().Name,
                new ParameterAttribute()
                {
                    Position = 0,
                    Mandatory = true,
                    ValueFromPipelineByPropertyName = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void WithNoScope_RemovesSourceFromSessionScope()
        {
            // Arrange
            RemovePackageSourceCommand cmd = new RemovePackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>(MockBehavior.Strict) { CallBase = true };
            InMemorySourceStore sessionStore = new InMemorySourceStore(new[] {
                new PackageSource("http://foo.bar", "Foo")
            });

            cmd.SourceService = mockService.Object;
            mockService.Setup(s => s.SessionStore).Returns(sessionStore);

            cmd.Name = "Foo";
            
            // Act
            cmd.Execute();

            // Assert
            Assert.False(sessionStore.Sources.Any());
        }

        [Theory]
        [PropertyData("ScopeSourceData")]
        public void WhenScopeParameterSpecified_RemovesSourceFromMatchingList(PackageSourceScope scope, Expression<Func<PackageSourceService, IPackageSourceStore>> storeSelector)
        {
            // Arrange
            RemovePackageSourceCommand cmd = new RemovePackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>() { CallBase = true };
            cmd.SourceService = mockService.Object;
            cmd.Scope = scope;
            InMemorySourceStore store = new InMemorySourceStore(new [] {
                new PackageSource("http://foo.bar", "Foo")
            });

            mockService.Setup(storeSelector).Returns(store);
            cmd.Name = "Foo";

            // Act
            cmd.Execute();

            // Assert
            Assert.False(store.Sources.Any());
        }

        [Theory]
        [PropertyData("ScopeSourceData")]
        public void WhenNoMatchingSourceInList_ThrowsKeyNotFoundException(PackageSourceScope scope, Expression<Func<PackageSourceService, IPackageSourceStore>> storeSelector)
        {
            // Arrange
            RemovePackageSourceCommand cmd = new RemovePackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>() { CallBase = true };
            cmd.SourceService = mockService.Object;
            cmd.Scope = scope;
            InMemorySourceStore store = new InMemorySourceStore(new[] {
                new PackageSource("http://foo.bar", "Bar")
            });

            mockService.Setup(storeSelector).Returns(store);
            cmd.Name = "Foo";

            // Act
            Assert.Throws<KeyNotFoundException>(() => cmd.Execute());

            // Assert
            Assert.Equal(new PackageSource("http://foo.bar", "Bar"), store.Sources.Single());
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

        private static Expression<Func<PackageSourceService, IPackageSourceStore>> MakeExpr(Expression<Func<PackageSourceService, IPackageSourceStore>> e)
        {
            return e;
        }
    }
}

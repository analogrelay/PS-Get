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
    public class AddPackageSourceCommandFacts
    {
        [Fact]
        public void VerifyCmdlet()
        {
            CmdletAssert.IsCmdlet(typeof(AddPackageSourceCommand), VerbsCommon.Add, "PackageSource");
        }

        [Fact]
        public void VerifyNameParameter()
        {
            CmdletAssert.IsParameter(
                () => new AddPackageSourceCommand().Name,
                new ParameterAttribute()
                {
                    Position = 0,
                    Mandatory = true,
                    ValueFromPipelineByPropertyName = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void VerifySourceParameter()
        {
            CmdletAssert.IsParameter(
                () => new AddPackageSourceCommand().Source,
                new ParameterAttribute()
                {
                    Position = 1,
                    Mandatory = true,
                    ValueFromPipelineByPropertyName = true
                },
                new ValidateNotNullOrEmptyAttribute());
        }

        [Fact]
        public void WithNoScope_AddsSourceToSessionScope()
        {
            // Arrange
            AddPackageSourceCommand cmd = new AddPackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>(MockBehavior.Strict) { CallBase = true };
            InMemorySourceStore sessionStore = new InMemorySourceStore();

            cmd.SourceService = mockService.Object;
            mockService.Setup(s => s.SessionStore).Returns(sessionStore);

            cmd.Name = "Foo";
            cmd.Source = "http://foo.bar";

            // Act
            cmd.Execute();

            // Assert
            Assert.Equal(new PackageSource("http://foo.bar", "Foo"), sessionStore.Sources.Single());
        }

        [Theory]
        [PropertyData("ScopeSourceData")]
        public void WhenScopeParameterSpecified_AddsSourceToMatchingList(PackageSourceScope scope, Expression<Func<PackageSourceService, IPackageSourceStore>> storeSelector)
        {
            // Arrange
            AddPackageSourceCommand cmd = new AddPackageSourceCommand().AutoConfigure();
            var mockService = new Mock<PackageSourceService>() { CallBase = true };
            cmd.SourceService = mockService.Object;
            cmd.Scope = scope;
            InMemorySourceStore store = new InMemorySourceStore();

            mockService.Setup(storeSelector).Returns(store);
            cmd.Source = "http://foo.bar";
            cmd.Name = "Foo";

            // Act
            cmd.Execute();

            // Assert
            Assert.Equal(new PackageSource("http://foo.bar", "Foo"), store.Sources.Single());
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

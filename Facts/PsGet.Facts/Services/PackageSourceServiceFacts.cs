using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Services;
using NuGet;
using Xunit;

namespace PsGet.Facts.Services
{
    public class PackageSourceServiceFacts
    {
        [Fact]
        public void UserSourcesOverrideMachineSources()
        {
            // Arrange
            PackageSourceService service = new PackageSourceService(
                new InMemorySourceStore(new[] {
                    new PackageSource("http://foo.bar", "Foo")
                }),
                new InMemorySourceStore(new[] {
                    new PackageSource("http://biz.baz", "Foo")
                }),
                new InMemorySourceStore());

            // Act
            PackageSource src = service.AllSources.Single();

            // Assert
            Assert.Equal(new PackageSource("http://biz.baz", "Foo"), src);
        }

        [Fact]
        public void SessionSourcesOverrideUserSources()
        {
            // Arrange
            PackageSourceService service = new PackageSourceService(
                new InMemorySourceStore(),
                new InMemorySourceStore(new[] {
                    new PackageSource("http://foo.bar", "Foo")
                }),
                new InMemorySourceStore(new[] {
                    new PackageSource("http://biz.baz", "Foo")
                }));

            // Act
            PackageSource src = service.AllSources.Single();

            // Assert
            Assert.Equal(new PackageSource("http://biz.baz", "Foo"), src);
        }
    }
}

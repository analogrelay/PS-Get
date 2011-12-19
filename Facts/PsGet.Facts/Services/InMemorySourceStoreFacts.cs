using PsGet.Services;
using Xunit;
using NuGet;
using System.Collections.Generic;

namespace PsGet.Facts.Services
{
    public class InMemorySourceStoreFacts
    {
        [Fact]
        public virtual void AddSourceAddsSourceToList()
        {
            // Arrange
            IPackageSourceStore store = new InMemorySourceStore();

            // Act
            store.AddSource("http://foo.bar", "Foo");

            // Assert
            Assert.Contains(new PackageSource("http://foo.bar", "Foo"), store.Sources);
        }

        [Fact]
        public virtual void RemoveSourceRemovesSourceFromList()
        {
            // Arrange
            IPackageSourceStore store = new InMemorySourceStore();
            store.AddSource("http://foo.bar", "Foo");

            // Act
            store.RemoveSource("Foo");

            // Assert
            Assert.DoesNotContain(new PackageSource("Foo", "http://foo.bar"), store.Sources);
        }

        [Fact]
        public virtual void RemoveSourceWithInvalidKeyThrowsKeyNotFoundException()
        {
            // Arrange
            IPackageSourceStore store = new InMemorySourceStore();

            // Act/Assert
            Assert.Throws<KeyNotFoundException>(() => store.RemoveSource("Foo"));
        }
    }
}
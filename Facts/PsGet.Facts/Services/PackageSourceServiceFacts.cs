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
            PackageSource src = service.Sources.Single();

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
            PackageSource src = service.Sources.Single();

            // Assert
            Assert.Equal(new PackageSource("http://biz.baz", "Foo"), src);
        }

        [Fact]
        public void AddSourceAddsSourceToSpecifiedStore()
        {
            // Arrange
            InMemorySourceStore machine = new InMemorySourceStore();
            InMemorySourceStore user = new InMemorySourceStore();
            InMemorySourceStore session = new InMemorySourceStore();
            PackageSourceService service = new PackageSourceService(machine, user, session);

            // Act
            service.AddSource(new PackageSource("http://machine.level", "Machine"), PackageSourceScope.Machine);
            service.AddSource(new PackageSource("http://user.level", "User"), PackageSourceScope.User);
            service.AddSource(new PackageSource("http://session.level", "Session"), PackageSourceScope.Session);

            // Assert
            Assert.Equal(new PackageSource("http://machine.level", "Machine"), machine.Sources.Single());
            Assert.Equal(new PackageSource("http://user.level", "User"), user.Sources.Single());
            Assert.Equal(new PackageSource("http://session.level", "Session"), session.Sources.Single());
        }

        [Fact]
        public void RemoveSourceRemovesSourceFromSpecifiedStore()
        {
            // Arrange
            InMemorySourceStore machine = new InMemorySourceStore(new[] { new PackageSource("http://machine.level", "Machine") });
            InMemorySourceStore user = new InMemorySourceStore(new[] { new PackageSource("http://user.level", "User") });
            InMemorySourceStore session = new InMemorySourceStore(new[] { new PackageSource("http://session.level", "Session") });
            PackageSourceService service = new PackageSourceService(machine, user, session);

            // Act
            service.RemoveSource("Machine", PackageSourceScope.Machine);
            service.RemoveSource("User", PackageSourceScope.User);
            service.RemoveSource("Session", PackageSourceScope.Session);

            // Assert
            Assert.DoesNotContain(new PackageSource("http://machine.level", "Machine"), machine.Sources);
            Assert.DoesNotContain(new PackageSource("http://user.level", "User"), user.Sources);
            Assert.DoesNotContain(new PackageSource("http://session.level", "Session"), session.Sources);
        }

        [Fact]
        public void RemoveSourceOnlyRemovesNamedSourceFromSpecifiedStore()
        {
            // Arrange
            InMemorySourceStore machine = new InMemorySourceStore(new[] { new PackageSource("http://machine.level", "Foo") });
            InMemorySourceStore user = new InMemorySourceStore(new[] { new PackageSource("http://user.level", "Foo") });
            InMemorySourceStore session = new InMemorySourceStore(new[] { new PackageSource("http://session.level", "Foo") });
            PackageSourceService service = new PackageSourceService(machine, user, session);

            // Act
            service.RemoveSource("Foo", PackageSourceScope.Session);

            // Assert
            Assert.Contains(new PackageSource("http://user.level", "Foo"), service.Sources);
            Assert.DoesNotContain(new PackageSource("http://machine.level", "Foo"), service.Sources);
            Assert.DoesNotContain(new PackageSource("http://session.level", "Foo"), service.Sources);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace PsGet.Services
{
    public interface IPackageSourceStore
    {
        IEnumerable<PackageSource> Sources { get; }
        void AddSource(PackageSource source);
        void RemoveSource(string name);
        void Save();
    }

    public static class PackageSourceStoreMixins
    {
        public static void AddSource(this IPackageSourceStore self, string source)
        {
            self.AddSource(new PackageSource(source));
        }

        public static void AddSource(this IPackageSourceStore self, string source, string name)
        {
            self.AddSource(new PackageSource(source, name));
        }
    }
}

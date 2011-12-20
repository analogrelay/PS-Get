using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using PsGet.Commands;

namespace PsGet.Services
{
    public class NullSourceStore : IPackageSourceStore
    {
        public IEnumerable<PackageSource> Sources
        {
            get { return Enumerable.Empty<PackageSource>(); }
        }

        public void AddSource(NuGet.PackageSource source)
        {
            throw new NotImplementedException("Null Package Source does not support AddSource. Have you changed the value of " + PackageSourceManagementCommand.SourceListVariable + "?");
        }

        public void RemoveSource(string name)
        {
            throw new NotImplementedException("Null Package Source does not support RemoveSource. Have you changed the value of " + PackageSourceManagementCommand.SourceListVariable + "?");
        }

        public void Save()
        {
            throw new NotImplementedException("Null Package Source does not support Save. Have you changed the value of " + PackageSourceManagementCommand.SourceListVariable + "?");
        }
    }
}

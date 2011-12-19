using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace PsGet.Services
{
    public class InMemorySourceStore : IPackageSourceStore
    {
        protected internal IDictionary<string, PackageSource> SourceList { get; private set; }

        public InMemorySourceStore() { 
            SourceList = new Dictionary<string, PackageSource>();
        }

        public InMemorySourceStore(IEnumerable<PackageSource> sources) : this()
        {
            foreach (PackageSource source in sources)
            {
                AddSource(source);
            }
        }

        public IEnumerable<PackageSource> Sources
        {
            get
            {
                return SourceList.Values;
            }
        }

        public virtual void AddSource(PackageSource source)
        {
            SourceList.Add(source.Name, source);
        }

        public virtual void RemoveSource(string name)
        {
            if (!SourceList.Remove(name))
            {
                throw new KeyNotFoundException(String.Format("Source not found: {0}", name));
            }
        }

        public virtual void Save()
        {
            // No-op for in memory store.
        }
    }
}

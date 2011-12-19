using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;
using PsGet.Services;

namespace PsGet.Commands
{
    public abstract class PackageSourceCmdlet : CommandBase
    {
        private PackageSourceService _service = null;

        internal static readonly string SourceListVariable = "PSGetSources";

        protected internal PackageSourceService SourceService
        {
            get
            {
                return _service ?? InitializeService();
            }
            internal set { _service = value; }
        }

        private PackageSourceService InitializeService()
        {
            return SourceService = new PackageSourceService(
                XmlSourceStore.CreateMachine(),
                XmlSourceStore.CreateUser(),
                GetSessionStore());
        }

        private IPackageSourceStore GetSessionStore()
        {
            object store = Session.Get(SourceListVariable);
            if (store == null)
            {
                InMemorySourceStore newStore = new InMemorySourceStore();
                Session.Set(SourceListVariable, newStore);
                return newStore;
            }
            IPackageSourceStore convertedStore = store as IPackageSourceStore;
            return convertedStore ?? new NullSourceStore();
        }
    }
}

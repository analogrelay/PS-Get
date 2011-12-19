using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;

namespace PsGet.Services
{
    public class PackageSourceNameComparer : IEqualityComparer<PackageSource>
    {
        public bool Equals(PackageSource x, PackageSource y)
        {
            return String.Equals(x.Name, y.Name);
        }

        public int GetHashCode(PackageSource obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}

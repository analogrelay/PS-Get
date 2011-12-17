using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Facts.TestDoubles
{
    // Test Doubles are tested via their use in unit tests
    [ExcludeFromCodeCoverage]
    internal class SimpleModuleMetadata : IModuleMetadata
    {
        public Version Version { get; private set; }

        public SimpleModuleMetadata(Version ver)
        {
            Version = ver;
        }
    }
}

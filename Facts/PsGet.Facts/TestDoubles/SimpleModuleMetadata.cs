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
        public string Name { get; private set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string ModuleBase { get; set; }

        public SimpleModuleMetadata(string name, Version ver)
        {
            Name = name;
            Version = ver;
        }
    }
}

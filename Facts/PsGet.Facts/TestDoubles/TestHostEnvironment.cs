using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace PsGet.Facts.TestDoubles
{
    // Test Doubles are tested via their use in unit tests
    [ExcludeFromCodeCoverage]
    public class TestHostEnvironment : IHostEnvironment
    {
        public static readonly string DefaultModuleBase = @"D:\Foo";
        public static readonly string DefaultInstallationRoot = @"D:\";

        public string ModuleBase { get; private set; }

        public TestHostEnvironment() : this(DefaultModuleBase) { }
        public TestHostEnvironment(string moduleBase)
        {
            ModuleBase = moduleBase;
        }
    }
}

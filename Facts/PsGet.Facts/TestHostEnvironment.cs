using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Hosting;

namespace PsGet.Facts
{
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

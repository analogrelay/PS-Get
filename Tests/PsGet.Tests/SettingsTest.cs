using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit.Extensions;
using Xunit;
using System.Management.Automation;

namespace PsGet.Tests {
    public class SettingsTest : TestClass {
        [Fact]
        public void HelperPath_Is_ModuleBase_Plus_HelperExeName() {
            Settings settings = new Settings(@"C:\Test");

            Assert.Equal(@"C:\Test\PsGet.Helper.exe", settings.HelperPath);
        }

        [Fact]
        public void InstallationRoot_Is_One_Folder_Up_From_Module_Base() {
            Settings settings = new Settings(@"C:\Test\Sub");

            Assert.Equal(@"C:\Test", settings.InstallationRoot);
        }

        [Fact]
        public void DefaultSource_Is_NuGet_Main_Feed() {
            Settings settings = new Settings(@"C:\Test");

            Assert.Equal("https://go.microsoft.com/fwlink/?LinkID=206669", settings.DefaultSource);
        }
    }
}

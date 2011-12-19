using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Hosting;
using NuGet;
using PsGet.Cmdlets;
using Moq;
using Xunit;
using PsGet.Facts.TestDoubles;

namespace PsGet.Facts
{
    public static class CmdletExtensions
    {
        public static T AutoConfigure<T>(this T self) where T : CommandBase
        {
            self.HostEnvironment = new TestHostEnvironment();
            self.CommandRuntime = new TestCommandRuntime();
            return self;
        }

        public static CommandOutput Execute<T>(this T self) where T : CommandBase
        {
            // Assume the configuration has already been established (properties are set, etc.) and do a
            // single item run of the pipeline
            return self.Execute(new Action<T>[] { (_) => {} });
        }

        public static CommandOutput Execute<T>(this T self, IEnumerable<Action<T>> inputConfigurations) where T : CommandBase
        {
            TestCommandRuntime runtime = new TestCommandRuntime();
            self.CommandRuntime = runtime;
            self.BeginProcessingCore();
            foreach (Action<T> configs in inputConfigurations)
            {
                configs(self);
                self.ProcessRecordCore();
            }
            self.EndProcessingCore();
            return runtime.Output;
        }

        public static TestCommandInvoker AttachInvoker(this CommandBase self)
        {
            return self.AttachInvoker(autoHandle: true);
        }

        public static TestCommandInvoker AttachInvoker(this CommandBase self, bool autoHandle)
        {
            TestCommandInvoker invoker = new TestCommandInvoker(autoHandle);
            self.Invoker = invoker;
            return invoker;
        }

        public static TestSessionStore AttachSession(this CommandBase self)
        {
            TestSessionStore store = new TestSessionStore();
            self.Session = store;
            return store;
        }

        public static Mock<IPackageManager> AttachPackageManager(this PackageManagerCmdlet self) {
            Mock<IPackageManager> mock = new Mock<IPackageManager>();
            self.PackageManagerFactory = (source, destination) =>
            {
                InstallPackageCommand cmd = self as InstallPackageCommand;
                if (cmd != null)
                {
                    Assert.Equal(cmd.Source, source);
                    Assert.Equal(cmd.Destination, destination);
                }
                return mock.Object;
            };
            return mock;
        }
    }
}

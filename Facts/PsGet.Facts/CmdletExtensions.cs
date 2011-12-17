using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using PsGet.Hosting;

namespace PsGet.Facts
{
    public static class CmdletExtensions
    {
        /// <summary>
        /// Sets up the default values for test abstractions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <returns></returns>
        public static T AutoConfigure<T>(this T self) where T : CommandBase
        {
            self.HostEnvironment = new TestHostEnvironment();
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
    }
}

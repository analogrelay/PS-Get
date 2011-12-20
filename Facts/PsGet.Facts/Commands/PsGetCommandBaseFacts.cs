using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Hosting;
using Moq;
using PsGet.Commands;

namespace PsGet.Facts.Commands
{
    public class PsGetCommandBaseFacts
    {
        [Fact]
        public void DefaultConstructorConfiguresPowerShellHosting()
        {
            TestCommand cmd = new TestCommand();

            Assert.IsType<PowerShellHostEnvironment>(cmd.HostEnvironment);
            Assert.IsType<PowerShellInvoker>(cmd.Invoker);
        }

        [Fact]
        public void BeginProcessingCallsBeginProcessingCore()
        {
            // Arrange
            var cmd = new Mock<TestCommand>() { CallBase = true };

            // Act
            cmd.Object.AutoConfigure().FireBeginProcessing();

            // Assert
            cmd.Verify(c => c.BeginProcessingCore());
        }

        [Fact]
        public void ProcessRecordCallsProcessRecordCore()
        {
            // Arrange
            var cmd = new Mock<TestCommand>() { CallBase = true };

            // Act
            cmd.Object.FireProcessRecord();

            // Assert
            cmd.Verify(c => c.ProcessRecordCore());
        }

        [Fact]
        public void EndProcessingCallsEndProcessingCore()
        {
            // Arrange
            var cmd = new Mock<TestCommand>() { CallBase = true };

            // Act
            cmd.Object.FireEndProcessing();

            // Assert
            cmd.Verify(c => c.EndProcessingCore());
        }

        [Fact]
        public void StopProcessingCallsStopProcessingCore()
        {
            // Arrange
            var cmd = new Mock<TestCommand>() { CallBase = true };

            // Act
            cmd.Object.FireStopProcessing();

            // Assert
            cmd.Verify(c => c.StopProcessingCore());
        }

        public class TestCommand : PsGetCommandBase
        {
            public void FireBeginProcessing() { BeginProcessing(); }
            public void FireProcessRecord() { ProcessRecord(); }
            public void FireEndProcessing() { EndProcessing(); }
            public void FireStopProcessing() { StopProcessing(); }
        }
    }
}

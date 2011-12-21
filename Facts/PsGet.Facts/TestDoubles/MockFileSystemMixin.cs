using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.IO;

namespace PsGet.Facts.TestDoubles
{
    public static class MockFileSystemMixin
    {
        public static void SetupTestFile(this Mock<MockableFileSystem> self, string fileName, string content)
        {
            self.Setup(e => e.FileExists(fileName)).Returns(true);
            self.Setup(e => e.OpenFile(fileName)).Returns(() =>
                new MemoryStream(Encoding.UTF8.GetBytes(content))
            );
        }
    }
}

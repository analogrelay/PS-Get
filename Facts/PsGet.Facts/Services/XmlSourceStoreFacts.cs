using System.IO;
using System.Text;
using Moq;
using NuGet;
using PsGet.Services;
using Xunit;
using PsGet.Facts.TestDoubles;

namespace PsGet.Facts.Services
{
    public class XmlSourceStoreFacts
    {
        [Fact]
        public void ConstructorWithNonExistantFileLoadsEmptyList()
        {
            // Arrange
            var mockFileSystem = new Mock<MockableFileSystem>();

            // Act
            XmlSourceStore store = new XmlSourceStore(mockFileSystem.Object, "sourceList.xml");

            // Assert
            Assert.Empty(store.Sources);
        }

        [Fact]
        public void SavingAfterAddingSourcesUpdatesFile()
        {
            const string expected = @"<?xml version=""1.0""?>
<sources xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <source name=""Foo"">http://foo.bar</source>
  <source name=""Baz"">http://biz.baz</source>
</sources>";

            // Arrange
            var mockFileSystem = new Mock<MockableFileSystem>();
            MemoryStream strm = CaptureFileContent(mockFileSystem, "sourceList.xml");
            XmlSourceStore store = new XmlSourceStore(mockFileSystem.Object, "sourceList.xml");
            store.AddSource("http://foo.bar", "Foo");
            store.AddSource("http://biz.baz", "Baz");
            
            // Act
            store.Save();

            // Assert
            Assert.Equal(expected, GetContent(strm));
        }

        [Fact]
        public void SavingAfterRemovingSourcesUpdatesFile()
        {
            const string toLoad = @"<?xml version=""1.0""?>
<sources xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <source name=""Foo"">http://foo.bar</source>
  <source name=""Baz"">http://biz.baz</source>
</sources>";

            const string expected = @"<?xml version=""1.0""?>
<sources xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <source name=""Baz"">http://biz.baz</source>
</sources>";

            // Arrange
            var mockFileSystem = new Mock<MockableFileSystem>();
            SetFileContent(mockFileSystem, "sourceList.xml", toLoad);
            XmlSourceStore store = new XmlSourceStore(mockFileSystem.Object, "sourceList.xml");
            MemoryStream strm = CaptureFileContent(mockFileSystem, "sourceList.xml");
            store.RemoveSource("Foo");

            // Act
            store.Save();

            // Assert
            Assert.Equal(expected, GetContent(strm));
        }

        [Fact]
        public void ConstructorLoadsFile()
        {
            const string toLoad = @"<?xml version=""1.0""?>
<sources xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <source name=""Foo"">http://foo.bar</source>
  <source name=""Baz"">http://biz.baz</source>
</sources>";

            // Arrange
            var mockFileSystem = new Mock<MockableFileSystem>();
            SetFileContent(mockFileSystem, "sourceList.xml", toLoad);

            // Act
            XmlSourceStore store = new XmlSourceStore(mockFileSystem.Object, "sourceList.xml");

            // Assert
            Assert.Contains(new PackageSource("http://foo.bar", "Foo"), store.Sources);
            Assert.Contains(new PackageSource("http://biz.baz", "Baz"), store.Sources);
        }

        private void SetFileContent(Mock<MockableFileSystem> mockFileSystem, string fileName, string content)
        {
            mockFileSystem.Setup(fs => fs.FileExists(fileName)).Returns(true);
            mockFileSystem.Setup(fs => fs.OpenFile(fileName)).Returns(
                () => new MemoryStream(Encoding.UTF8.GetBytes(content)));
        }

        private string GetContent(MemoryStream strm)
        {
            // Stream was closed by test, so we need to extract the array and read it
            byte[] data = strm.ToArray();
            using (StreamReader rdr = new StreamReader(new MemoryStream(data)))
            {
                return rdr.ReadToEnd();
            }
        }

        private MemoryStream CaptureFileContent(Mock<MockableFileSystem> mockFileSystem, string fileName)
        {
            MemoryStream strm = new MemoryStream();
            mockFileSystem.Setup(fs => fs.OpenFile(fileName)).Returns(strm);
            return strm;
        }
    }
}

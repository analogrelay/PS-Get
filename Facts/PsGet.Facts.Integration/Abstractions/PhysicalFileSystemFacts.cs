using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using PsGet.Abstractions;
using System.IO;

namespace PsGet.Facts.Integration.Abstractions
{
    public class PhysicalFileSystemFacts : IDisposable
    {
        private string testRoot;
        public PhysicalFileSystemFacts()
        {
            testRoot = Path.GetFullPath(@"TestOutput_PhysicalFileSystemFacts");
            if (Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
            Directory.CreateDirectory(testRoot);
            Assert.True(Directory.GetFileSystemEntries(testRoot).Length == 0, String.Format("Startup code failed to initialize '{0}' to empty directory", testRoot));
        }

        public void Dispose()
        {
            if (Directory.Exists(testRoot))
            {
                Directory.Delete(testRoot, recursive: true);
            }
        }

        [Fact]
        public void FileExistsReturnsTrueIfFileIsPresentOnDisk()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            WriteTestFile("testExists.txt");

            // Act/Assert
            Assert.True(fs.FileExists("testExists.txt"));
        }

        [Fact]
        public void FileExistsReturnsFalseIfFileIsNotPresentOnDisk()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            
            // Act/Assert
            Assert.False(fs.FileExists("testExists.txt"));
        }

        [Fact]
        public void DeleteFileDeletesPhysicalFileIfExists()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            WriteTestFile("testDelete.txt");

            // Act
            fs.DeleteFile("testDelete.txt");

            // Assert
            Assert.False(File.Exists(ResolveTestFile("testDelete.txt")));
        }

        [Fact]
        public void DeleteFileSilentlyFailsForNonExistantFile()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            
            // Act
            fs.DeleteFile("testDeleteNonExistant.txt");

            // Assert
            Assert.False(File.Exists(ResolveTestFile("testDeleteNonExistant.txt")));
        }

        [Fact]
        public void GetFilePathReturnsCombinedPathEvenIfFileDoesNotExist()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            
            // Act
            string actual = fs.GetFullPath(@"tests\testGetFullPath.txt");

            // Assert
            Assert.Equal(Path.Combine(testRoot, @"tests\testGetFullPath.txt"), actual);
        }

        [Fact]
        public void OpenFileReturnsStreamContainingAllFileData()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            WriteTestFile("testOpenRead.txt", "Foo Bar Baz");

            // Act
            Stream strm = fs.OpenFile("testOpenRead.txt");

            // Assert
            Assert.Equal("Foo Bar Baz", GetContent(strm));
        }

        [Fact]
        public void OpenFileReturnsWriteableStreamPositionedAtFileStart()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            WriteTestFile("testOpenWrite.txt", "Foo Bar Baz");
            
            // Act
            using (Stream strm = fs.OpenFile("testOpenWrite.txt"))
            {
                using (StreamWriter writer = new StreamWriter(strm))
                {
                    writer.Write("Quz");
                }
            }
            
            // Assert
            Assert.Equal("Quz Bar Baz", File.ReadAllText(ResolveTestFile("testOpenWrite.txt")));
        }

        [Fact]
        public void OpenFileLocksFileForAllAccess()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            WriteTestFile("testLock.txt", "Foo Bar Baz");

            // Act
            using (Stream strm = fs.OpenFile("testLock.txt"))
            {
                // Assert
                IOException ex = Assert.Throws<IOException>(() => fs.OpenFile("testLock.txt"));
                Assert.Equal(String.Format("The process cannot access the file '{0}' because it is being used by another process.", ResolveTestFile("testLock.txt")), ex.Message);
            }
        }

        [Fact]
        public void OpenFileCreatesParentDirectoryTreeIfNecessary()
        {
            // Arrange
            PhysicalFileSystem fs = new PhysicalFileSystem(testRoot);
            
            // Act
            using (Stream strm = fs.OpenFile(@"testParent\folder\testOpenWrite.txt")) { }

            // Assert
            Assert.True(Directory.Exists(ResolveTestFile(@"testParent\folder")));
        }

        private string ResolveTestFile(string name)
        {
            return Path.Combine(testRoot, name);
        }

        private void WriteTestFile(string name)
        {
            WriteTestFile(name, "this is a test file, delete it if you see it!");
        }

        private void WriteTestFile(string name, string content)
        {
            File.WriteAllText(ResolveTestFile(name), content);
        }

        private string GetContent(Stream strm)
        {
            strm.Seek(0, SeekOrigin.Begin);
            using (StreamReader rdr = new StreamReader(strm))
            {
                return rdr.ReadToEnd();
            }
        }
    }
}

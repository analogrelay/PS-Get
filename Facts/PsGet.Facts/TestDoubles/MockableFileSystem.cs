using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PsGet.Abstractions;
using System.IO;

namespace PsGet.Facts.TestDoubles
{
    public abstract class MockableFileSystem : IFileSystem
    {
        public abstract bool FileExists(string fileName);
        public abstract void DeleteFile(string fileName);
        public abstract Stream OpenFile(string fileName);
        public abstract string GetFullPath(string fileName);
    }
}

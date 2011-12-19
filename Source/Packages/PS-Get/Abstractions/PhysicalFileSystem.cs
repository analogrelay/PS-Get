using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PsGet.Abstractions
{
    internal class PhysicalFileSystem : IFileSystem
    {
        private string _root;

        public PhysicalFileSystem(string root)
        {
            _root = root;
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(GetFullPath(fileName));
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(GetFullPath(fileName));
        }

        public Stream OpenFile(string fileName)
        {
            return File.Open(GetFullPath(fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public string GetFullPath(string fileName)
        {
            return Path.Combine(_root, fileName);
        }
    }
}

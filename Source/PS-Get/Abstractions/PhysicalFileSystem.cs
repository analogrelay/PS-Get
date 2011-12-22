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
            string fullPath = GetFullPath(fileName); 
            string parent = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            return File.Open(fullPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public string GetFullPath(string fileName)
        {
            return Path.Combine(_root, fileName);
        }

        public IEnumerable<string> GetAllFiles()
        {
            return GetAllFiles(_root).SelectMany(s => s);
        }

        private IEnumerable<IEnumerable<string>> GetAllFiles(string dir)
        {
            foreach (var subdir in Directory.EnumerateDirectories(dir))
            {
                yield return GetAllFiles(subdir).SelectMany(s => s);
            }
            yield return Directory.EnumerateFiles(dir).Select(f => 
                (f.StartsWith(_root) ? f.Substring(_root.Length) : f).TrimStart('\\'));
        }
    }
}

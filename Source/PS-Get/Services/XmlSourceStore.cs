using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using NuGet;
using IFileSystem = PsGet.Abstractions.IFileSystem;
using PhysicalFileSystem = PsGet.Abstractions.PhysicalFileSystem;

namespace PsGet.Services
{
    public class XmlSourceStore : InMemorySourceStore
    {
        private static XmlSerializer Serializer = new XmlSerializer(typeof(PackageSourceBuilder[]), new XmlRootAttribute("sources"));

        private IFileSystem _fs;
        private string _fileName;

        public XmlSourceStore(string rootPath, string fileName) : this(new PhysicalFileSystem(rootPath), fileName)
        {

        }

        internal XmlSourceStore(IFileSystem fs, string fileName)
        {
            if (fs == null) { throw new ArgumentNullException("fs"); }
            if (String.IsNullOrEmpty(fileName)) { throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "'{0}' cannot be null or empty", "fileName"), "fileName"); }

            _fs = fs;
            _fileName = fileName;

            LoadFromFile();
        }

        public override void Save()
        {
            if (_fs.FileExists(_fileName))
            {
                _fs.DeleteFile(_fileName);
            }
            using (Stream strm = _fs.OpenFile(_fileName))
            {
                Serializer.Serialize(strm, SourceList.Values.Select(src => new PackageSourceBuilder(src)).ToArray());
            }
        }

        private void LoadFromFile()
        {
            if (!_fs.FileExists(_fileName))
            {
                return;
            }

            PackageSourceBuilder[] list;
            using (Stream strm = _fs.OpenFile(_fileName))
            {
                list = Serializer.Deserialize(strm) as PackageSourceBuilder[];
            }
            if (list == null)
            {
                throw new InvalidDataException("Source list file is invalid: " + _fs.GetFullPath(_fileName));
            }
            SourceList.AddRange(list.Select(src => new KeyValuePair<string, PackageSource>(src.Name, src.Build())));
        }

        internal static XmlSourceStore CreateMachine()
        {
            return Create(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
        }

        internal static XmlSourceStore CreateUser()
        {
            return Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

        private static XmlSourceStore Create(string dataRoot)
        {
            PhysicalFileSystem fs = new PhysicalFileSystem(Path.Combine(dataRoot, "PS-Get"));
            return new XmlSourceStore(fs, "sourceList.xml");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuGet;
using System.Xml.Serialization;

namespace PsGet.Services
{
    [XmlType("source")]
    public class PackageSourceBuilder
    {
        [XmlText]
        public string Source { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        public PackageSourceBuilder() { }
        public PackageSourceBuilder(PackageSource source)
        {
            Source = source.Source;
            Name = source.Name;
        }

        public PackageSource Build()
        {
            return new PackageSource(Source, Name);
        }
    }
}

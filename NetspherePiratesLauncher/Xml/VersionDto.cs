using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetspherePiratesLauncher.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "versions")]
    public class VersionDto
    {
        [XmlElement(ElementName = "patch")]
        public PatchDto[] Patchs { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class PatchDto
    {
        [XmlAttribute]
        public string Version { get; set; }

        [XmlElement(ElementName = "file")]
        public FileInfoDto[] Files { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class FileInfoDto
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string CRC { get; set; }
    }
}

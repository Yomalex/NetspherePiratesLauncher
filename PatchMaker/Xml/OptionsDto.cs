using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetspherePiratesLauncher.Xml
{
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "options")]
    public class OptionsDto
    {
        [XmlElement]
        public string Lang { get; set; }

        [XmlElement]
        public string Version { get; set; }

        [XmlElement]
        public string AuthIP { get; set; }

        [XmlElement]
        public string Register { get; set; }

        [XmlElement]
        public string News { get; set; }

        [XmlElement]
        public FtpInfo FTP { get; set; }
    }

    [XmlType(AnonymousType = true)]
    public class FtpInfo
    {
        [XmlElement]
        public string URL { get; set; }

        [XmlElement]
        public string User { get; set; }

        [XmlElement]
        public string Password { get; set; }
    }
}

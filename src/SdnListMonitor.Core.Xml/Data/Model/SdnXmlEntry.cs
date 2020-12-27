using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Xml.Serialization;
using static SdnListMonitor.Core.Xml.Constants;

namespace SdnListMonitor.Core.Xml.Data.Model
{
    /// <summary>
    /// Represents an entry of Specially Designated Nationals List XML file.
    /// </summary>
    [XmlRoot (ElementName = SdnXmlEntryNodeName, Namespace = SdnXmlDefaultNamespace)]
    public class SdnXmlEntry : ISdnEntry
    {
        [XmlElement (ElementName = "uid")]
        public int Uid { get; set; }

        [XmlElement (ElementName = "firstName", IsNullable = true)]
        public string FirstName { get; set; }

        [XmlElement (ElementName = "lastName")]
        public string LastName { get; set; }

        [XmlElement (ElementName = "title", IsNullable = true)]
        public string Title { get; set; }

        [XmlElement (ElementName = "sdnType")]
        public string SdnType { get; set; }

        [XmlElement (ElementName = "remarks", IsNullable = true)]
        public string Remarks { get; set; }
    }
}

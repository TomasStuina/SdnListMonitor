using SdnListMonitor.Core.Abstractions.Model;
using System.Xml.Serialization;

namespace SdnListMonitor.Core.Model.Xml
{
    /// <summary>
    /// Represents an entry of Specially Designated Nationals List XML file.
    /// </summary>
    [XmlRoot (ElementName = SdnEntryNodeName, Namespace = "http://tempuri.org/sdnList.xsd")]
    public class SdnXmlEntry : ISdnEntry
    {
        internal const string SdnEntryNodeName = "sdnEntry";

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

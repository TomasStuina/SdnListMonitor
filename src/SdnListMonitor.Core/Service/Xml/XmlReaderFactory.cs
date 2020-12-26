using SdnListMonitor.Core.Abstractions.Service.Xml;
using System.Xml;

namespace SdnListMonitor.Core.Service.Xml
{
    /// <summary>
    /// Provides a factory for instantiating <see cref="XmlReader"/>.
    /// </summary>
    public class XmlReaderFactory : IXmlReaderFactory
    {
        /// <summary>
        /// Creates a <see cref="XmlReader"/> instance for the provided
        /// URI string.
        /// </summary>
        /// <param name="inputUri">URI string to create <see cref="XmlReader"/> for.</param>
        /// <param name="xmlReaderSettings"><see cref="XmlReaderSettings"/> to customize <see cref="XmlReader"/>.</param>
        /// <returns>Created <see cref="XmlReader"/> instance.</returns>
        public XmlReader Create (string inputUri, XmlReaderSettings xmlReaderSettings) =>
            XmlReader.Create (inputUri, xmlReaderSettings);
    }
}

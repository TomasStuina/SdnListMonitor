using System.Xml;

namespace SdnListMonitor.Core.Service.Data.Xml
{
    /// <summary>
    /// Provides an interface for instantiating <see cref="XmlReader"/>.
    /// </summary>
    public interface IXmlReaderFactory
    {
        /// <summary>
        /// Creates a <see cref="XmlReader"/> instance for the provided
        /// URI string.
        /// </summary>
        /// <param name="inputUri">URI string to create <see cref="XmlReader"/> for.</param>
        /// <param name="xmlReaderSettings"><see cref="XmlReaderSettings"/> to customize <see cref="XmlReader"/>.</param>
        /// <returns>Created <see cref="XmlReader"/> instance.</returns>
        XmlReader Create (string inputUri, XmlReaderSettings xmlReaderSettings);
    }
}

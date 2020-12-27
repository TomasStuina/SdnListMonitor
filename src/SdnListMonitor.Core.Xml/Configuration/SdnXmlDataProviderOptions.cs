namespace SdnListMonitor.Core.Xml.Configuration
{
    /// <summary>
    /// Options for configuring <see cref="SdnXmlDataProvider"/>.
    /// </summary>
    public class SdnXmlDataProviderOptions
    {
        /// <summary>
        /// The path where Specially Designated Nationals List XML file is located.
        /// </summary>
        /// <remarks>
        /// This can be either a local or remote path.
        /// </remarks>
        public string XmlFilePath { get; set; }
    }
}

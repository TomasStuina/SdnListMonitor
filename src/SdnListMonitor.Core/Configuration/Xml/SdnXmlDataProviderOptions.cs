namespace SdnListMonitor.Core.Configuration.Xml
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
        /// This can be either a local path or a remote one.
        /// </remarks>
        public string XmlFilePath { get; set; }
    }
}

using Microsoft.Extensions.Options;
using SdnListMonitor.Core.Abstractions.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Configuration.Xml;
using SdnListMonitor.Core.Extensions;
using SdnListMonitor.Core.Model.Xml;
using SdnListMonitor.Core.Service.Snapshot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SdnListMonitor.Core.Service.Data.Xml
{
    /// <summary>
    /// Provides the retrieval of the Specially Designated Nationals List entries
    /// from SDN.xml file that corresponds to the format defined in SDN.xsd schema file
    /// (https://home.treasury.gov/system/files/126/sdn.xsd).
    /// </summary>
    public class SdnXmlDataProvider : ISdnDataProvider
    {
        private readonly IXmlReaderFactory m_xmlReaderFactory;
        private readonly SdnXmlDataProviderOptions m_options;
        private readonly XmlSerializer m_xmlSerializer;
        private readonly XmlReaderSettings m_xmlReaderSettings;

        private const string SdnXmlDefaultNamespace = "http://tempuri.org/sdnList.xsd";
        private const string SdnListNodeName = "sdnList";

        public SdnXmlDataProvider (IXmlReaderFactory xmlReaderFactory, IOptions<SdnXmlDataProviderOptions> options)
        {
            m_xmlReaderFactory = xmlReaderFactory.ThrowIfNull (nameof (xmlReaderFactory));
            m_options = options.ThrowIfNull (nameof (options)).Value;
            m_xmlSerializer = new XmlSerializer (typeof (SdnXmlEntry), SdnXmlDefaultNamespace);
            m_xmlReaderSettings = new XmlReaderSettings { Async = true, IgnoreWhitespace = true, IgnoreComments = true };
        }

        /// <summary>
        /// Gets all the Specially Designated Nationals List entries from the SDN.xml, located in the configured path.
        /// </summary>
        /// <remarks>
        /// The SDN.xml file is read by streaming it fragment by fragment (<sdnEntry/> node). This allows to process
        /// SDN List entries as we go, instead of loading the whole SDN XML tree to the memory and then processing it.
        /// Though, this also depends how much of the data is being buffered by the underlying stream in XML reader.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An <see cref="IAsyncEnumerable{ISdnEntry}"/> that contains all the entries.</returns>
        public async Task<ISdnDataSet> GetSdnDataAsync (CancellationToken cancellationToken = default)
        {
            var snapshot = await SortedSdnDataSet.CreateAsync (GetSdnEntriesAsync (cancellationToken));
            return snapshot;
        }

        private async IAsyncEnumerable<ISdnEntry> GetSdnEntriesAsync (CancellationToken cancellationToken = default)
        {
            using XmlReader xmlReader = m_xmlReaderFactory.Create (m_options.XmlFilePath, m_xmlReaderSettings);

            ReadToRootNode (xmlReader);

            await foreach (var sdnEntry in ReadSdnEntriesAsync (xmlReader))
                yield return sdnEntry;
        }

        private void ReadToRootNode (XmlReader xmlReader)
        {
            try
            {
            // Consider SDN.xml file malformed if the <sdnList/> root node is not present.
            if (!xmlReader.ReadToDescendant (SdnListNodeName))
                throw new InvalidOperationException (Res.ErrorWhileRetrievingSdnList);
            }
            catch (XmlException e)
            {
                // Consider SDN.xml file malformed if a XmlException is encountered while attempting to 
                // read the <sdnList/> root node.
                throw new InvalidOperationException (Res.ErrorWhileRetrievingSdnList, e);
            }
        }

        private async IAsyncEnumerable<ISdnEntry> ReadSdnEntriesAsync (XmlReader xmlReader)
        {
            while (!xmlReader.EOF)
            {
                // Skip all the nodes that are not <sdnEntry/> as we are only interested in them.
                if (!xmlReader.IsStartElement () || !string.Equals (xmlReader.Name, SdnXmlEntry.SdnEntryNodeName, StringComparison.Ordinal))
                {
                    await xmlReader.ReadAsync ().ConfigureAwait (false);
                    continue;
                }

                // Consider SDN.xml file malformed if we are unable to deserialize one of the <sdnList/> nodes.
                if (!m_xmlSerializer.CanDeserialize (xmlReader))
                    throw new InvalidOperationException (Res.ErrorWhileRetrievingSdnList);

                yield return m_xmlSerializer.Deserialize (xmlReader) as ISdnEntry;
            }
        }
    }
}

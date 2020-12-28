using Microsoft.Extensions.Options;
using SdnListMonitor.Core.Abstractions.Configuration;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Data;
using SdnListMonitor.Core.Service.Data;
using SdnListMonitor.Core.Service.Monitoring;
using SdnListMonitor.Core.Xml.Configuration;
using SdnListMonitor.Core.Xml.Service.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SdnListMonitor.ConsoleApp
{
    class Program
    {
        static async Task Main (string[] args)
        {
            var source = new CancellationTokenSource ();
            var token = source.Token;

            // Initialize comparers for the whole SDN List and individual entries:
            IComparer<ISdnEntry> dataSetEntryComparer = new AscendingByUidComparer ();
            IEqualityComparer<ISdnEntry> entryEqualityComparer = new SdnEntryEqualityComparer ();

            // Initialize a checker that is going to be used for comparing the stored SDN List with the fetched one:
            ISdnDataChangesChecker dataChangesChecker = new SdnDataSymmetryChecker (dataSetEntryComparer, entryEqualityComparer);

            // Initialize a retriever for retrieving SDN List from a local/remote location:
            var sdnXmlDataRetrieverOptions = new SdnXmlDataRetrieverOptions { XmlFilePath = @"{insert XML path here}" };
            ISdnDataRetriever dataRetriever = new SdnXmlDataRetriever (new XmlReaderFactory (), dataSetEntryComparer, Options.Create (sdnXmlDataRetrieverOptions));

            // Prefetch the SDN List and store it in memory:
            var preloadedSdnData = await dataRetriever.FetchSdnDataAsync (token).ConfigureAwait (false);
            ISdnDataPersistence dataPersistence = new InMemorySdnDataPersistence (preloadedSdnData);

            var monitoringOptions = new SdnMonitorOptions () { MonitoringInterval = TimeSpan.FromSeconds (10) };
            // Initialize a changes monitor that will be running in the background and register a callback to be executed when there are new changes:
            using var monitor = new SdnChangesMonitorService (dataChangesChecker, dataRetriever, dataPersistence, Options.Create (monitoringOptions));
            monitor.OnSdnDataChanged (OnSdnDataChanged);

            Console.WriteLine ($"MONITORING START - US - OFAC Specially Designated Nationals (SDN) List {DateTimeOffset.Now}.");
            await monitor.StartAsync (token);

            // Pressing a key will stop previously started monitor (Dispose method will be called):
            Console.ReadKey ();
        }

        private static void OnSdnDataChanged (object sender, SdnDataChangedEventArgs args)
        {
            Console.WriteLine ($"LIST UPDATED - US - OFAC Specially Designated Nationals (SDN) List {DateTimeOffset.Now}.");
            Console.WriteLine ($" {args.EntriesAdded} added, {args.EntriesModified} modified, {args.EntriesRemoved} removed");
        }
    }
}

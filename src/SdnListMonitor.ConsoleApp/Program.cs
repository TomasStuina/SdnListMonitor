using Microsoft.Extensions.Options;
using SdnListMonitor.Core.Abstractions.Configuration;
using SdnListMonitor.Core.Data;
using SdnListMonitor.Core.Service.Data;
using SdnListMonitor.Core.Service.Monitoring;
using SdnListMonitor.Core.Xml.Configuration;
using SdnListMonitor.Core.Xml.Data.Model;
using SdnListMonitor.Core.Xml.Service.Data;
using System;
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
            var entriesOrderComparer = new SdnEntryAscendingByUidComparer ();
            var entryEqualityComparer = new SdnXmlEntryEqualityComparer ();

            // Initialize a checker that is going to be used for comparing the stored SDN List with the fetched one:
            var dataChangesChecker = new SdnDataSymmetryChecker<SdnXmlEntry> (entriesOrderComparer, entryEqualityComparer);

            // Initialize a retriever for retrieving SDN List from a local/remote location:
            var sdnXmlDataRetrieverOptions = new SdnXmlDataRetrieverOptions { XmlFilePath = @"{insert SDN.XML path here}" };
            var dataRetriever = new SdnXmlDataRetriever (new XmlReaderFactory (), entriesOrderComparer, Options.Create (sdnXmlDataRetrieverOptions));

            // Prefetch the SDN List and store it in memory:
            var preloadedSdnData = await dataRetriever.FetchSdnDataAsync (token).ConfigureAwait (false);
            var dataPersistence = new InMemorySdnDataPersistence<SdnXmlEntry> (preloadedSdnData);

            var monitoringOptions = new SdnMonitorOptions () { MonitoringInterval = TimeSpan.FromMinutes (5) };
            // Initialize a changes monitor that will be running in the background and register a callback to be executed when there are new changes:

            using var monitor = new SdnChangesMonitorService<SdnXmlEntry> (dataChangesChecker, dataRetriever, dataPersistence, Options.Create (monitoringOptions));
            monitor.OnSdnDataChanged (OnSdnDataChanged);

            Console.WriteLine ($"MONITORING START - US - OFAC Specially Designated Nationals (SDN) List {DateTimeOffset.Now}.");
            await monitor.StartAsync (token);

            // Pressing a key will stop previously started monitor (Dispose method will be called, which will internally invoke StopAsync):
            Console.ReadKey ();
        }

        private static void OnSdnDataChanged (object sender, SdnDataChangedEventArgs args)
        {
            Console.WriteLine ($"LIST UPDATED - US - OFAC Specially Designated Nationals (SDN) List {DateTimeOffset.Now}.");
            Console.WriteLine ($" {args.EntriesAdded} added, {args.EntriesModified} modified, {args.EntriesRemoved} removed");
        }
    }
}

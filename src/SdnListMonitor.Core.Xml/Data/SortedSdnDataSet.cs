using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Xml.Data
{
    /// <summary>
    /// Represents a sorted <see cref="ISdnEntry"/> implementation.
    /// </summary>
    internal class SortedSdnDataSet : ISdnDataSet
    {
        private readonly SortedSet<ISdnEntry> m_sdnEntries;

        private SortedSdnDataSet (IComparer<ISdnEntry> comparer)
        {
            m_sdnEntries = new SortedSet<ISdnEntry> (comparer);
        }

        public IEnumerable<ISdnEntry> Entries => m_sdnEntries;

        public static async Task<SortedSdnDataSet> CreateAsync (IAsyncEnumerable<ISdnEntry> entries, IComparer<ISdnEntry> comparer)
        {
            var sdnDataSet = new SortedSdnDataSet (comparer);
            await foreach (var entry in entries)
                sdnDataSet.AddEntry (entry);

            return sdnDataSet;
        }

        private void AddEntry (ISdnEntry sdnEntry) => m_sdnEntries.Add (sdnEntry);
    }
}

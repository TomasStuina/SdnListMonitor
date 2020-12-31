using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Xml.Data
{
    /// <summary>
    /// Represents a sorted <see cref="ISdnDataSet{TEntry}"/> implementation.
    /// </summary>
    internal class SortedSdnDataSet<TEntry> : ISdnDataSet<TEntry> where TEntry : class, ISdnEntry
    {
        private readonly SortedSet<TEntry> m_sdnEntries;

        private SortedSdnDataSet (IComparer<TEntry> comparer)
        {
            m_sdnEntries = new SortedSet<TEntry> (comparer);
        }

        public IEnumerable<TEntry> Entries => m_sdnEntries;

        public static async Task<SortedSdnDataSet<TEntry>> CreateAsync (IAsyncEnumerable<TEntry> entries, IComparer<TEntry> comparer)
        {
            var sdnDataSet = new SortedSdnDataSet<TEntry> (comparer);
            await foreach (var entry in entries)
                sdnDataSet.AddEntry (entry);

            return sdnDataSet;
        }

        private void AddEntry (TEntry sdnEntry) => m_sdnEntries.Add (sdnEntry);
    }
}

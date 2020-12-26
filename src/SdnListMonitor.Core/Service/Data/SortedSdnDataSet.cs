using SdnListMonitor.Core.Abstractions.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Service.Snapshot
{
    public class SortedSdnDataSet : ISdnDataSet
    {
        private readonly SortedSet<ISdnEntry> m_sdnEntries;

        private SortedSdnDataSet ()
        {
            m_sdnEntries = new SortedSet<ISdnEntry> (new AscendingByUid ());
        }

        public IEnumerable<ISdnEntry> Entries => m_sdnEntries;

        public static async Task<SortedSdnDataSet> CreateAsync (IAsyncEnumerable<ISdnEntry> entries)
        {
            var snapshot = new SortedSdnDataSet ();
            await foreach (var entry in entries)
                snapshot.m_sdnEntries.Add (entry);

            return snapshot;
        }

        private class AscendingByUid : IComparer<ISdnEntry>
        {
            public int Compare (ISdnEntry x, ISdnEntry y)
            {
                return x.Uid.CompareTo (y.Uid);
            }
        }
    }
}

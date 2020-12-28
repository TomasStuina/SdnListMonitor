using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Extensions;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Service.Data
{
    public class InMemorySdnDataPersistence : ISdnDataPersistence
    {
        private readonly SortedDictionary<int, ISdnEntry> m_storedEntries;

        public InMemorySdnDataPersistence (ISdnDataSet sdnDataSet) : this ()
        {
            sdnDataSet.ThrowIfNull (nameof (sdnDataSet));
            PopulateWithEntriesFrom (sdnDataSet);
        }

        public InMemorySdnDataPersistence ()
        {
            m_storedEntries = new SortedDictionary<int, ISdnEntry> ();
        }

        public IEnumerable<ISdnEntry> Entries => m_storedEntries.Values;

        public void Add (ISdnEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            m_storedEntries.TryAdd (entry.Uid, entry);
        }

        public void Remove (ISdnEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            m_storedEntries.Remove (entry.Uid);
        }

        public void Update (ISdnEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            if (m_storedEntries.ContainsKey (entry.Uid))
                m_storedEntries[entry.Uid] = entry;
        }

        private void PopulateWithEntriesFrom (ISdnDataSet sdnDataSet)
        {
            foreach (var sdnEntry in sdnDataSet.Entries)
                m_storedEntries[sdnEntry.Uid] = sdnEntry;
        }
    }
}

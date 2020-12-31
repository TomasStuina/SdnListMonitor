using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Extensions;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Service.Data
{
    /// <summary>
    /// Provides in memory storage for <see cref="ISdnEntry"/> derivative instances.
    /// </summary>
    public class InMemorySdnDataPersistence<TEntry> : ISdnDataPersistence<TEntry> where TEntry : class, ISdnEntry
    {
        private readonly SortedDictionary<int, TEntry> m_storedEntries;

        /// <summary>
        /// Instantiates <see cref="InMemorySdnDataPersistence{TEntry}"/> with
        /// entries from the provided <see cref="ISdnDataSet{TEntry}"/> instance.
        /// </summary>
        /// <param name="sdnDataSet"><see cref="ISdnDataSet"/> instance to add entries from.</param>
        public InMemorySdnDataPersistence (ISdnDataSet<TEntry> sdnDataSet) : this ()
        {
            sdnDataSet.ThrowIfNull (nameof (sdnDataSet));
            PopulateWithEntriesFrom (sdnDataSet);
        }

        public InMemorySdnDataPersistence ()
        {
            m_storedEntries = new SortedDictionary<int, TEntry> ();
        }

        /// <summary>
        /// Returns stored <see cref="TEntry"/> instances.
        /// </summary>
        public IEnumerable<TEntry> Entries => m_storedEntries.Values;

        /// <summary>
        /// Adds <see cref="TEntry"/> instance if does not exist.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        public void Add (TEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            m_storedEntries.TryAdd (entry.Uid, entry);
        }

        /// <summary>
        /// Removes <see cref="TEntry"/> instance if it exists.
        /// </summary>
        /// <param name="entry">Entry to remove.</param>
        public void Remove (TEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            m_storedEntries.Remove (entry.Uid);
        }

        /// <summary>
        /// Updates <see cref="TEntry"/> instance if it exist.
        /// </summary>
        /// <param name="entry">Entry to update.</param>
        public void Update (TEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            if (m_storedEntries.ContainsKey (entry.Uid))
                m_storedEntries[entry.Uid] = entry;
        }

        private void PopulateWithEntriesFrom (ISdnDataSet<TEntry> sdnDataSet)
        {
            foreach (var sdnEntry in sdnDataSet.Entries)
                m_storedEntries[sdnEntry.Uid] = sdnEntry;
        }
    }
}

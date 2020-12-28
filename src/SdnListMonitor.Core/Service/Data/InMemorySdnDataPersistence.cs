using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Extensions;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Service.Data
{
    /// <summary>
    /// Provides in memory storage for <see cref="ISdnEntry"/> instances.
    /// </summary>
    public class InMemorySdnDataPersistence : ISdnDataPersistence
    {
        private readonly SortedDictionary<int, ISdnEntry> m_storedEntries;

        /// <summary>
        /// Instantiates <see cref="InMemorySdnDataPersistence"/> with
        /// entries from the provided <see cref="ISdnDataSet"/> instance.
        /// </summary>
        /// <param name="sdnDataSet"><see cref="ISdnDataSet"/> instance to add entries from.</param>
        public InMemorySdnDataPersistence (ISdnDataSet sdnDataSet) : this ()
        {
            sdnDataSet.ThrowIfNull (nameof (sdnDataSet));
            PopulateWithEntriesFrom (sdnDataSet);
        }

        public InMemorySdnDataPersistence ()
        {
            m_storedEntries = new SortedDictionary<int, ISdnEntry> ();
        }

        /// <summary>
        /// Returns stored <see cref="ISdnEntry"/> instances.
        /// </summary>
        public IEnumerable<ISdnEntry> Entries => m_storedEntries.Values;

        /// <summary>
        /// Adds <see cref="ISdnEntry"/> instance if does not exist.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        public void Add (ISdnEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            m_storedEntries.TryAdd (entry.Uid, entry);
        }

        /// <summary>
        /// Removes <see cref="ISdnEntry"/> instance if it exists.
        /// </summary>
        /// <param name="entry">Entry to remove.</param>
        public void Remove (ISdnEntry entry)
        {
            entry.ThrowIfNull (nameof (entry));
            m_storedEntries.Remove (entry.Uid);
        }

        /// <summary>
        /// Updates <see cref="ISdnEntry"/> instance if it exist.
        /// </summary>
        /// <param name="entry">Entry to update.</param>
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

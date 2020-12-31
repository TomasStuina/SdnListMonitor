using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Extensions
{
    /// <summary>
    /// Extensions for <see cref="ISdnDataPersistence"/> implementations.
    /// </summary>
    internal static class SdnDataPersistenceExtensions
    {
        /// <summary>
        /// Applies <see cref="ISdnDataChangesCheckResult{TEntry}"/> to <see cref="ISdnDataPersistence{TEntry}"/> instance.
        /// </summary>
        /// <param name="dataPersistence"><see cref="ISdnDataPersistence{TEntry}"/> to apply changes for.</param>
        /// <param name="changesCheckResult"><see cref="ISdnDataChangesCheckResult{TEntry}"/> to apply.</param>
        public static void ApplyChanges<TEntry> (this ISdnDataPersistence<TEntry> dataPersistence, ISdnDataChangesCheckResult<TEntry> changesCheckResult)
            where TEntry : class, ISdnEntry
        {
            changesCheckResult.EntriesAdded?.ForEachEntry (dataPersistence.Add);
            changesCheckResult.EntriesModified?.ForEachEntry (dataPersistence.Update);
            changesCheckResult.EntriesRemoved?.ForEachEntry (dataPersistence.Remove);
        }

        private static void ForEachEntry<TEntry> (this IEnumerable<TEntry> entries, Action<TEntry> action)
            where TEntry : class, ISdnEntry
        {
            foreach (var entry in entries)
                action.Invoke (entry);
        }
    }
}

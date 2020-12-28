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
        /// Applies <see cref="ISdnDataChangesCheckResult"/> to <see cref="ISdnDataPersistence"/> instance.
        /// </summary>
        /// <param name="dataPersistence"><see cref="ISdnDataPersistence"/> to apply changes for.</param>
        /// <param name="changesCheckResult"><see cref="ISdnDataChangesCheckResult"/> to apply.</param>
        public static void ApplyChanges (this ISdnDataPersistence dataPersistence, ISdnDataChangesCheckResult changesCheckResult)
        {
            changesCheckResult.EntriesAdded?.ForEachEntry (dataPersistence.Add);
            changesCheckResult.EntriesModified?.ForEachEntry (dataPersistence.Update);
            changesCheckResult.EntriesRemoved?.ForEachEntry (dataPersistence.Remove);
        }

        private static void ForEachEntry (this IEnumerable<ISdnEntry> entries, Action<ISdnEntry> action)
        {
            foreach (var entry in entries)
                action.Invoke (entry);
        }
    }
}

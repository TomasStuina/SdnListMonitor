using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Extensions
{
    internal static class SdnDataPersistenceExtensions
    {
        public static void ApplyChanges (this ISdnDataPersistence dataPersistence, ISdnDataChangesCheckResult changesCheckResult)
        {
            changesCheckResult.EntriesAdded?.ForEachEntry (dataPersistence.Add);
            changesCheckResult.EntriesModified?.ForEachEntry (dataPersistence.Update);
            changesCheckResult.EntriesRemoved?.ForEachEntry (dataPersistence.Remove);
        }

        private static void ForEachEntry (this IEnumerable<ISdnEntry> entries, Action<ISdnEntry> action)
        {
            foreach (var entry in entries)
                action (entry);
        }
    }
}

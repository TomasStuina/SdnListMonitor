using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// A base class for implementing <see cref="ISdnDataChangesCheckResult{TEntry}"/>.
    /// </summary>
    public abstract class SdnDataChangesCheckResultBase<TEntry> : ISdnDataChangesCheckResult<TEntry>
         where TEntry : class, ISdnEntry
    {
        /// <summary>
        /// Indicates if the data has changed.
        /// </summary>
        /// <returns>
        /// <c>true</c> - if there are entries that were added, removed, or modified.
        /// <c>false</c> - otherwise.
        /// </returns>
        public bool DataChanged => EntriesAdded.Count > 0 || EntriesRemoved.Count > 0 || EntriesModified.Count > 0;

        public abstract IReadOnlyCollection<TEntry> EntriesAdded { get; }

        public abstract IReadOnlyCollection<TEntry> EntriesRemoved { get; }

        public abstract IReadOnlyCollection<TEntry> EntriesModified { get; }
    }
}

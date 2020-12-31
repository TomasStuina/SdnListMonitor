using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Represents a comparison result in <see cref="ISdnDataChangesChecker{TEntry}"/> implementations.
    /// </summary>
    public interface ISdnDataChangesCheckResult<TEntry> where TEntry : class, ISdnEntry
    {
        /// <summary>
        /// Indicates if the data has changed.
        /// </summary>
        public bool DataChanged { get; }

        /// <summary>
        /// Entries added.
        /// </summary>
        public IReadOnlyCollection<TEntry> EntriesAdded { get; }

        /// <summary>
        /// Entries removed.
        /// </summary>
        public IReadOnlyCollection<TEntry> EntriesRemoved { get; }

        /// <summary>
        /// Entries modified.
        /// </summary>
        public IReadOnlyCollection<TEntry> EntriesModified { get; }
    }
}

using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Represents a comparison result in <see cref="ISdnDataChangesChecker"/> implementations.
    /// </summary>
    public interface ISdnDataChangesCheckResult
    {
        /// <summary>
        /// Indicates if the data has changed.
        /// </summary>
        public bool DataChanged { get; }

        /// <summary>
        /// Entries added.
        /// </summary>
        public IReadOnlyCollection<ISdnEntry> EntriesAdded { get; }

        /// <summary>
        /// Entries removed.
        /// </summary>
        public IReadOnlyCollection<ISdnEntry> EntriesRemoved { get; }

        /// <summary>
        /// Entries modified.
        /// </summary>
        public IReadOnlyCollection<ISdnEntry> EntriesModified { get; }
    }
}

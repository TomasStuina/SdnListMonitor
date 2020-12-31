using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Provides an interface for storing <see cref="ISdnEntry"/> derivative instances.
    /// </summary>
    public interface ISdnDataPersistence<TEntry> : ISdnDataSet<TEntry> where TEntry : class, ISdnEntry
    {
        /// <summary>
        /// Adds <see cref="TEntry"/> instance if does not exist.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        void Add (TEntry entry);

        /// <summary>
        /// Removes <see cref="TEntry"/> instance if it exists.
        /// </summary>
        /// <param name="entry">Entry to remove.</param>
        void Remove (TEntry entry);

        /// <summary>
        /// Updates <see cref="TEntry"/> instance if it exist.
        /// </summary>
        /// <param name="entry">Entry to update.</param>
        void Update (TEntry entry);
    }
}

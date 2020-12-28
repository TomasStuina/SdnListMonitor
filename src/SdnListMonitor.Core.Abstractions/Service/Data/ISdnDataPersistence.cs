using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Provides an interface for storing <see cref="ISdnEntry"/> instances.
    /// </summary>
    public interface ISdnDataPersistence : ISdnDataSet
    {
        /// <summary>
        /// Adds <see cref="ISdnEntry"/> instance if does not exist.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        void Add (ISdnEntry entry);

        /// <summary>
        /// Removes <see cref="ISdnEntry"/> instance if it exists.
        /// </summary>
        /// <param name="entry">Entry to remove.</param>
        void Remove (ISdnEntry entry);

        /// <summary>
        /// Updates <see cref="ISdnEntry"/> instance if it exist.
        /// </summary>
        /// <param name="entry">Entry to update.</param>
        void Update (ISdnEntry entry);
    }
}

using SdnListMonitor.Core.Abstractions.Model;
using System.Collections.Generic;
using System.Threading;

namespace SdnListMonitor.Core.Abstractions.Service
{
    /// <summary>
    /// Provides an interface for retrieving Specially Designated Nationals List entries
    /// from any data source.
    /// </summary>
    public interface ISdnDataProvider
    {
        /// <summary>
        /// Gets all the Specially Designated Nationals List entries by asynchronously enumerating
        /// them one by one.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An <see cref="IAsyncEnumerable{ISdnEntry}"/> that contains all the entries.</returns>
        IAsyncEnumerable<ISdnEntry> GetSdnEntriesAsync (CancellationToken cancellationToken = default);
    }
}

using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Provides an interface for retrieving Specially Designated Nationals List entries
    /// from any data source.
    /// </summary>
    public interface ISdnDataRetriever<TEntry> where TEntry : class, ISdnEntry
    {
        /// <summary>
        /// Gets all the Specially Designated Nationals List entries by asynchronously enumerating
        /// them one by one.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>An <see cref="IAsyncEnumerable{ISdnEntry}"/> that contains all the entries.</returns>
        Task<ISdnDataSet<TEntry>> FetchSdnDataAsync (CancellationToken cancellationToken = default);
    }
}

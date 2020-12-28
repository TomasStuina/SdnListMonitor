using SdnListMonitor.Core.Abstractions.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Provides an interface for comparing two <see cref="ISdnDataSet"/> instances.
    /// </summary>
    public interface ISdnDataChangesChecker
    {
        /// <summary>
        /// Checks for differences between two <see cref="ISdnDataSet"/> instances:
        /// <paramref name="oldDataSet"/> and <paramref name="newDataSet"/>.
        /// </summary>
        /// <remarks>
        /// The result is a difference in entries in the <paramref name="newDataSet"/> compared to the <paramref name="oldDataSet"/>.
        /// In other words, entries that were added, modified, or removed in <see cref="newDataSet"/>.
        /// </remarks>
        /// <param name="oldDataSet">The initial SDN entries data set.</param>
        /// <param name="newDataSet">The new SDN entries data set.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="Task{SdnDataChangesCheckResult}"/> indicating task completion and a comparison result.</returns>
        Task<ISdnDataChangesCheckResult> CheckForChangesAsync (ISdnDataSet oldDataSet, ISdnDataSet newDataSet, CancellationToken cancellationToken = default);
    }
}

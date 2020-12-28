using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Extensions;
using SdnListMonitor.Core.Abstractions.Service.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Service.Data
{
    /// <summary>
    /// Provides a symmetrical comparison between two <see cref="ISdnDataSet"/> instances.
    /// </summary>
    /// <remarks>
    // "Symmetrical" term here describes two SDN data sets following some sort of order, e.g.,
    // both data sets are in UID ascending order.
    /// </remarks>
    public class SdnDataSymmetryChecker : ISdnDataChangesChecker
    {
        private readonly IComparer<ISdnEntry> m_dataSetEntryComparer;
        private readonly IEqualityComparer<ISdnEntry> m_entryEqualityComparer;

        /// <summary>
        /// Instantiates <see cref="SdnDataSymmetryChecker"/>.
        /// </summary>
        /// <param name="dataSetEntryComparer">A comparer that describes the symmetry between both data sets.</param>
        /// <param name="entryEqualityComparer">
        /// An equality comparer for a deeper comparison when two entries are considered equal in a shallow comparison.
        /// </param>
        public SdnDataSymmetryChecker (IComparer<ISdnEntry> dataSetEntryComparer, IEqualityComparer<ISdnEntry> entryEqualityComparer)
        {
            m_dataSetEntryComparer = dataSetEntryComparer.ThrowIfNull (nameof (dataSetEntryComparer));
            m_entryEqualityComparer = entryEqualityComparer.ThrowIfNull (nameof (entryEqualityComparer));
        }

        /// <summary>
        /// Checks for symmetry differences between two <see cref="ISdnDataSet"/> instances:
        /// <paramref name="oldDataSet"/> and <paramref name="newDataSet"/>.
        /// </summary>
        /// <remarks>
        /// The result is a symmetry difference in entries in the <paramref name="newDataSet"/> compared to the <paramref name="oldDataSet"/>.
        /// In other words, entries that were added, modified, or removed in <see cref="newDataSet"/>.
        /// </remarks>
        /// <param name="oldDataSet">The initial SDN entries data set.</param>
        /// <param name="newDataSet">The new SDN entries data set.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns><see cref="Task{SdnDataChangesCheckResult}"/> indicating task completion and a comparison result.</returns>
        public Task<ISdnDataChangesCheckResult> CheckForChangesAsync (ISdnDataSet oldDataSet, ISdnDataSet newDataSet, CancellationToken cancellationToken = default)
        {
            var oldDataSetEnumerator = oldDataSet.ThrowIfNull (nameof (oldDataSet)).Entries.GetEnumerator ();
            var newDataSetEnumerator = newDataSet.ThrowIfNull (nameof (newDataSet)).Entries.GetEnumerator ();

            bool oldDataSetNext = oldDataSetEnumerator.MoveNext ();
            bool newDataSetNext = newDataSetEnumerator.MoveNext ();

            // Using LinkedList for O(1) Add and O(1) Count operations.
            // Also, we will only need to access them consecutively later on,
            // to save these changes in some persistence instance. 
            var entriesAdded = new LinkedList<ISdnEntry> ();
            var entriesRemoved = new LinkedList<ISdnEntry> ();
            var entriesModified = new LinkedList<ISdnEntry> ();

            // Enumerate both SDN entry sets side by side until one of the enumerators reaches the end.
            // For the sake of example, we will assume here that both SDN entry sets are compared in ascending order by the UID.
            while (oldDataSetNext && newDataSetNext)
            {
                var oldDataSetCurrent = oldDataSetEnumerator.Current;
                var newDataSetCurrent = newDataSetEnumerator.Current;
                // Perform a shallow comparison, e.g., find the difference between UIDs:
                int comparisonResult = m_dataSetEntryComparer.Compare (oldDataSetCurrent, newDataSetCurrent);
                if (comparisonResult < 0)
                {
                    // if the entry in the old data set has a lesser UID than the one in the new data set,
                    // then this entry does not exist in the new data set. For example:
                    // old data set: 1 2 3
                    // new data set: 2 3
                    entriesRemoved.AddLast (oldDataSetCurrent);
                    // Only move the enumerator of the old data set to catch up with the new data set:
                    oldDataSetNext = oldDataSetEnumerator.MoveNext ();
                }
                else if (comparisonResult == 0)
                {
                    // If both UIDs are equal, then we will consider both entries to be the same entry.
                    // However, we should perform a deeper comparison to verify that the entry in the new data set has
                    // the same properties as the one in the old data set (checking if it has not been modified).
                    // Deeper comparison details depend on the separate equality comparer.
                    if (!m_entryEqualityComparer.Equals (oldDataSetCurrent, newDataSetCurrent))
                        entriesModified.AddLast (newDataSetCurrent);

                    // Move both enumerators because the entry was neither added nor removed:
                    oldDataSetNext = oldDataSetEnumerator.MoveNext ();
                    newDataSetNext = newDataSetEnumerator.MoveNext ();
                }
                else
                {
                    // If the entry in the old data set has a greater UID than the one in the new data set,
                    // new entries were added in the beginning or the middle of the new set. For example:
                    // old data set: 1 3
                    // new data set: 1 2 3
                    // 3 (old data set) is greater than 2 (new data set), thus 2 is a new entry
                    entriesAdded.AddLast (newDataSetCurrent);
                    // Only move the enumerator of the new data set to catch up with the old data set:
                    newDataSetNext = newDataSetEnumerator.MoveNext ();
                }
            }

            // Count the rest of the entries at the end of both enumerators. For example:
            // old data set: 1 2
            // new data set: 1 2 3 4

            // If the old data set enumerator has not reached the end, add one item (the current one) as removed.
            // Move old data set enumerator to the end and add the remaining entries as removed.
            if (oldDataSetNext)
            {
                entriesRemoved.AddLast (oldDataSetEnumerator.Current);
                AddEntriesFromEnumerator (entriesRemoved, oldDataSetEnumerator);
            }

            // If the new data set enumerator has not reached the end, add one item (the current one) as added.
            // Move new data set enumerator to the end and add the remaining entries as newly added.
            if (newDataSetNext)
            {
                entriesAdded.AddLast (newDataSetEnumerator.Current);
                AddEntriesFromEnumerator (entriesAdded, newDataSetEnumerator);
            }

            return Task.FromResult<ISdnDataChangesCheckResult> (new SdnDataSymmetryCheckerResult (entriesAdded, entriesRemoved, entriesModified));
        }

        private void AddEntriesFromEnumerator (LinkedList<ISdnEntry> entries, IEnumerator<ISdnEntry> entriesEnumerator)
        {
            while (entriesEnumerator.MoveNext ())
                entries.AddLast (entriesEnumerator.Current);
        }

        private class SdnDataSymmetryCheckerResult : SdnDataChangesCheckResultBase
        {
            public SdnDataSymmetryCheckerResult (IReadOnlyCollection<ISdnEntry> added, 
                IReadOnlyCollection<ISdnEntry> removed, IReadOnlyCollection<ISdnEntry> modified)
            {
                EntriesAdded = added;
                EntriesRemoved = removed;
                EntriesModified = modified;
            }

            public override IReadOnlyCollection<ISdnEntry> EntriesAdded { get; }

            public override IReadOnlyCollection<ISdnEntry> EntriesRemoved { get; }

            public override IReadOnlyCollection<ISdnEntry> EntriesModified { get; }
        }
    }
}

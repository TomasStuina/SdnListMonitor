using SdnListMonitor.Core.Abstractions.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Service.Data
{
    public class SdnDataSymmetryChecker : ISdnDataChangesChecker
    {
        private readonly IEqualityComparer<ISdnEntry> m_entryEqualityComparer;

        public SdnDataSymmetryChecker (IEqualityComparer<ISdnEntry> entryEqualityComparer)
        {
            m_entryEqualityComparer = entryEqualityComparer.ThrowIfNull (nameof (entryEqualityComparer));
        }

        public Task<SdnDataChangesCheckResult> CheckForChangesAsync (ISdnDataSet oldDataSet, ISdnDataSet newDataSet)
        {
            var oldDataSetEnumerator = oldDataSet.ThrowIfNull (nameof (oldDataSet)).Entries.GetEnumerator ();
            var newDataSetEnumerator = newDataSet.ThrowIfNull (nameof (newDataSet)).Entries.GetEnumerator ();

            bool oldDataSetNext = oldDataSetEnumerator.MoveNext ();
            bool newDataSetNext = newDataSetEnumerator.MoveNext ();

            int entriesAdded = 0;
            int entriesRemoved = 0;
            int entriesModified = 0;

            while (oldDataSetNext && newDataSetNext)
            {
                var oldDataSetCurrent = oldDataSetEnumerator.Current;
                var newDataSetCurrent = newDataSetEnumerator.Current;

                int uidComparisonResult = oldDataSetCurrent.Uid.CompareTo (newDataSetCurrent.Uid);
                if (uidComparisonResult < 0)
                {
                    entriesRemoved++;
                    oldDataSetNext = oldDataSetEnumerator.MoveNext ();
                }
                else if (uidComparisonResult == 0)
                {
                    if (!m_entryEqualityComparer.Equals (oldDataSetCurrent, newDataSetCurrent))
                        entriesModified++;

                    oldDataSetNext = oldDataSetEnumerator.MoveNext ();
                    newDataSetNext = newDataSetEnumerator.MoveNext ();
                }
                else
                {
                    entriesAdded++;
                    newDataSetNext = newDataSetEnumerator.MoveNext ();
                }
            }

            if (oldDataSetNext)
                entriesRemoved += GetRemainingItemsCount (oldDataSetEnumerator) + 1;

            if (newDataSetNext)
                entriesAdded += GetRemainingItemsCount (newDataSetEnumerator) + 1;

            return Task.FromResult (new SdnDataChangesCheckResult (entriesAdded, entriesRemoved, entriesModified));
        }

        private int GetRemainingItemsCount (IEnumerator<ISdnEntry> entriesEnumerator)
        {
            int remainingEntriesCount = 0;
            while (entriesEnumerator.MoveNext ())
                remainingEntriesCount++;

            return remainingEntriesCount;
        }
    }
}

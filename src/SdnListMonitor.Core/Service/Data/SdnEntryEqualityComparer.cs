using SdnListMonitor.Core.Abstractions.Data.Model;
using System;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Service.Data
{
    public class SdnEntryEqualityComparer : IEqualityComparer<ISdnEntry>
    {
        public bool Equals (ISdnEntry first, ISdnEntry second)
        {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            if (!AreBothEntriesHaveEqualSingleLevelProperties (first, second))
                return false;

            return true;
        }

        public int GetHashCode (ISdnEntry entry) => entry?.Uid ?? -1;

        private bool AreBothEntriesHaveEqualSingleLevelProperties (ISdnEntry first, ISdnEntry second)
        {
            if (first.Uid != second.Uid)
                return false;

            if (!string.Equals (first.FirstName, second.FirstName, StringComparison.InvariantCulture) 
                || !string.Equals (first.LastName, second.LastName, StringComparison.InvariantCulture))
                return false;

            if (!string.Equals (first.Title, second.Title, StringComparison.InvariantCulture) 
                || !string.Equals (first.Remarks, second.Remarks, StringComparison.InvariantCulture))
                return false;

            return string.Equals (first.SdnType, second.SdnType, StringComparison.OrdinalIgnoreCase);
        }
    }
}

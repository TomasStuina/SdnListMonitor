using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Data
{
    /// <summary>
    /// Compares two <see cref="ISdnEntry"/> instances by their UID's
    /// in ascending order.
    /// </summary>
    public class AscendingByUidComparer : IComparer<ISdnEntry>
    {
        public int Compare (ISdnEntry first, ISdnEntry second)
        {
            if (first == null && second == null)
                return 0;
            else if (first != null && second == null)
                return 1;
            else if (first == null && second != null)
                return -1;
            else
                return first.Uid.CompareTo (second.Uid);
        }
    }
}

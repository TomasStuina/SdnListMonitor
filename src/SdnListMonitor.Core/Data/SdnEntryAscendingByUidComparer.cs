using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Data
{
    /// <summary>
    /// Compares two <see cref="ISdnEntry"/> instances by their UID's
    /// in ascending order.
    /// </summary>
    public class SdnEntryAscendingByUidComparer : IComparer<ISdnEntry>
    {
        /// <summary>
        /// Compares two <see cref="ISdnEntry"/> instances by their UIDs.
        /// </summary>
        /// <param name="first">Instance to compare against <paramref name="second"/>.</param>
        /// <param name="second">Instance to compare against <paramref name="first"/>.</param>
        /// <returns>
        /// <c>-1</c> - If <paramref name="first"/> is <c>null</c>, or has a smaller UID value than <paramref name="second"/>.
        /// <c>0</c> - If <paramref name="first"/> and <paramref name="second"/> are <c>null</c>, or both have the same UID value.
        /// <c>1</c> - If <paramref name="second"/> is <c>null</c>, or has a larger UID value than <paramref name="first"/>.
        /// </returns>
        public int Compare (ISdnEntry first, ISdnEntry second)
        {
            if (first is null && second is null)
                return 0;
            else if (!(first is null) && second is null)
                return 1;
            else if (first is null && !(second is null))
                return -1;
            else
                return first.Uid.CompareTo (second.Uid);
        }
    }
}

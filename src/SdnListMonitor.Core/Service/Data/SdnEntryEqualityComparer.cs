﻿using SdnListMonitor.Core.Abstractions.Data.Model;
using System;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Service.Data
{
    /// <summary>
    /// Provides an equality comparison of two <see cref="ISdnEntry"/> instances.
    /// </summary>
    public class SdnEntryEqualityComparer : IEqualityComparer<ISdnEntry>
    {
        /// <summary>
        /// Compares the equality of two <see cref="ISdnEntry"/> instances by their properties.
        /// </summary>
        /// <param name="first">The first instance to compare against the <paramref name="second"/>.</param>
        /// <param name="second">The second instance to compare against the <paramref name="first"/>.</param>
        /// <returns></returns>
        public bool Equals (ISdnEntry first, ISdnEntry second)
        {
            if (first is null && second is null)
                return true;

            if (first is null || second is null)
                return false;

            if (!HaveEqualSingleLevelProperties (first, second))
                return false;

            return true;
        }

        public int GetHashCode (ISdnEntry entry)
        {
            if (entry is null)
                return 0;

            return entry.Uid;
        }

        private bool HaveEqualSingleLevelProperties (ISdnEntry first, ISdnEntry second)
        {
            if (first.Uid != second.Uid)
                return false;

            if (!HaveSameCredentials (first, second))
                return false;

            if (!string.Equals (first.Remarks, second.Remarks, StringComparison.InvariantCulture))
                return false;

            return string.Equals (first.SdnType, second.SdnType, StringComparison.OrdinalIgnoreCase);
        }

        private bool HaveSameCredentials (ISdnEntry first, ISdnEntry second)
        {
            return string.Equals (first.FirstName, second.FirstName, StringComparison.InvariantCulture)
                && string.Equals (first.LastName, second.LastName, StringComparison.InvariantCulture)
                    && string.Equals (first.Title, second.Title, StringComparison.InvariantCulture);
        }
    }
}

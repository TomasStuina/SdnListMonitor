using System;

namespace SdnListMonitor.Core.Abstractions.Service.Monitoring
{
    /// <summary>
    /// Supplies information about a SDN data change event that is being raised.
    /// </summary>
    public class SdnDataChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The number of new entries added.
        /// </summary>
        public int EntriesAdded { get; set; }

        /// <summary>
        /// The number of entries removed.
        /// </summary>
        public int EntriesRemoved { get; set; }

        /// <summary>
        /// The number of entries modified.
        /// </summary>
        public int EntriesModified { get; set; }
    }
}

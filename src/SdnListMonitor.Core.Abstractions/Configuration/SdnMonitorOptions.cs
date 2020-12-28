using System;

namespace SdnListMonitor.Core.Abstractions.Configuration
{
    /// <summary>
    /// Options for configuring <see cref="ISdnChangesMonitorService"/>.
    /// </summary>
    public class SdnMonitorOptions
    {
        /// <summary>
        /// The interval between points when <see cref="ISdnChangesMonitorService"/> checks for changes in SDN List.
        /// The default value is 60 seconds. 
        /// </summary>
        public TimeSpan MonitoringInterval { get; set; } = TimeSpan.FromSeconds (60);
    }
}

using Microsoft.Extensions.Hosting;
using System;

namespace SdnListMonitor.Core.Abstractions.Service.Monitoring
{
    /// <summary>
    /// Provides an interface for monitoring changes in Specially Designated Nationals List.
    /// </summary>
    public interface ISdnChangesMonitorService : IHostedService
    {
        /// <summary>
        /// Registers
        /// </summary>
        /// <param name="callback"></param>
        void OnSdnDataChanged (Action<object, SdnDataChangedEventArgs> callback);
    }
}

using Microsoft.Extensions.Hosting;
using System;

namespace SdnListMonitor.Core.Abstractions.Service.Monitoring
{
    /// <summary>
    /// A base class for implementing <see cref="ISdnChangesMonitorService"/>.
    /// </summary>
    public abstract class SdnChangesMonitorServiceBase : BackgroundService, ISdnChangesMonitorService
    {
        protected Action<object, SdnDataChangedEventArgs> OnSdnDataChangedDelegate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback">Callback to register.</param>
        public void OnSdnDataChanged (Action<object, SdnDataChangedEventArgs> callback)
        {
            OnSdnDataChangedDelegate += callback;
        }
    }
}

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SdnListMonitor.Core.Abstractions.Configuration;
using SdnListMonitor.Core.Abstractions.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Abstractions.Service.Monitoring
{
    /// <summary>
    /// A base class for implementing Specially Designated Nationals List monitors.
    /// </summary>
    public abstract class SdnMonitorServiceBase : BackgroundService
    {
        private readonly int m_monitoringIntervalMilliseconds;

        protected SdnMonitorServiceBase (IOptions<SdnMonitorOptions> options)
        {
            options.ThrowIfNull (nameof (options));
            m_monitoringIntervalMilliseconds = Convert.ToInt32 (options.Value.MonitoringInterval.TotalMilliseconds);
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteMonitoringCheckAsync (stoppingToken);
                await Task.Delay (m_monitoringIntervalMilliseconds, stoppingToken);
            }
        }

        protected abstract Task ExecuteMonitoringCheckAsync (CancellationToken stoppingToken);
    }
}

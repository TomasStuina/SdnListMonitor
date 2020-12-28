﻿using Microsoft.Extensions.Options;
using SdnListMonitor.Core.Abstractions.Configuration;
using SdnListMonitor.Core.Abstractions.Extensions;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Abstractions.Service.Monitoring;
using SdnListMonitor.Core.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Service.Monitoring
{
    /// <summary>
    /// Provides Specially Designated Nationals List changes monitoring.
    /// Tracks added, removed, and modified changes.
    /// </summary>
    public class SdnChangesMonitorService : SdnMonitorServiceBase
    {
        private readonly ISdnDataChangesChecker m_dataChangesChecker;
        private readonly ISdnDataRetriever m_dataRetriever;
        private readonly ISdnDataPersistence m_dataPersistence;
        private Action<object, SdnDataChangedEventArgs> m_onSdnDataChangedDelegate;

        public SdnChangesMonitorService (ISdnDataChangesChecker dataChangesChecker, ISdnDataRetriever dataRetriever, ISdnDataPersistence dataPersistence,
            IOptions<SdnMonitorOptions> options) : base (options)
        {
            m_dataChangesChecker = dataChangesChecker.ThrowIfNull (nameof (dataChangesChecker));
            m_dataRetriever = dataRetriever.ThrowIfNull (nameof (dataRetriever));
            m_dataPersistence = dataPersistence.ThrowIfNull (nameof (dataPersistence));
        }

        /// <summary>
        /// Registers a delegate to execute when there are changes in SDN List.
        /// </summary>
        /// <param name="onSdnDataChangedDelegate">Delegate to register.</param>
        public void OnSdnDataChanged (Action<object, SdnDataChangedEventArgs> onSdnDataChangedDelegate) =>
            m_onSdnDataChangedDelegate += onSdnDataChangedDelegate.ThrowIfNull (nameof (onSdnDataChangedDelegate));

        protected override async Task ExecuteMonitoringCheckAsync (CancellationToken stoppingToken)
        {
            var retrievedSdnData = await m_dataRetriever.FetchSdnDataAsync (stoppingToken).ConfigureAwait (false);
            if (retrievedSdnData == null)
                return;
            
            var result = await m_dataChangesChecker.CheckForChangesAsync (m_dataPersistence, retrievedSdnData, stoppingToken).ConfigureAwait (false);
            if (result == null || !result.DataChanged)
                return;

            m_dataPersistence.ApplyChanges (result);
            RaiseDataChangedEvent (result);
        }

        private void RaiseDataChangedEvent (ISdnDataChangesCheckResult changesCheckresult)
        {
            m_onSdnDataChangedDelegate?.Invoke (this, new SdnDataChangedEventArgs
            {
                EntriesAdded = changesCheckresult.EntriesAdded?.Count ?? 0,
                EntriesRemoved = changesCheckresult.EntriesRemoved?.Count ?? 0,
                EntriesModified = changesCheckresult.EntriesModified?.Count ?? 0
            });
        }
    }
}

using SdnListMonitor.Core.Abstractions.Model;
using System;
using System.Threading.Tasks;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    public interface ISdnDataChangesChecker
    {
        Task<SdnDataChangesCheckResult> CheckForChangesAsync (ISdnDataSet oldDataSet, ISdnDataSet newDataSet);
    }
}

using SdnListMonitor.Core.Abstractions.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    public interface ISdnDataSet
    {
        IEnumerable<ISdnEntry> Entries { get; }
    }
}

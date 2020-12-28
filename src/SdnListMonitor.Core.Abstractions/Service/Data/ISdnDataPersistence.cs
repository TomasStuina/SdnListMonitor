using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;

namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    /// <summary>
    /// Provides an interface for storing <see cref="ISdnEntry"/> instances.
    /// </summary>
    public interface ISdnDataPersistence : ISdnDataSet
    {
        void Add (ISdnEntry entry);

        void Remove (ISdnEntry entry);

        void Update (ISdnEntry entry);
    }
}

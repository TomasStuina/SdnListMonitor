﻿using SdnListMonitor.Core.Abstractions.Data.Model;
using System.Collections.Generic;

namespace SdnListMonitor.Core.Abstractions.Data
{
    /// <summary>
    /// Provides an interface to access Specially Designated Nationals List.
    /// </summary>
    public interface ISdnDataSet
    {
        IEnumerable<ISdnEntry> Entries { get; }
    }
}

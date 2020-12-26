namespace SdnListMonitor.Core.Abstractions.Service.Data
{
    readonly public struct SdnDataChangesCheckResult
    {
        public SdnDataChangesCheckResult (int added, int removed, int modified)
        {
            DataChanged = added > 0 || removed > 0 || modified > 0;
            EntriesAdded = added;
            EntriesRemoved = removed;
            EntriesModified = modified;
        }

        public bool DataChanged { get; }

        public int EntriesAdded { get; }

        public int EntriesRemoved { get; }

        public int EntriesModified { get; }
    }
}

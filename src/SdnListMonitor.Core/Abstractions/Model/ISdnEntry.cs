namespace SdnListMonitor.Core.Abstractions.Model
{
    /// <summary>
    /// Represents an entry of Specially Designated Nationals List.
    /// </summary>
    public interface ISdnEntry
    {
        public int Uid { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Title { get; }

        public string SdnType { get; }

        public string Remarks { get; }
    }
}

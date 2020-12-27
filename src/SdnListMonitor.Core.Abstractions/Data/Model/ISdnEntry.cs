namespace SdnListMonitor.Core.Abstractions.Data.Model
{
    /// <summary>
    /// Represents an entry of Specially Designated Nationals List.
    /// </summary>
    public interface ISdnEntry
    {
        int Uid { get; }

        string FirstName { get; }

        string LastName { get; }

        string Title { get; }

        string SdnType { get; }

        string Remarks { get; }
    }
}

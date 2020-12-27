using SdnListMonitor.Core.Abstractions.Service.Data;
using Shouldly;
using Xunit;

namespace SdnListMonitor.Core.Abstractions.Tests.Service.Data
{
    public class SdnDataChangesCheckResultTests
    {
        [Theory]
        [InlineData (1, 0, 0)]
        [InlineData (0, 1, 0)]
        [InlineData (1, 0, 1)]
        public void DataChanged_WhenAnyOfThePassedValuesIsGreaterThanZero_ShouldReturnTrue (int added, int removed, int modified)
        {
            // Act & Assert
            new SdnDataChangesCheckResult (added, removed, modified).DataChanged.ShouldBeTrue ();
        }

        [Fact]
        public void DataChanged_WhenAllPassedValuesAreZero_ShouldReturnFalse ()
        {
            // Act & Assert
            new SdnDataChangesCheckResult (0, 0, 0).DataChanged.ShouldBeFalse ();
        }

        [Theory]
        [InlineData (int.MinValue)]
        [InlineData (0)]
        [InlineData (int.MaxValue)]
        public void EntriesAdded_WhenAddedValueIsPassed_ShouldReturnTheExactValue (int added)
        {
            // Act & Assert
            new SdnDataChangesCheckResult (added, 0, 0).EntriesAdded.ShouldBe (added);
        }

        [Theory]
        [InlineData (int.MinValue)]
        [InlineData (0)]
        [InlineData (int.MaxValue)]
        public void EntriesRemoved_WhenAddedValueIsPassed_ShouldReturnTheExactValue (int removed)
        {
            // Act & Assert
            new SdnDataChangesCheckResult (0, removed, 0).EntriesRemoved.ShouldBe (removed);
        }

        [Theory]
        [InlineData (int.MinValue)]
        [InlineData (0)]
        [InlineData (int.MaxValue)]
        public void EntriesModified_WhenAddedValueIsPassed_ShouldReturnTheExactValue (int modified)
        {
            // Act & Assert
            new SdnDataChangesCheckResult (0, 0, modified).EntriesModified.ShouldBe (modified);
        }
    }
}

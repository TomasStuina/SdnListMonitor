using Moq;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using Shouldly;
using Xunit;

namespace SdnListMonitor.Core.Abstractions.Tests.Service.Data
{
    public class SdnDataChangesCheckResultBaseTests
    {
        [Theory]
        [InlineData (1, 0, 0)]
        [InlineData (0, 1, 0)]
        [InlineData (1, 0, 1)]
        public void DataChanged_WhenAnyOfThePassedValuesIsGreaterThanZero_ShouldReturnTrue (int added, int removed, int modified)
        {
            // Arrange
            var result = new Mock<SdnDataChangesCheckResultBase<ISdnEntry>> ();
            result.Setup (self => self.EntriesAdded.Count).Returns (added);
            result.Setup (self => self.EntriesRemoved.Count).Returns (removed);
            result.Setup (self => self.EntriesModified.Count).Returns (modified);

            // Act & Assert
            result.Object.DataChanged.ShouldBeTrue ();
        }

        [Fact]
        public void DataChanged_WhenAllPassedValuesAreZero_ShouldReturnFalse ()
        {
            // Arrange
            var result = new Mock<SdnDataChangesCheckResultBase<ISdnEntry>> ();
            result.Setup (self => self.EntriesAdded.Count).Returns (0);
            result.Setup (self => self.EntriesRemoved.Count).Returns (0);
            result.Setup (self => self.EntriesModified.Count).Returns (0);

            // Act & Assert
            result.Object.DataChanged.ShouldBeFalse ();
        }
    }
}

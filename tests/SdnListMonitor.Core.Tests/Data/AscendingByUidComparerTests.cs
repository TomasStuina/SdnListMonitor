using Moq;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Data;
using Shouldly;
using Xunit;

namespace SdnListMonitor.Core.Tests.Data
{
    public class AscendingByUidComparerTests
    {
        private readonly AscendingByUidComparer m_ascendingByUidComparer;

        public AscendingByUidComparerTests ()
        {
            m_ascendingByUidComparer = new AscendingByUidComparer ();
        }

        [Fact]
        public void Compare_WhenFirstSdnEntryNull_ShouldReturnMinusOne ()
        {
            // Act & Assert
            m_ascendingByUidComparer.Compare (null, Mock.Of<ISdnEntry> ()).ShouldBe (-1);
        }

        [Fact]
        public void Compare_WhenBothSdnEntriesNull_ShouldReturnZero ()
        {
            // Act & Assert
            m_ascendingByUidComparer.Compare (null, null).ShouldBe (0);
        }

        [Fact]
        public void Compare_WhenSecondSdnEntryNull_ShouldReturnOne ()
        {
            // Act & Assert
            m_ascendingByUidComparer.Compare (Mock.Of<ISdnEntry> (), null).ShouldBe (1);
        }

        [Fact]
        public void Compare_WhenFirstSdnEntryHasLesserUid_ShouldReturnMinusOne ()
        {
            // Arrange
            var first = Mock.Of<ISdnEntry> (self => self.Uid == 0);
            var second = Mock.Of<ISdnEntry> (self => self.Uid == 1);

            // Act & Assert
            m_ascendingByUidComparer.Compare (first, second).ShouldBe (-1);
        }

        [Fact]
        public void Compare_WhenBothSdnEntriesHaveSameUid_ShouldReturnZero ()
        {
            // Arrange
            var first = Mock.Of<ISdnEntry> (self => self.Uid == 0);
            var second = Mock.Of<ISdnEntry> (self => self.Uid == 0);

            // Act & Assert
            m_ascendingByUidComparer.Compare (first, second).ShouldBe (0);
        }

        [Fact]
        public void Compare_WhenFirstSdnEntryHasGreaterUid_ShouldReturnOne ()
        {
            // Arrange
            var first = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var second = Mock.Of<ISdnEntry> (self => self.Uid == 0);

            // Act & Assert
            m_ascendingByUidComparer.Compare (first, second).ShouldBe (1);
        }
    }
}

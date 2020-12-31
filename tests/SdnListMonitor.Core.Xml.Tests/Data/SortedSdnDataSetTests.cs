using Moq;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Xml.Data;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SdnListMonitor.Core.Xml.Tests.Data
{
    public class SortedSdnDataSetTests
    {
        [Fact]
        public async Task CreateAsync_WhenEmptyAsyncEnumerablePassed_ShouldReturnSdnDataWithoutEntries ()
        {
            // Act
            var sdnDataSet = await SortedSdnDataSet<ISdnEntry>.CreateAsync (AsyncEnumerable.Empty<ISdnEntry> (), Mock.Of <IComparer<ISdnEntry>> ());

            // Assert
            sdnDataSet.Entries.ShouldBeEmpty ();
        }

        [Fact]
        public async Task CreateAsync_WhenAscendingUidComparerPassed_ShouldReturnSdnDataWithEntriesInUidAscendingOrder ()
        {
            // Arrange
            var entries = new[]
            {
                Mock.Of<ISdnEntry> (self => self.Uid == 1),
                Mock.Of<ISdnEntry> (self => self.Uid == 2),
                Mock.Of<ISdnEntry> (self => self.Uid == 0) 
            };

            var comparer = new Mock<IComparer<ISdnEntry>> ();
            comparer.Setup (self => self.Compare (It.IsAny<ISdnEntry> (), It.IsAny<ISdnEntry> ()))
                    .Returns<ISdnEntry, ISdnEntry> ((x, y) => x.Uid.CompareTo (y.Uid));

            // Act
            var sdnDataSet = await SortedSdnDataSet<ISdnEntry>.CreateAsync (CreateSdnEntriesAsyncEnumerable (entries), comparer.Object);

            // Assert
            var sortedEntries = sdnDataSet.Entries.ToArray ();
            sortedEntries.Length.ShouldBe (entries.Length);
            sortedEntries[0].Uid.ShouldBe (0);
            sortedEntries[1].Uid.ShouldBe (1);
            sortedEntries[2].Uid.ShouldBe (2);
        }

        [Fact]
        public async Task CreateAsync_WhenDescendingUidComparerPassed_ShouldReturnSdnDataWithEntriesInUidDescendingOrder ()
        {
            // Arrange
            var entries = new[]
            {
                Mock.Of<ISdnEntry> (self => self.Uid == 1),
                Mock.Of<ISdnEntry> (self => self.Uid == 2),
                Mock.Of<ISdnEntry> (self => self.Uid == 0)
            };

            var comparer = new Mock<IComparer<ISdnEntry>> ();
            comparer.Setup (self => self.Compare (It.IsAny<ISdnEntry> (), It.IsAny<ISdnEntry> ()))
                    .Returns<ISdnEntry, ISdnEntry> ((x, y) => y.Uid.CompareTo (x.Uid));

            // Act
            var sdnDataSet = await SortedSdnDataSet<ISdnEntry>.CreateAsync (CreateSdnEntriesAsyncEnumerable (entries), comparer.Object);

            // Assert
            var sortedEntries = sdnDataSet.Entries.ToArray ();
            sortedEntries.Length.ShouldBe (entries.Length);
            sortedEntries[0].Uid.ShouldBe (2);
            sortedEntries[1].Uid.ShouldBe (1);
            sortedEntries[2].Uid.ShouldBe (0);
        }

        private static async IAsyncEnumerable<ISdnEntry> CreateSdnEntriesAsyncEnumerable (params ISdnEntry[] entries)
        {
            foreach (var entry in entries)
                yield return await Task.FromResult (entry);
        }
    }
}

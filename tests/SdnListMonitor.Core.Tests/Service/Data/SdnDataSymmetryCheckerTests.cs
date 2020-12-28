using Moq;
using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Data;
using SdnListMonitor.Core.Service.Data;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SdnListMonitor.Core.Tests.Service.Data
{
    public class SdnDataSymmetryCheckerTests
    {
        private IComparer<ISdnEntry> m_dataSetEntryComparer;

        public SdnDataSymmetryCheckerTests ()
        {
            m_dataSetEntryComparer = new AscendingByUidComparer ();
        }

        [Fact]
        public void Ctor_WhenDataSetEntryComparerNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new SdnDataSymmetryChecker (null, Mock.Of<IEqualityComparer<ISdnEntry>> ()))
                  .ParamName
                  .ShouldBe ("dataSetEntryComparer");
        }

        [Fact]
        public void Ctor_WhenEntryEqualityComparerNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new SdnDataSymmetryChecker (m_dataSetEntryComparer, null))
                  .ParamName
                  .ShouldBe ("entryEqualityComparer");
        }

        [Fact]
        public void CheckForChangesAsync_WhenOldDataSetNull_ShouldShouldThrowArgumentNullException ()
        {
            // Arrange
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act & Assert
            Should.Throw<ArgumentNullException> (async () =>
                await sdnDataSymmetryChecker.CheckForChangesAsync (null, Mock.Of<ISdnDataSet> ()))
                  .ParamName
                  .ShouldBe ("oldDataSet");
        }

        [Fact]
        public void CheckForChangesAsync_WhenNewDataSetNull_ShouldShouldThrowArgumentNullException ()
        {
            // Arrange
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);

            // Act & Assert
            Should.Throw<ArgumentNullException> (async () =>
                await sdnDataSymmetryChecker.CheckForChangesAsync (Mock.Of<ISdnDataSet> (), null))
                  .ParamName
                  .ShouldBe ("newDataSet");
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenBothDataSetsEmpty_ShouldReturnDataUnchangedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet ();
            var newDataSet = CreateSdnDataSet ();
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeFalse ();
            result.EntriesAdded.ShouldBeEmpty ();
            result.EntriesRemoved.ShouldBeEmpty ();
            result.EntriesModified.ShouldBeEmpty ();
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidAdded_ShouldReturnAddedResult ()
        {
            // Arrange
            var entryAdded = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet ();
            var newDataSet = CreateSdnDataSet (entryAdded);
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidAddedAtTheStart_ShouldReturnAddedResult ()
        {
            // Arrange
            var entryAdded = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 2 });
            var newDataSet = CreateSdnDataSet (entryAdded, new TestEntry { Uid = 2 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidAddedInBetween_ShouldReturnAddedResult ()
        {
            // Arrange
            var entryAdded = new TestEntry { Uid = 2 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, new TestEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryAdded, new TestEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithNewUidAddedAtTheEnd_ShouldReturnAddedResult ()
        {
            // Arrange
            var firstEntryAdded = new TestEntry { Uid = 2 };
            var secondEntryAdded = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 });
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, firstEntryAdded, secondEntryAdded);
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { firstEntryAdded, secondEntryAdded });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithUidRemoved_ShouldReturnRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryRemoved);
            var newDataSet = CreateSdnDataSet ();
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithUidRemovedAtTheStart_ShouldReturnRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryRemoved, new TestEntry { Uid = 2 });
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 2 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithUidRemovedInBetween_ShouldReturnRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 2 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryRemoved, new TestEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, new TestEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithUidRemovedAtTheEnd_ShouldReturnRemovedResult ()
        {
            // Arrange
            var firstEntryRemoved = new TestEntry { Uid = 2 };
            var secondEntryRemoved = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, firstEntryRemoved, secondEntryRemoved);
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (new[] { firstEntryRemoved, secondEntryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidAreNotEqual_ShouldReturnModifiedResult ()
        {
            // Arrange
            var entryFromOld = new TestEntry { Uid = 1 };
            var entryFromNew = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryFromOld);
            var newDataSet = CreateSdnDataSet (entryFromNew);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (false);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesModified.ShouldBe (new[] { entryFromNew });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidAtTheStartAreNotEqual_ShouldReturnModifiedResult ()
        {
            // Arrange
            var entryFromOld = new TestEntry { Uid = 1 };
            var entryFromNew = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryFromOld, new TestEntry { Uid = 2 });
            var newDataSet = CreateSdnDataSet (entryFromNew, new TestEntry { Uid = 3 });
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (false);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesModified.ShouldBe (new[] { entryFromNew });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidInBetweenAreNotEqual_ShouldReturnModifiedResult ()
        {
            // Arrange
            var entryFromOld = new TestEntry { Uid = 3 };
            var entryFromNew = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryFromOld, new TestEntry { Uid = 4 });
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 2 }, entryFromNew, new TestEntry { Uid = 5 });
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (false);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesModified.ShouldBe (new[] { entryFromNew });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidAtTheEndAreNotEqual_ShouldReturnModifiedResult ()
        {
            // Arrange
            var entryFromOld = new TestEntry { Uid = 3 };
            var entryFromNew = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryFromOld);
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 2 }, entryFromNew);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (false);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesModified.ShouldBe (new[] { entryFromNew });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidInDifferentPositionsAreNotEqual_ShouldReturnModifiedResult ()
        {
            // Arrange
            var entryFromOld = new TestEntry { Uid = 4 };
            var entryFromNew = new TestEntry { Uid = 4 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryFromOld);
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 2 }, new TestEntry { Uid = 3 }, entryFromNew);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (false);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesModified.ShouldBe (new[] { entryFromNew });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidAreEqual_ShouldReturnDataChangedFalse ()
        {
            // Arrange
            var entryFromOld = new TestEntry { Uid = 1 };
            var entryFromNew = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryFromOld);
            var newDataSet = CreateSdnDataSet (entryFromNew);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (true);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeFalse ();
            result.EntriesModified.ShouldBeEmpty ();
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidReplacesAnotherOne_ShouldReturnAddedAndRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 2 };
            var entryAdded = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryRemoved);
            var newDataSet = CreateSdnDataSet (entryAdded);
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidReplacesOneAtTheStart_ShouldReturnAddedAndRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 2 };
            var entryAdded = new TestEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryRemoved, new TestEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (entryAdded, new TestEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidReplacesOneInBetween_ShouldReturnAddedAndRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 2 };
            var entryAdded = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryRemoved, new TestEntry { Uid = 4 });
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryAdded, new TestEntry { Uid = 4 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidReplacesOneAtTheEnd_ShouldReturnAddedAndRemovedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 2 };
            var entryAdded = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryRemoved);
            var newDataSet = CreateSdnDataSet (new TestEntry { Uid = 1 }, entryAdded);
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer);;

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenNewSdnDataHasAddedRemovedAndModifiedEntries_ShouldReturnAddedRemovedAndModifiedResult ()
        {
            // Arrange
            var entryRemoved = new TestEntry { Uid = 1 };
            var entryModifed = new TestEntry { Uid = 2 };
            var entryAdded = new TestEntry { Uid = 3 };
            var oldDataSet = CreateSdnDataSet (entryRemoved, entryModifed);
            var newDataSet = CreateSdnDataSet (entryModifed, entryAdded);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (It.IsAny<ISdnEntry> (), It.IsAny<ISdnEntry> ())).Returns (false);
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (m_dataSetEntryComparer, entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (new[] { entryAdded });
            result.EntriesRemoved.ShouldBe (new[] { entryRemoved });
            result.EntriesModified.ShouldBe (new[] { entryModifed });
        }

        private static ISdnDataSet CreateSdnDataSet (params ISdnEntry[] sdnEntries) =>
            Mock.Of<ISdnDataSet> (self => self.Entries == sdnEntries);

        private class TestEntry : ISdnEntry
        {
            public int Uid { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Title { get; set; }
            public string SdnType { get; set; }
            public string Remarks { get; set; }
        }
    }
}

using Moq;
using SdnListMonitor.Core.Abstractions.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Model.Xml;
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
        [Fact]
        public void Ctor_WhenEntryEqualityComparerNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new SdnDataSymmetryChecker (null))
                  .ParamName
                  .ShouldBe ("entryEqualityComparer");
        }

        [Fact]
        public void CheckForChangesAsync_WhenOldDataSetNull_ShouldShouldThrowArgumentNullException ()
        {
            // Arrange
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

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
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

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
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeFalse ();
            result.EntriesAdded.ShouldBe (0);
            result.EntriesRemoved.ShouldBe (0);
            result.EntriesModified.ShouldBe (0);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidAdded_ShouldReturnAddedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet ();
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidAddedAtTheStart_ShouldReturnAddedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 2 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidAddedInBetween_ShouldReturnAddedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 }, new SdnXmlEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithNewUidAddedAtTheEnd_ShouldReturnAddedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 }, new SdnXmlEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (2);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithUidRemoved_ShouldReturnRemovedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 });
            var newDataSet = CreateSdnDataSet ();
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithUidRemovedAtTheStart_ShouldReturnRemovedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 2 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithUidRemovedInBetween_ShouldReturnRemovedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 }, new SdnXmlEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidAreNotEqual_ShouldReturnModifiedResult ()
        {
            // Arrange
            var entryFromOld = new SdnXmlEntry { Uid = 1 };
            var entryFromNew = new SdnXmlEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryFromOld);
            var newDataSet = CreateSdnDataSet (entryFromNew);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (false);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesModified.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithSameUidAreEqual_ShouldReturnDataChangedFalse ()
        {
            // Arrange
            var entryFromOld = new SdnXmlEntry { Uid = 1 };
            var entryFromNew = new SdnXmlEntry { Uid = 1 };
            var oldDataSet = CreateSdnDataSet (entryFromOld);
            var newDataSet = CreateSdnDataSet (entryFromNew);
            var entryEqualityComparer = new Mock<IEqualityComparer<ISdnEntry>> ();
            entryEqualityComparer.Setup (self => self.Equals (entryFromOld, entryFromNew)).Returns (true);

            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer.Object);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeFalse ();
            result.EntriesModified.ShouldBe (0);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntriesWithUidRemovedAtTheEnd_ShouldReturnRemovedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 }, new SdnXmlEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesRemoved.ShouldBe (2);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidReplacesOneAtTheStart_ShouldReturnAddedAndRemovedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 2 }, new SdnXmlEntry { Uid = 3 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 3 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (1);
            result.EntriesRemoved.ShouldBe (1);
        }

        [Fact]
        public async Task CheckForChangesAsync_WhenEntryWithNewUidReplacesOneInBetween_ShouldReturnAddedAndRemovedResult ()
        {
            // Arrange
            var oldDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 2 }, new SdnXmlEntry { Uid = 4 });
            var newDataSet = CreateSdnDataSet (new SdnXmlEntry { Uid = 1 }, new SdnXmlEntry { Uid = 3 }, new SdnXmlEntry { Uid = 4 });
            var entryEqualityComparer = Mock.Of<IEqualityComparer<ISdnEntry>> ();
            var sdnDataSymmetryChecker = new SdnDataSymmetryChecker (entryEqualityComparer);

            // Act
            var result = await sdnDataSymmetryChecker.CheckForChangesAsync (oldDataSet, newDataSet);

            // Assert
            result.DataChanged.ShouldBeTrue ();
            result.EntriesAdded.ShouldBe (1);
            result.EntriesRemoved.ShouldBe (1);
        }

        private static ISdnDataSet CreateSdnDataSet (params ISdnEntry[] sdnEntries) =>
            Mock.Of<ISdnDataSet> (self => self.Entries == sdnEntries);
    }
}

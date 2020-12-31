using Moq;
using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Service.Data;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace SdnListMonitor.Core.Tests.Service.Data
{
    public class InMemorySdnDataPersistenceTests
    {
        [Fact]
        public void Ctor_WhenSdnDataSetNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new InMemorySdnDataPersistence<ISdnEntry> (null))
                  .ParamName
                  .ShouldBe ("sdnDataSet");
        }

        [Fact]
        public void Ctor_WhenEmptySdnDataSetProvided_ShouldHaveNoEntries ()
        {
            // Arrange
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == Enumerable.Empty<ISdnEntry> ());

            // Act & Assert
            new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet).Entries.ShouldBeEmpty ();
        }

        [Fact]
        public void Ctor_WhenSdnDataSetWithEntriesProvided_ShouldHaveTheseEntriesAdded ()
        {
            // Arrange
            var entries = new[]
            {
                Mock.Of<ISdnEntry> (self => self.Uid == 0),
                Mock.Of<ISdnEntry> (self => self.Uid == 1),
                Mock.Of<ISdnEntry> (self => self.Uid == 2),
            };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == entries);

            // Act
            var persistenceEntries = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet).Entries;

            // Assert
            persistenceEntries.ShouldBe (entries);
        }

        [Fact]
        public void Ctor_WhenSdnDataSetWithUnorderedEntriesProvided_ShouldHaveTheseEntriesSortedByUid ()
        {
            // Arrange
            var entries = new[]
            {
                Mock.Of<ISdnEntry> (self => self.Uid == 1),
                Mock.Of<ISdnEntry> (self => self.Uid == 2),
                Mock.Of<ISdnEntry> (self => self.Uid == 0),
            };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == entries);

            // Act
            var persistenceEntries = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet).Entries.ToArray ();

            // Assert
            persistenceEntries.Length.ShouldBe (3);
            persistenceEntries[0].Uid.ShouldBe (0);
            persistenceEntries[1].Uid.ShouldBe (1);
            persistenceEntries[2].Uid.ShouldBe (2);
        }

        [Fact]
        public void Ctor_WhenNoSdnDataSetProvided_ShouldHaveNoEntries ()
        {
            // Act & Assert
            new InMemorySdnDataPersistence<ISdnEntry> ().Entries.ShouldBeEmpty ();
        }

        [Fact]
        public void Add_WhenEntryNull_ShoulThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new InMemorySdnDataPersistence<ISdnEntry> ().Add (null))
                  .ParamName
                  .ShouldBe ("entry");
        }

        [Fact]
        public void Add_WhenEntryWithNewUidPassed_ShoulAddIt ()
        {
            // Arrange
            var newEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntry = Mock.Of<ISdnEntry> (self => self.Uid == 0);
            var currentEntries = new[] { currentEntry };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Add (newEntry);

            // Assert
            persistence.Entries.ShouldBe (new[] { currentEntry, newEntry });
        }

        [Fact]
        public void Add_WhenEntryWithNewUidHasLesserUidThanLastEntry_ShoulAddItSorted ()
        {
            // Arrange
            var newEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntryFirst = Mock.Of<ISdnEntry> (self => self.Uid == 0);
            var currentEntryLast = Mock.Of<ISdnEntry> (self => self.Uid == 2);
            var currentEntries = new[] { currentEntryFirst, currentEntryLast };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Add (newEntry);

            // Assert
            persistence.Entries.ShouldBe (new[] { currentEntryFirst, newEntry, currentEntryLast });
        }

        [Fact]
        public void Add_WhenEntryWithTheSameUidAlreadysExists_ShoulNotAddIt ()
        {
            // Arrange
            var newEntry = Mock.Of<ISdnEntry> (self => self.Uid == 0);
            var currentEntries = new[] { Mock.Of<ISdnEntry> (self => self.Uid == 0) };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Add (newEntry);

            // Assert
            persistence.Entries.ShouldBe (currentEntries);
        }

        [Fact]
        public void Remove_WhenEntryNull_ShoulThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new InMemorySdnDataPersistence<ISdnEntry> ().Remove (null))
                  .ParamName
                  .ShouldBe ("entry");
        }

        [Fact]
        public void Remove_WhenEntryWithExistingUidPassed_ShoulRemoveIt ()
        {
            // Arrange
            var entryToRemove = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntries = new[] { currentEntry };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Remove (entryToRemove);

            // Assert
            persistence.Entries.ShouldBeEmpty ();
        }

        [Fact]
        public void Remove_WhenEntryWithNewUidPassed_ShoulKeepEntriesAsTheyAre ()
        {
            // Arrange
            var entryToRemove = Mock.Of<ISdnEntry> (self => self.Uid == 2);
            var currentEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntries = new[] { currentEntry };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Remove (entryToRemove);

            // Assert
            persistence.Entries.ShouldBe (currentEntries);
        }

        [Fact]
        public void Update_WhenEntryNull_ShoulThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new InMemorySdnDataPersistence<ISdnEntry> ().Update (null))
                  .ParamName
                  .ShouldBe ("entry");
        }

        [Fact]
        public void Update_WhenEntryWithExistingUidPassed_ShoulReplaceTheCurrentOne ()
        {
            // Arrange
            var newEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntries = new[] { currentEntry };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Update (newEntry);

            // Assert
            persistence.Entries.ShouldBe (new[] { newEntry });
        }

        [Fact]
        public void Update_WhenEntryWithNewUidPassed_ShoulKeepEntriesAsTheyAre ()
        {
            // Arrange
            var newEntry = Mock.Of<ISdnEntry> (self => self.Uid == 2);
            var currentEntry = Mock.Of<ISdnEntry> (self => self.Uid == 1);
            var currentEntries = new[] { currentEntry };
            var sdnDataSet = Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == currentEntries);
            var persistence = new InMemorySdnDataPersistence<ISdnEntry> (sdnDataSet);

            // Act
            persistence.Update (newEntry);

            // Assert
            persistence.Entries.ShouldBe (currentEntries);
        }
    }
}

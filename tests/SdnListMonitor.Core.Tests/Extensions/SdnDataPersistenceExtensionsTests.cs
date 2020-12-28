using Moq;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Extensions;
using System.Collections.Generic;
using Xunit;

namespace SdnListMonitor.Core.Tests.Extensions
{
    public class SdnDataPersistenceExtensionsTests
    {
        [Fact]
        public void ApplyChanges_WhenChangeResultHasAddedEntries_ShouldInvokeAdd ()
        {
            // Arrange
            var firstEntry = Mock.Of<ISdnEntry> ();
            var secondEntry = Mock.Of<ISdnEntry> ();
            var entriesAdded = new List<ISdnEntry> { firstEntry, secondEntry };
            var changesCheckResult = Mock.Of<ISdnDataChangesCheckResult> (self =>
                self.EntriesAdded == entriesAdded.AsReadOnly ());

            var persistence = new Mock<ISdnDataPersistence> ();

            // Act
            persistence.Object.ApplyChanges (changesCheckResult);

            // Assert
            persistence.Verify (self => self.Add (firstEntry), Times.Once);
            persistence.Verify (self => self.Add (secondEntry), Times.Once);
        }

        [Fact]
        public void ApplyChanges_WhenChangeResultHasRemovedEntries_ShouldInvokeRemove ()
        {
            // Arrange
            var firstEntry = Mock.Of<ISdnEntry> ();
            var secondEntry = Mock.Of<ISdnEntry> ();
            var entriesRemoved = new List<ISdnEntry> { firstEntry, secondEntry };
            var changesCheckResult = Mock.Of<ISdnDataChangesCheckResult> (self =>
                self.EntriesRemoved == entriesRemoved.AsReadOnly ());

            var persistence = new Mock<ISdnDataPersistence> ();

            // Act
            persistence.Object.ApplyChanges (changesCheckResult);

            // Assert
            persistence.Verify (self => self.Remove (firstEntry), Times.Once);
            persistence.Verify (self => self.Remove (secondEntry), Times.Once);
        }

        [Fact]
        public void ApplyChanges_WhenChangeResultHasModifiedEntries_ShouldInvokeUpdate ()
        {
            // Arrange
            var firstEntry = Mock.Of<ISdnEntry> ();
            var secondEntry = Mock.Of<ISdnEntry> ();
            var entriesModified = new List<ISdnEntry> { firstEntry, secondEntry };
            var changesCheckResult = Mock.Of<ISdnDataChangesCheckResult> (self =>
                self.EntriesModified == entriesModified.AsReadOnly ());

            var persistence = new Mock<ISdnDataPersistence> ();

            // Act
            persistence.Object.ApplyChanges (changesCheckResult);

            // Assert
            persistence.Verify (self => self.Update (firstEntry), Times.Once);
            persistence.Verify (self => self.Update (secondEntry), Times.Once);
        }
    }
}

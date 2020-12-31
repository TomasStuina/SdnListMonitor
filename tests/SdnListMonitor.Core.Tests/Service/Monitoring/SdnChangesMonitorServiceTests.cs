using Microsoft.Extensions.Options;
using Moq;
using SdnListMonitor.Core.Abstractions.Configuration;
using SdnListMonitor.Core.Abstractions.Data;
using SdnListMonitor.Core.Abstractions.Data.Model;
using SdnListMonitor.Core.Abstractions.Service.Data;
using SdnListMonitor.Core.Service.Monitoring;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SdnListMonitor.Core.Tests.Service.Monitoring
{
    public class SdnChangesMonitorServiceTests
    {
        private readonly Mock<ISdnDataChangesChecker<ISdnEntry>> m_dataChangesChecker;
        private readonly Mock<ISdnDataRetriever<ISdnEntry>> m_dataRetriever;
        private readonly Mock<ISdnDataPersistence<ISdnEntry>> m_dataPersistence;
        private readonly IOptions<SdnMonitorOptions> m_options;

        public SdnChangesMonitorServiceTests ()
        {
            m_dataChangesChecker = new Mock<ISdnDataChangesChecker<ISdnEntry>> ();
            m_dataRetriever = new Mock<ISdnDataRetriever<ISdnEntry>> ();
            m_dataPersistence = new Mock<ISdnDataPersistence<ISdnEntry>> ();
            m_options = Options.Create (new SdnMonitorOptions ());
        }

        [Fact]
        public void Ctor_WhenDataChangesCheckerNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() =>
                new SdnChangesMonitorService<ISdnEntry> (null, m_dataRetriever.Object, m_dataPersistence.Object, m_options))
                  .ParamName
                  .ShouldBe ("dataChangesChecker");
        }

        [Fact]
        public void Ctor_WhenDataRetrieverNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() =>
                new SdnChangesMonitorService<ISdnEntry> (m_dataChangesChecker.Object, null, m_dataPersistence.Object, m_options))
                  .ParamName
                  .ShouldBe ("dataRetriever");
        }

        [Fact]
        public void Ctor_WhenDataPersistenceNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() =>
                new SdnChangesMonitorService<ISdnEntry> (m_dataChangesChecker.Object, m_dataRetriever.Object, null, m_options))
                  .ParamName
                  .ShouldBe ("dataPersistence");
        }

        [Fact]
        public void Ctor_WhenOptionsNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() =>
                new SdnChangesMonitorService<ISdnEntry> (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, null))
                  .ParamName
                  .ShouldBe ("options");
        }

        [Fact]
        public void OnSdnDataChanged_WhenOnSdnDataChangedDelegateNull_ShouldThrowArgumentNullException ()
        {
            // Arrange
            var monitorService = new SdnChangesMonitorService<ISdnEntry> (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            // Act & Assert
            Should.Throw<ArgumentNullException> (() => monitorService.OnSdnDataChanged (null))
                  .ParamName
                  .ShouldBe ("onSdnDataChangedDelegate");
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenInvoked_ShouldFetchSdnData ()
        {
            // Arrange
            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            m_dataRetriever.Verify (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ()), Times.Once);
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenFetchSdnDataAsyncReturnsNull_ShouldNotCheckForChanges ()
        {
            // Arrange
            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            m_dataChangesChecker.Verify (self => 
                self.CheckForChangesAsync (
                    It.IsAny<ISdnDataSet<ISdnEntry>> (), 
                    It.IsAny<ISdnDataSet<ISdnEntry>> (), 
                    It.IsAny<CancellationToken> ()), 
                Times.Never);
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenFetchSdnDataAsyncReturnsSdnDataSet_ShouldCheckForChanges ()
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            m_dataChangesChecker.Verify (self =>
                self.CheckForChangesAsync (
                    m_dataPersistence.Object,
                    fetchedSdnDataSet,
                    It.IsAny<CancellationToken> ()),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenDataChanged_ShouldInvokeDataPersistence ()
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());

            var entryAdded = Mock.Of<ISdnEntry> ();
            var entryRemoved = Mock.Of<ISdnEntry> ();
            var entryModified = Mock.Of<ISdnEntry> ();
            m_dataChangesChecker.Setup (self => self.CheckForChangesAsync (m_dataPersistence.Object, fetchedSdnDataSet, It.IsAny<CancellationToken> ()))
                                .ReturnsAsync (CreateSdnDataChangedCheckResult (dataChanged: true, entryAdded, entryRemoved, entryModified));

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            m_dataPersistence.Verify (self => self.Add (entryAdded), Times.Once);
            m_dataPersistence.Verify (self => self.Remove (entryRemoved), Times.Once);
            m_dataPersistence.Verify (self => self.Update (entryModified), Times.Once);
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenDataNotChanged_ShouldNotInvokeDataPersistence ()
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());

            var entryAdded = Mock.Of<ISdnEntry> ();
            var entryRemoved = Mock.Of<ISdnEntry> ();
            var entryModified = Mock.Of<ISdnEntry> ();
            m_dataChangesChecker.Setup (self => self.CheckForChangesAsync (m_dataPersistence.Object, fetchedSdnDataSet, It.IsAny<CancellationToken> ()))
                                .ReturnsAsync (CreateSdnDataChangedCheckResult (dataChanged: false, entryAdded, entryRemoved, entryModified));

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            m_dataPersistence.Verify (self => self.Add (It.IsAny<ISdnEntry> ()), Times.Never);
            m_dataPersistence.Verify (self => self.Remove (It.IsAny<ISdnEntry> ()), Times.Never);
            m_dataPersistence.Verify (self => self.Update (It.IsAny<ISdnEntry> ()), Times.Never);
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenCheckForChangesAsyncReturnsNull_ShouldNotInvokeOnSdnDataChangedDelegate ()
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());
            m_dataChangesChecker.Setup (self => self.CheckForChangesAsync (m_dataPersistence.Object, fetchedSdnDataSet, It.IsAny<CancellationToken> ()))
                                .ReturnsAsync ((ISdnDataChangesCheckResult<ISdnEntry>) null);

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            var onSdnDataChangedDelegate = new Mock<Action<object, SdnDataChangedEventArgs>> ();
            monitorService.OnSdnDataChanged (onSdnDataChangedDelegate.Object);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            onSdnDataChangedDelegate.Verify (self => self (It.IsAny<object> (), It.IsAny<SdnDataChangedEventArgs> ()), Times.Never);
        }


        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenDataNotChanged_ShouldNotInvokeOnSdnDataChangedDelegate ()
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());
            m_dataChangesChecker.Setup (self => self.CheckForChangesAsync (m_dataPersistence.Object, fetchedSdnDataSet, It.IsAny<CancellationToken> ()))
                                .ReturnsAsync (CreateSdnDataChangedCheckResult (dataChanged: false, added: 0, removed: 0, modified: 0));

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            var onSdnDataChangedDelegate = new Mock<Action<object, SdnDataChangedEventArgs>> ();
            monitorService.OnSdnDataChanged (onSdnDataChangedDelegate.Object);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            onSdnDataChangedDelegate.Verify (self => self (It.IsAny<object> (), It.IsAny<SdnDataChangedEventArgs> ()), Times.Never);
        }

        [Theory]
        [InlineData (0)]
        [InlineData (100)]
        [InlineData (10000)]
        public async Task ExecuteMonitoringCheckAsync_WhenDataChanged_ShouldInvokeOnSdnDataChangedDelegateWithEventArgs (int entriesCount)
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());

            var checkResult = CreateSdnDataChangedCheckResult (dataChanged: true, added: entriesCount, removed: entriesCount, modified: entriesCount);
            m_dataChangesChecker.Setup (self => self.CheckForChangesAsync (m_dataPersistence.Object, fetchedSdnDataSet, It.IsAny<CancellationToken> ()))
                                .ReturnsAsync (checkResult);

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            var onSdnDataChangedDelegate = new Mock<Action<object, SdnDataChangedEventArgs>> ();
            monitorService.OnSdnDataChanged (onSdnDataChangedDelegate.Object);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            onSdnDataChangedDelegate.Verify (self =>
                self (
                    monitorService,
                    It.Is<SdnDataChangedEventArgs> (args =>
                        args.EntriesAdded == checkResult.EntriesAdded.Count
                        && args.EntriesAdded == checkResult.EntriesAdded.Count
                        && args.EntriesAdded == checkResult.EntriesAdded.Count)),
                Times.Once);
        }

        [Fact]
        public async Task ExecuteMonitoringCheckAsync_WhenThereAreMultipleDelegatesRegistered_ShouldInvokeAllOfThem ()
        {
            // Arrange
            var fetchedSdnDataSet = CreateSdnDataSet (Enumerable.Empty<ISdnEntry> ());
            m_dataRetriever.Setup (self => self.FetchSdnDataAsync (It.IsAny<CancellationToken> ())).ReturnsAsync (fetchedSdnDataSet);
            m_dataPersistence.Setup (self => self.Entries).Returns (Enumerable.Empty<ISdnEntry> ());

            var checkResult = CreateSdnDataChangedCheckResult (dataChanged: true, added: 0, removed: 0, modified: 0);
            m_dataChangesChecker.Setup (self => self.CheckForChangesAsync (m_dataPersistence.Object, fetchedSdnDataSet, It.IsAny<CancellationToken> ()))
                                .ReturnsAsync (checkResult);

            var monitorService = new SdnChangesMonitorServiceFake (m_dataChangesChecker.Object, m_dataRetriever.Object, m_dataPersistence.Object, m_options);

            var firstOnSdnDataChangedDelegate = new Mock<Action<object, SdnDataChangedEventArgs>> ();
            var secondOnSdnDataChangedDelegate = new Mock<Action<object, SdnDataChangedEventArgs>> ();
            monitorService.OnSdnDataChanged (firstOnSdnDataChangedDelegate.Object);
            monitorService.OnSdnDataChanged (secondOnSdnDataChangedDelegate.Object);

            // Act
            await monitorService.ExecuteMonitoringCheckAsync ();

            // Assert
            firstOnSdnDataChangedDelegate.Verify (self => self (monitorService, It.IsAny<SdnDataChangedEventArgs> ()), Times.Once);
            secondOnSdnDataChangedDelegate.Verify (self => self (monitorService, It.IsAny<SdnDataChangedEventArgs> ()), Times.Once);
        }

        private static ISdnDataSet<ISdnEntry> CreateSdnDataSet (IEnumerable<ISdnEntry> entries) =>
            Mock.Of<ISdnDataSet<ISdnEntry>> (self => self.Entries == entries);

        private static ISdnDataChangesCheckResult<ISdnEntry> CreateSdnDataChangedCheckResult (bool dataChanged, int added = 0, int removed = 0, int modified = 0)
        {
            return CreateSdnDataChangedCheckResult (dataChanged, new List<ISdnEntry> (added), new List<ISdnEntry> (removed), new List<ISdnEntry> (modified));
        }

        private static ISdnDataChangesCheckResult<ISdnEntry> CreateSdnDataChangedCheckResult (bool dataChanged, ISdnEntry added, ISdnEntry removed, ISdnEntry modified)
        {
            var addedEntries = new List<ISdnEntry> { added };
            var removedEntries = new List<ISdnEntry> { removed };
            var modifiedEntries = new List<ISdnEntry> { modified };

            return CreateSdnDataChangedCheckResult (dataChanged, addedEntries, removedEntries, modifiedEntries);
        }

        private static ISdnDataChangesCheckResult<ISdnEntry> CreateSdnDataChangedCheckResult (bool dataChanged,
            List<ISdnEntry> added = null, List<ISdnEntry> removed = null, List<ISdnEntry> modified = null)
        {

            return Mock.Of<ISdnDataChangesCheckResult<ISdnEntry>> (self =>
                self.DataChanged == dataChanged
                && self.EntriesAdded == added
                && self.EntriesRemoved == removed
                && self.EntriesModified == modified);
        }

        private class SdnChangesMonitorServiceFake : SdnChangesMonitorService<ISdnEntry>
        {
            public SdnChangesMonitorServiceFake (ISdnDataChangesChecker<ISdnEntry> dataChangesChecker,  ISdnDataRetriever<ISdnEntry> dataRetriever, 
                ISdnDataPersistence<ISdnEntry> dataPersistence, IOptions<SdnMonitorOptions> options) 
                : base (dataChangesChecker, dataRetriever, dataPersistence, options)
            {
            }

            public Task ExecuteMonitoringCheckAsync () => ExecuteMonitoringCheckAsync (default);
        }
    }
}

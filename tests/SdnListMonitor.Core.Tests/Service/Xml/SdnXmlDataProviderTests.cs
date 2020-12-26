using Microsoft.Extensions.Options;
using Moq;
using SdnListMonitor.Core.Abstractions.Service.Xml;
using SdnListMonitor.Core.Configuration.Xml;
using SdnListMonitor.Core.Model.Xml;
using SdnListMonitor.Core.Service.Xml;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xunit;

namespace SdnListMonitor.Core.Tests.Service
{
    public class SdnXmlDataProviderTests
    {
        private readonly IOptions<SdnXmlDataProviderOptions> m_options;

        public SdnXmlDataProviderTests ()
        {
            m_options = Options.Create (new SdnXmlDataProviderOptions ());
        }

        [Fact]
        public void Ctor_WhenXmlReaderFactoryNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new SdnXmlDataProvider (null, m_options))
                  .ParamName
                  .ShouldBe ("xmlReaderFactory");
        }

        [Fact]
        public void Ctor_WhenOptionsNull_ShouldThrowArgumentNullException ()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => new SdnXmlDataProvider (Mock.Of<IXmlReaderFactory> (), null))
                  .ParamName
                  .ShouldBe ("options");
        }

        [Fact]
        public async Task GetSdnEntriesAsync_WhenFilePathIsSetInOptions_ShouldCreateXmlReaderWithPresetPath ()
        {
            // Arrange
            m_options.Value.XmlFilePath = Guid.NewGuid ().ToString ();
            var xmlReader = new Mock<XmlReader> ();
            xmlReader.Setup (self => self.ReadToDescendant (It.IsAny<string> ())).Returns (true);
            xmlReader.Setup (self => self.EOF).Returns (true);
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader.Object);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ();

            // Assert
            Mock.Get (xmlReaderFactory).Verify (self => self.Create (
                m_options.Value.XmlFilePath, 
                It.IsAny<XmlReaderSettings> ()), 
            Times.Once);
        }

        [Fact]
        public async Task GetSdnEntriesAsync_WhenXmlReaderIsReadyToBeCreated_ShouldSetXmlReaderSettings()
        {
            // Arrange
            var xmlReader = new Mock<XmlReader> ();
            xmlReader.Setup (self => self.ReadToDescendant (It.IsAny<string> ())).Returns (true);
            xmlReader.Setup (self => self.EOF).Returns (true);
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader.Object);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ();

            // Assert
            Mock.Get (xmlReaderFactory).Verify (self => self.Create (
                It.IsAny<string> (), 
                It.Is<XmlReaderSettings> (settings =>
                    settings.Async && settings.IgnoreComments && settings.IgnoreWhitespace)), 
            Times.Once);
        }

        [Fact]
        public void GetSdnEntriesAsync_WhenSdnXmlDoesNotContainSdnList_ShouldThrowInvalidOperationException ()
        {
            // Arrange
            var stream = CreateStreamFromString ("");
            var xmlReader = XmlReader.Create (stream, new XmlReaderSettings { Async = true });
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act & Assert
            var exception = Should.Throw<InvalidOperationException> (async () => await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ());
            exception.Message.ShouldBe ("An error occurred while retrieving SDN List.");
            exception.InnerException.ShouldBeOfType<XmlException> ();
        }

        [Fact]
        public void GetSdnEntriesAsync_WhenSdnXmlContainsUnknownRootElement_ShouldThrowInvalidOperationException ()
        {
            // Arrange
            var sdnListXml = new XElement ("UnknownElement");
            var stream = CreateStreamFromString (sdnListXml.ToString ());
            var xmlReader = XmlReader.Create (stream, new XmlReaderSettings { Async = true });
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            Should.Throw<InvalidOperationException> (async () => await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ())
                  .Message
                  .ShouldBe ("An error occurred while retrieving SDN List.");
        }

        [Fact]
        public async Task GetSdnEntriesAsync_WhenSdnListDoesNotContainSdnEntries_ShouldReturnEmptyEnumeration ()
        {
            // Arrange
            var sdnListXml = CreateSdnListXElement ();
            var stream = CreateStreamFromString (sdnListXml.ToString ());
            var xmlReader = XmlReader.Create (stream, new XmlReaderSettings { Async = true });
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            var sdnEntries = await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ();

            // Assert
            sdnEntries.ShouldBeEmpty ();
        }

        [Fact]
        public async Task GetSdnEntriesAsync_WhenSdnListContainsSdnEntries_ShouldReturnSdnEntries ()
        {
            // Arrange
            var expectedSdnEntries = new[] 
                {
                new SdnXmlEntry { Uid = 1 },
                new SdnXmlEntry { Uid = 2 },
                new SdnXmlEntry { Uid = 3 },
                };
            var sdnListXml = CreateSdnListXElement (expectedSdnEntries);
            var stream = CreateStreamFromString (sdnListXml.ToString ());
            var xmLReaderSettings = new XmlReaderSettings { Async = true, IgnoreComments = true, IgnoreWhitespace = true };
            var xmlReader = XmlReader.Create (stream, xmLReaderSettings);
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            var actualSdnEntries = await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ();

            // Assert
            actualSdnEntries.Length.ShouldBe (expectedSdnEntries.Length);
            for (int i = 0; i < expectedSdnEntries.Length; i++)
                actualSdnEntries[i].Uid.ShouldBe (expectedSdnEntries[i].Uid);
        }

        [Fact]
        public async Task GetSdnEntriesAsync_WhenSdnEntryIsDeserialized_ShouldReturnSdnEntryWithExactValues ()
        {
            // Arrange
            var sdnXmlEntry = new SdnXmlEntry () 
            { 
                Uid = 1,
                FirstName = "FirstName",
                LastName = "LastName",
                Title = "Title",
                SdnType = "SdnType",
                Remarks = "Remarks"
            };
            var sdnListXml = CreateSdnListXElement (sdnXmlEntry);
            var stream = CreateStreamFromString (sdnListXml.ToString ());
            var xmlReader = XmlReader.Create (stream, new XmlReaderSettings { Async = true });
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            var sdnEntry = await sdnXmlDataProvider.GetSdnEntriesAsync ().SingleAsync ();

            // Assert
            sdnEntry.ShouldSatisfyAllConditions (
                () => sdnEntry.Uid.ShouldBe (sdnXmlEntry.Uid),
                () => sdnEntry.FirstName.ShouldBe (sdnXmlEntry.FirstName),
                () => sdnEntry.LastName.ShouldBe (sdnXmlEntry.LastName),
                () => sdnEntry.Title.ShouldBe (sdnXmlEntry.Title),
                () => sdnEntry.SdnType.ShouldBe (sdnXmlEntry.SdnType),
                () => sdnEntry.Remarks.ShouldBe (sdnXmlEntry.Remarks)
                );
        }

        [Fact]
        public void GetSdnEntriesAsync_WhenSdnListContainMalformedSdnEntry_ShouldThrowInvalidOperationException ()
        {
            // Arrange
            var sdnListXml = new XElement ("sdnList", new XElement ("sdnEntry", "Malformed content"));
            var stream = CreateStreamFromString (sdnListXml.ToString ());
            var xmlReader = XmlReader.Create (stream, new XmlReaderSettings { Async = true });
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act & Assert
            Should.Throw<InvalidOperationException> (async () => await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ())
                  .Message
                  .ShouldBe ("An error occurred while retrieving SDN List.");
        }

        [Fact]
        public async Task GetSdnEntriesAsync_WhenSdnEntriesAreReturned_ShouldDisposeXmlReader ()
        {
            // Arrange
            var xmlReader = new Mock<XmlReader> ();
            xmlReader.Setup (self => self.ReadToDescendant (It.IsAny<string> ())).Returns (true);
            xmlReader.Setup (self => self.EOF).Returns (true);
            var disposableXmlReader = xmlReader.As<IDisposable> ();
            var xmlReaderFactory = CreateXmlReaderFactory (xmlReader.Object);
            var sdnXmlDataProvider = new SdnXmlDataProvider (xmlReaderFactory, m_options);

            // Act
            await sdnXmlDataProvider.GetSdnEntriesAsync ().ToArrayAsync ();

            // Assert
            disposableXmlReader.Verify (self => self.Dispose (), Times.Once);
        }

        private static Stream CreateStreamFromString (string text) =>
            new MemoryStream (Encoding.UTF8.GetBytes (text));

        private static XElement CreateSdnListXElement (params SdnXmlEntry[] sdnXmlEntries)
        {
            XNamespace ns = "http://tempuri.org/sdnList.xsd";
            var sdnEntryXElements = sdnXmlEntries.Select (entry => CreateXElementFrom (ns, entry)).ToArray ();
            return new XElement (ns + "sdnList", sdnEntryXElements);
        }

        private static XElement CreateXElementFrom (XNamespace ns, SdnXmlEntry sdnXmlEntry)
        {
            return new XElement (ns + "sdnEntry",
                new XElement (ns + "uid", sdnXmlEntry.Uid),
                new XElement (ns + "firstName", sdnXmlEntry.FirstName),
                new XElement (ns + "lastName", sdnXmlEntry.LastName),
                new XElement (ns + "title", sdnXmlEntry.Title),
                new XElement (ns + "sdnType", sdnXmlEntry.SdnType),
                new XElement (ns + "remarks", sdnXmlEntry.Remarks)
                );
        }

        private static IXmlReaderFactory CreateXmlReaderFactory (XmlReader xmlReader) =>
            Mock.Of<IXmlReaderFactory> (self => self.Create (It.IsAny<string> (), It.IsAny<XmlReaderSettings> ()) == xmlReader);
    }
}

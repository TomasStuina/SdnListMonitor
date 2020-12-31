using Moq;
using SdnListMonitor.Core.Xml.Data.Model;
using SdnListMonitor.Core.Xml.Service.Data;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace SdnListMonitor.Core.Tests.Service.Data
{
    public class SdnXmlEntryEqualityComparerTests
    {
        private readonly SdnXmlEntryEqualityComparer m_comparer;

        public SdnXmlEntryEqualityComparerTests ()
        {
            m_comparer = new SdnXmlEntryEqualityComparer ();
        }

        [Fact]
        public void Equals_WhenBothSdnEntriesNull_ShouldReturnTrue ()
        {
            // Act & Assert
            m_comparer.Equals (null, null).ShouldBeTrue ();
        }

        [Fact]
        public void Equals_WhenFirstSdnEntryNull_ShouldReturnFalse ()
        {
            // Act & Assert
            m_comparer.Equals (null, Mock.Of<SdnXmlEntry> ()).ShouldBeFalse ();
        }

        [Fact]
        public void Equals_WhenSecondSdnEntryNull_ShouldReturnFalse ()
        {
            // Act & Assert
            m_comparer.Equals (null, Mock.Of<SdnXmlEntry> ()).ShouldBeFalse ();
        }

        [Theory]
        [InlineData (0, 1)]
        [InlineData (1, 0)]
        public void Equals_WhenBothUidsAreDifferent_ShouldReturnFalse (int firstEntryUid, int secondEntryUid)
        {
            // Arrange
            var firstEntry = Mock.Of<SdnXmlEntry> (self => self.Uid == firstEntryUid);
            var secondEntry = Mock.Of<SdnXmlEntry> (self => self.Uid == secondEntryUid);

            // Act & Assert
            m_comparer.Equals (firstEntry, secondEntry).ShouldBeFalse ();
        }

        [Theory]
        [MemberData (nameof (CaseSensitiveStringTestData))]
        public void Equals_WhenBothFirstNamesAreDifferent_ShouldReturnFalse (string firstEntryFirstName, string secondEntryFirstName)
        {
            // Arrange
            var firstEntry = Mock.Of<SdnXmlEntry> (self => self.FirstName == firstEntryFirstName);
            var secondEntry = Mock.Of<SdnXmlEntry> (self => self.FirstName == secondEntryFirstName);

            // Act & Assert
            m_comparer.Equals (firstEntry, secondEntry).ShouldBeFalse ();
        }

        [Theory]
        [MemberData (nameof (CaseSensitiveStringTestData))]
        public void Equals_WhenBothLastNamesAreDifferent_ShouldReturnFalse (string firstEntryLastName, string secondEntryLastName)
        {
            // Arrange
            var firstEntry = Mock.Of<SdnXmlEntry> (self => self.LastName == firstEntryLastName);
            var secondEntry = Mock.Of<SdnXmlEntry> (self => self.LastName == secondEntryLastName);

            // Act & Assert
            m_comparer.Equals (firstEntry, secondEntry).ShouldBeFalse ();
        }

        [Theory]
        [MemberData (nameof (CaseSensitiveStringTestData))]
        public void Equals_WhenBothTitlesAreDifferent_ShouldReturnFalse (string firstEntryTitle, string secondEntryTitle)
        {
            // Arrange
            var firstEntry = Mock.Of<SdnXmlEntry> (self => self.Title == firstEntryTitle);
            var secondEntry = Mock.Of<SdnXmlEntry> (self => self.Title == secondEntryTitle);

            // Act & Assert
            m_comparer.Equals (firstEntry, secondEntry).ShouldBeFalse ();
        }

        [Theory]
        [MemberData (nameof (IgnoreCaseSensitiveStringTestData))]
        public void Equals_WhenBothSdnTypesAreDifferent_ShouldReturnFalse (string firstEntrySdnType, string secondEntrySdnType)
        {
            // Arrange
            var firstEntry = Mock.Of<SdnXmlEntry> (self => self.SdnType == firstEntrySdnType);
            var secondEntry = Mock.Of<SdnXmlEntry> (self => self.SdnType == secondEntrySdnType);

            // Act & Assert
            m_comparer.Equals (firstEntry, secondEntry).ShouldBeFalse ();
        }

        [Theory]
        [MemberData (nameof (CaseSensitiveStringTestData))]
        public void Equals_WhenBothRemarksAreDifferent_ShouldReturnFalse (string firstEntryRemarks, string secondEntryRemarks)
        {
            // Arrange
            var firstEntry = Mock.Of<SdnXmlEntry> (self => self.Remarks == firstEntryRemarks);
            var secondEntry = Mock.Of<SdnXmlEntry> (self => self.Remarks == secondEntryRemarks);

            // Act & Assert
            m_comparer.Equals (firstEntry, secondEntry).ShouldBeFalse ();
        }

        [Fact]
        public void GetHashCode_WhenSdnEntryNull_ShouldReturnZero ()
        {
            // Act & Assert
            m_comparer.GetHashCode (null).ShouldBe (0);
        }

        [Theory]
        [InlineData (int.MinValue)]
        [InlineData (0)]
        [InlineData (int.MaxValue)]
        public void GetHashCode_WhenSdnEntryNotNull_ShouldReturnUidValue(int uid)
        {
            // Arrange
            var entry = Mock.Of<SdnXmlEntry> (self => self.Uid == uid);

            // Act & Assert
            m_comparer.GetHashCode (entry).ShouldBe (uid);
        }

        public static IEnumerable<object[]> CaseSensitiveStringTestData ()
        {
            yield return new object[] { null, "" };
            yield return new object[] { "", null };
            yield return new object[] { "a", "b" };
            yield return new object[] { "differentcasing", "DIFFERENTCASING" };
            yield return new object[] { "\u0160", "S" }; // Š AND S
        }

        public static IEnumerable<object[]> IgnoreCaseSensitiveStringTestData ()
        {
            yield return new object[] { null, "" };
            yield return new object[] { "", null };
            yield return new object[] { "a", "b" };
            yield return new object[] { "\u0160", "S" }; // Š AND S
        }
    }
}

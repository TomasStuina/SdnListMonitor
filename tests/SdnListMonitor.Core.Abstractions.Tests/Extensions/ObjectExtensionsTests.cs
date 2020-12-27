using SdnListMonitor.Core.Abstractions.Extensions;
using Shouldly;
using System;
using Xunit;

namespace SdnListMonitor.Core.Abstractions.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Theory]
        [InlineData ("")]
        [InlineData ("argumentName")]
        public void ThrowIfNull_WhenObjectIsNull_ShouldThrowArgumentException (string paramName)
        {
            // Act & Assert
            Should.Throw<ArgumentNullException> (() => ObjectExtensions.ThrowIfNull ((object) null, paramName))
                  .ParamName
                  .ShouldBe (paramName);
        }

        [Fact]
        public void ThrowIfNull_WhenObjectIsNotNull_ShouldNotThrow ()
        {
            // Act & Assert
            Should.NotThrow (() => new object ().ThrowIfNull ("paramName"));
        }

        [Fact]
        public void ThrowIfNull_WhenObjectIsNotNull_ShouldReturnTheSameObject ()
        {
            // Arrange
            var instance = new object ();

            // Act & Assert
            instance.ThrowIfNull ("paramName").ShouldBeSameAs (instance);
        }
    }
}

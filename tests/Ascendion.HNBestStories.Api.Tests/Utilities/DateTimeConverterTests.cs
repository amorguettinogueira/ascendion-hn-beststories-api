using Ascendion.HNBestStories.Api.Utilities;
using FluentAssertions;
using Xunit;

namespace Ascendion.HNBestStories.Api.Tests.Utilities;

/// <summary>
/// Unit tests for the DateTimeConverter utility.
/// Tests Unix timestamp conversion to DateTime.
/// </summary>
public class DateTimeConverterTests
{
    [Fact]
    public void FromUnixTimeStamp_WithZero_ReturnsUnixEpoch()
    {
        // Arrange
        long unixTimeStamp = 0;

        // Act
        var result = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);

        // Assert
        result.Should().Be(DateTime.UnixEpoch);
    }

    [Fact]
    public void FromUnixTimeStamp_WithPositiveValue_ReturnsCorrectDateTime()
    {
        // Arrange
        long unixTimeStamp = 1000000000; // 2001-09-09 01:46:40 UTC

        // Act
        var result = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);

        // Assert
        result.Year.Should().Be(2001);
        result.Month.Should().Be(9);
        result.Day.Should().Be(9);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUnixTimeStamp_WithCurrentTime_ReturnsValidDateTime()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var unixTimeStamp = (long)now.Subtract(DateTime.UnixEpoch).TotalSeconds;

        // Act
        var result = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);

        // Assert
        // Allow 1 second difference due to rounding
        result.Should().BeCloseTo(now, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void FromUnixTimeStamp_ReturnedDateTimeIsUtc()
    {
        // Arrange
        long unixTimeStamp = 1609459200; // 2021-01-01 00:00:00 UTC

        // Act
        var result = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);

        // Assert
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUnixTimeStamp_WithMultipleValues_ConvertsConsistently()
    {
        // Arrange
        var timestamps = new long[] { 0, 1000, 1000000, 1000000000, 1609459200 };

        // Act & Assert
        foreach (var ts in timestamps)
        {
            var result = DateTimeConverter.FromUnixTimeStamp(ts);
            result.Kind.Should().Be(DateTimeKind.Utc);
        }
    }

    [Fact]
    public void FromUnixTimeStamp_WithNegativeValue_ReturnsDateBeforeEpoch()
    {
        // Arrange
        long unixTimeStamp = -1;
        var expectedDateTime = DateTime.UnixEpoch.AddSeconds(-1);

        // Act
        var result = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);

        // Assert
        result.Should().Be(expectedDateTime);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void FromUnixTimeStamp_WithMaxValue_ThrowsArgumentException()
    {
        // Arrange
        long unixTimeStamp = long.MaxValue;

        // Act & Assert
        var act = () => DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromUnixTimeStamp_Twice_WithSameInput_ReturnsSameResult()
    {
        // Arrange
        long unixTimeStamp = 1234567890;

        // Act
        var result1 = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);
        var result2 = DateTimeConverter.FromUnixTimeStamp(unixTimeStamp);

        // Assert
        result1.Should().Be(result2);
    }
}
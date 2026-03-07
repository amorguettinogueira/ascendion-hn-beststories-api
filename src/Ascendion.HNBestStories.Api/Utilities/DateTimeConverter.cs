namespace Ascendion.HNBestStories.Api.Utilities;

/// <summary>
/// Provides utilities for date and time conversions.
/// Responsible for converting Unix timestamps to .NET DateTime objects.
/// </summary>
public static class DateTimeConverter
{
    /// <summary>
    /// Converts a Unix timestamp (seconds since January 1, 1970) to a UTC DateTime.
    /// </summary>
    /// <param name="unixTimeStamp">The Unix timestamp in seconds.</param>
    /// <returns>A DateTime object in UTC timezone.</returns>
    /// <exception cref="ArgumentException">Thrown when the timestamp results in an out-of-range DateTime.</exception>
    public static DateTime FromUnixTimeStamp(long unixTimeStamp)
    {
        try
        {
            return DateTime.UnixEpoch.AddSeconds(unixTimeStamp);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentException($"The provided Unix timestamp {unixTimeStamp} is out of valid DateTime range.", nameof(unixTimeStamp), ex);
        }
    }
}

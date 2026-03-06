using System.ComponentModel.DataAnnotations;

namespace Ascendion.HNBestStories.Api.Settings;

/// <summary>
/// General API configuration settings.
/// Supports environment variables in Docker containers.
/// </summary>
public class ApiSettings
{
    public const string SectionName = "Api";

    /// <summary>
    /// Whether the API is running in development mode.
    /// Environment variable: API_DEVELOPMENT_MODE
    /// </summary>
    public bool DevelopmentMode { get; set; } = false;

    /// <summary>
    /// Enable detailed error messages in API responses.
    /// Should be false in production.
    /// Environment variable: API_DETAILED_ERRORS
    /// </summary>
    public bool DetailedErrorMessages { get; set; } = false;
}

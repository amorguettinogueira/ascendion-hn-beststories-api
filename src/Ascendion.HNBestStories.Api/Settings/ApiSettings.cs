namespace Ascendion.HNBestStories.Api.Settings;

/// <summary>
/// General API configuration settings.
/// Supports environment variable override via configuration binding.
/// Environment variable names should use double underscores for nested properties (e.g., Api__DevelopmentMode).
/// </summary>
public sealed class ApiSettings
{
    /// <summary>
    /// The configuration section name for API settings.
    /// Used when binding configuration values.
    /// </summary>
    public const string SectionName = "Api";

    /// <summary>
    /// Gets or sets a value indicating whether the API is running in development mode.
    /// </summary>
    /// <remarks>
    /// When true, enables additional development features and logging.
    /// Should be false in production environments.
    /// Default: false
    /// </remarks>
    public bool DevelopmentMode { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to enable detailed error messages in API responses.
    /// </summary>
    /// <remarks>
    /// When true, API error responses include full exception details and stack traces.
    /// Should always be false in production to avoid information disclosure.
    /// Default: false
    /// </remarks>
    public bool DetailedErrorMessages { get; set; } = false;
}
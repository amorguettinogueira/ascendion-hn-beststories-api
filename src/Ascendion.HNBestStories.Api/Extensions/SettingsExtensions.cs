using Ascendion.HNBestStories.Api.Settings;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Ascendion.HNBestStories.Api.Extensions;

/// <summary>
/// Extension methods for configuring and validating application settings.
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Registers and validates application settings from configuration.
    /// Throws InvalidOperationException if settings are invalid.
    /// </summary>
    /// <param name="services">The service collection to register settings with.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <returns>The same service collection for chaining.</returns>
    /// <exception cref="InvalidOperationException">Thrown when settings validation fails.</exception>
    public static IServiceCollection AddApplicationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HackerNews settings with validation
        services
            .Configure<HackerNewsSettings>(configuration.GetSection(HackerNewsSettings.SectionName))
            .AddSingleton(sp => sp.GetRequiredService<IOptions<HackerNewsSettings>>().Value);

        // Register API settings with validation
        services
            .Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName))
            .AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);

        // Validate all settings on startup
        ValidateAllSettings(services, configuration);

        return services;
    }

    /// <summary>
    /// Validates all application settings against their data annotations.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration root.</param>
    /// <exception cref="InvalidOperationException">Thrown when any settings fail validation.</exception>
    private static void ValidateAllSettings(IServiceCollection services, IConfiguration configuration)
    {
        ValidateHackerNewsSettings(configuration);
        ValidateApiSettings(configuration);
    }

    /// <summary>
    /// Validates HackerNews settings.
    /// </summary>
    /// <param name="configuration">The configuration root.</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    private static void ValidateHackerNewsSettings(IConfiguration configuration)
    {
        var settings = new HackerNewsSettings();
        configuration.GetSection(HackerNewsSettings.SectionName).Bind(settings);

        var context = new ValidationContext(settings, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(settings, context, results, validateAllProperties: true))
        {
            var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException(
                $"HackerNews settings validation failed: {errors}");
        }
    }

    /// <summary>
    /// Validates API settings.
    /// </summary>
    /// <param name="configuration">The configuration root.</param>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    private static void ValidateApiSettings(IConfiguration configuration)
    {
        var settings = new ApiSettings();
        configuration.GetSection(ApiSettings.SectionName).Bind(settings);

        var context = new ValidationContext(settings, serviceProvider: null, items: null);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(settings, context, results, validateAllProperties: true))
        {
            var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException(
                $"Api settings validation failed: {errors}");
        }
    }
}
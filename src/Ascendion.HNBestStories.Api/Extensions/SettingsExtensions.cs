using Ascendion.HNBestStories.Api.Settings;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Ascendion.HNBestStories.Api.Extensions;

public static class SettingsExtensions
{
    /// <summary>
    /// Registers and validates application settings from configuration.
    /// Throws if settings are invalid.
    /// </summary>
    public static IServiceCollection AddApplicationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HackerNews settings with validation
        _ = services
            .Configure<HackerNewsSettings>(configuration.GetSection(HackerNewsSettings.SectionName))
            .AddSingleton(sp => sp.GetRequiredService<IOptions<HackerNewsSettings>>().Value);

        // Register API settings with validation
        _ = services
            .Configure<ApiSettings>(configuration.GetSection(ApiSettings.SectionName))
            .AddSingleton(sp => sp.GetRequiredService<IOptions<ApiSettings>>().Value);

        // Validate settings on startup
        ValidateSettings(services, configuration);

        return services;
    }

    private static void ValidateSettings(IServiceCollection services, IConfiguration configuration)
    {
        var hackerNewsSettings = new HackerNewsSettings();

        configuration.GetSection(HackerNewsSettings.SectionName).Bind(hackerNewsSettings);

        var context = new ValidationContext(hackerNewsSettings);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(hackerNewsSettings, context, results, true))
        {
            var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"HackerNews settings validation failed: {errors}");
        }

        var apiSettings = new ApiSettings();
        configuration.GetSection(ApiSettings.SectionName).Bind(apiSettings);

        var apiContext = new ValidationContext(apiSettings);
        var apiResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(apiSettings, apiContext, apiResults, true))
        {
            var errors = string.Join("; ", apiResults.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Api settings validation failed: {errors}");
        }
    }
}
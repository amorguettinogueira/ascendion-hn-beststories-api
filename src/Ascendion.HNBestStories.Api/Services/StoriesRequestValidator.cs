using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Settings;

namespace Ascendion.HNBestStories.Api.Services;

/// <inheritdoc />
public sealed class StoriesRequestValidator(HackerNewsSettings settings) : IStoriesRequestValidator
{
    private readonly HackerNewsSettings _settings = settings
        ?? throw new ArgumentNullException(nameof(settings));

    /// <inheritdoc />
    public string? Validate(int numberOfStories)
    {
        if (numberOfStories <= 0)
        {
            return "Number of stories must be greater than 0.";
        }

        if (numberOfStories > _settings.MaxStoriesAllowed)
        {
            return $"Number of stories must not exceed {_settings.MaxStoriesAllowed}.";
        }

        return null;
    }
}

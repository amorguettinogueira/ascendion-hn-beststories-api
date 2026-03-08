using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Endpoints;

/// <summary>
/// Provides endpoint mappings for best stories API operations.
/// </summary>
public static class BestStoriesEndpoints
{
    /// <summary>
    /// Maps the best stories endpoints to the application.
    /// </summary>
    /// <param name="app">The web application builder.</param>
    public static void MapBestStoriesEndpoints(this WebApplication app) =>
        app.MapGet("/beststories/{numberOfStories}", HandleGetBestStories)
            .WithName("GetBestStories")
            .AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Summary = "Retrieves the top N best stories from Hacker News";
                operation.Description = "Returns the top N best stories from Hacker News API, sorted by score in descending order.";
                return Task.CompletedTask;
            })
            .Produces<IEnumerable<BestStory>>(StatusCodes.Status200OK,
                contentType: "application/json")
            .Produces(StatusCodes.Status400BadRequest,
                contentType: "application/json")
            .Produces(StatusCodes.Status500InternalServerError,
                contentType: "application/json");

    /// <summary>
    /// Handles GET requests for the best stories endpoint.
    /// </summary>
    /// <param name="numberOfStories">The number of stories to retrieve (path parameter).</param>
    /// <param name="bestStoriesService">The service responsible for fetching best stories.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>
    /// A successful response containing the stories, or an error response indicating what went wrong.
    /// </returns>
    private static async Task<IResult> HandleGetBestStories(
        int numberOfStories,
        IBestStoriesService bestStoriesService,
        CancellationToken cancellationToken)
    {
        var response = await bestStoriesService.GetBestStoriesAsync(numberOfStories, cancellationToken);

        return response switch
        {
            ApiResponse.Success success =>
                Results.Ok(success.Data),
            ApiResponse.Error error =>
                Results.BadRequest(new { error = error.Message }),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
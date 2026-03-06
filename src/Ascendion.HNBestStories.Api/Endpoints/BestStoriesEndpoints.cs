using Ascendion.HNBestStories.Api.Abstractions;
using Ascendion.HNBestStories.Api.Models;

namespace Ascendion.HNBestStories.Api.Endpoints;

public static class BestStoriesEndpoints
{
    public static void MapBestStoriesEndpoints(this WebApplication app) =>
        _ = app.MapGet("/beststories/{numberOfStories}", HandleGetBestStories)
           .AddOpenApiOperationTransformer((operation, context, ct) =>
           {
               operation.Summary = "Retrieves the top N best stories from Hacker News";
               operation.Description = "Returns the top N best stories from Hacker News API, sorted by score in descending order.";
               return Task.CompletedTask;
           });

    private static async Task<IResult> HandleGetBestStories(
        int numberOfStories,
        IBestStoriesService bestStoriesService,
        CancellationToken cancellationToken)
    {
        var response = await bestStoriesService.GetBestStoriesAsync(numberOfStories, cancellationToken);

        return response switch
        {
            ApiResponse.Success success => Results.Ok(success.Data),
            ApiResponse.Error error => Results.BadRequest(new { error = error.Message }),
            _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
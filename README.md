# Ascendion HN Best Stories API

A resilient ASP.NET Core API that fetches and ranks the top N best stories from Hacker News, demonstrating modern .NET architecture, best practices, and comprehensive testing.

## 1. Summary & Best Practices

### Overview
This solution provides a REST API endpoint (`GET /beststories/{numberOfStories}`) that retrieves the top N best stories from the Hacker News API, ranked by score in descending order. The implementation demonstrates production-ready patterns and best practices.

### Support & Documentation

For detailed information, see:
- **Architecture Details**: [ARCHITECTURE_DETAILS.md](ARCHITECTURE_DETAILS.md)
- **Best Practices**: [BEST_PRACTICES.md](BEST_PRACTICES.md) (applied patterns and principles)
- **Configuration Details**: [CONFIGURATION.md](CONFIGURATION.md)

---

## 2. Assumptions & Design Decisions

### Error Handling Strategy

**Individual Story Fetch Failures (Graceful Degradation)**
- **Decision**: Individual story fetch failures are caught and logged, not propagated
- **Rationale**: Maximizes user value by returning available stories rather than failing completely
- **Implementation**: `FetchStoryWithFallbackAsync()` catches exceptions and returns null, which are filtered out
- **Impact**: If 8 of 10 stories fail, the API returns 2 stories instead of 0
- **Logging**: Failures logged at WARNING level (cancellation) or ERROR level (other exceptions)

**Service-Level Exceptions (Fail-Fast)**
- **Decision**: Exceptions from GetBestStoryIdsAsync and validation are caught and converted to error responses
- **Rationale**: These are critical path failures that indicate system issues, not individual data problems
- **Implementation**: Caught in GetBestStoriesAsync and returned as ApiResponse.Error
- **Impact**: Caller receives structured error response with context instead of unhandled exception

**Generic Exceptions (Cascaded)**
- **Decision**: Unexpected exceptions are NOT re-thrown; they're returned as error responses
- **Rationale**: ASP.NET Core global exception handlers can still catch them; API caller gets structured response
- **Implementation**: Catch-all in GetBestStoriesAsync converts exceptions to error messages
- **Impact**: API stability - no unhandled exceptions leak to caller

### Caching Strategy

- **IMemoryCache**: In-process cache for single-instance scenarios
- **TTL Configuration**: Separate TTLs for story IDs (5 min) vs. individual stories (1 hour)
- **Cache Invalidation**: Time-based expiry; no manual invalidation
- **Future**: Ready for Redis or distributed cache migration

### Validation Approach

- **Early Validation**: Input validated before API calls
- **Validator Interface**: Allows easy swapping of validation logic
- **Validation Exceptions**: Dedicated StoriesValidationException for validation failures

### API Response Design

- **Success Response**: Returns paginated data (IEnumerable<BestStory>)
- **Error Response**: Structured error with descriptive message
- **No Null Responses**: Either Success or Error, never null
- **Type-Safe**: Discriminated union pattern prevents misuse

---

## 3. Configuration

All configuration can be set via: `appsettings.json` (checked into repo with defaults), or `appsettings.{Environment}.json` (environment-specific overrides), or `.env` file (for Docker and local development) or  Environment variables (system-level overrides).

### Configuration Levers

| Setting | Default | Purpose | Type |
|---------|---------|---------|------|
| `BestStoriesIdsCacheDurationMinutes` | 5 | Cache TTL for story ID list | int |
| `StoryCacheDurationHours` | 1 | Cache TTL for individual stories | int |
| `MaxStoriesAllowed` | 30 | Maximum stories API can return | int |
| `MaxParallelRequests` | 10 | Concurrent story fetch limit | int |
| `RequestTimeoutSeconds` | 2 | HTTP request timeout for external API | int |
| `MaxRetryAttempts` | 3 | Max retry attempts for transient failures | int |
| `RetryDelayMilliseconds` | 200 | Base delay for retry backoff | int |
| `CircuitBreakerFailureRatio` | 0.5 | Failure ratio to trip circuit breaker | double |
| `CircuitBreakerSamplingDurationSeconds` | 30 | Time window for circuit breaker sampling | int |
| `CircuitBreakerMinimumThroughput` | 5 | Minimum requests before circuit breaker can trip | int |

> Performance tunning tips:
> 1. **Increase MaxParallelRequests** for faster fetching (but respect upstream API)
> 2. **Increase Cache Durations** to reduce API calls (at cost of freshness)
> 3. **Adjust Retry Delays** for slower networks (exponential backoff helps)
> 4. **Monitor Circuit Breaker** - adjust failure ratio if too sensitive

---

## 4. How to Run

### Prerequisites

- .NET 10 SDK ([download](https://dotnet.microsoft.com/download/dotnet/10.0))
- (Optional) Docker Desktop for containerized execution

### Setup

1. **Clone the repository**
   ```
   git clone https://github.com/amorguettinogueira/ascendion-hn-beststories-api.git
   ```

2. **Configure environment (optional)**
   ```
   cd ascendion-hn-beststories-api\src
   cp .env.example .env
   # Edit .env with desired configuration
   ```

### Running the API

> Important!
> Via `dotnet run` the API will be available only via HTTP (HTTPS is not configured) on port 5279 `http://localhost:5279/beststories/10`.

**Development mode** (with OpenAPI docs at http://localhost:5279/openapi/v1.json)
```
cd Ascendion.HNBestStories.Api
dotnet run
```

**With custom port**
```
cd Ascendion.HNBestStories.Api
dotnet run -- --urls "http://localhost:8080"
```

### Docker Execution

> Important! 
> Via `docker-compose` the API will be available only via HTTPS on port 5001 `https://localhost:5001/beststories/10`.

**Using docker-compose**
```
cd src
docker-compose up --build
```

### Testing the API

Once running, the API is available at `http://localhost:5279` or `https://localhost:5001`.

**Example requests:**

```
# Get top 10 best stories
curl http://localhost:5279/beststories/10
```

**Response example:**
```json
[
  {
    "title": "A Great Story",
    "uri": "https://example.com/story",
    "postedBy": "username",
    "time": "2024-03-08T12:00:00Z",
    "score": 500,
    "commentCount": 125
  },
  ...
]
```

You can also use Postman to import the OpenAPI spec from `http://localhost:5279/openapi/v1.json` or `https://localhost:5001/openapi/v1.json` for interactive testing.

---

## 5. Unit Tests & Code Coverage

### Running Unit Tests

#### All Tests
```
cd tests\Ascendion.HNBestStories.Api.Tests
dotnet test
```

### Code Coverage Reports

#### Generate Coverage Report

> Important! 
> The command below throws error `2026-03-08T12:34:59: File '.\ascendion-hn-beststories-api\src\Ascendion.HNBestStories.Api\obj\Debug\net10.0\Microsoft.AspNetCore.OpenApi.SourceGenerators\Microsoft.AspNetCore.OpenApi.SourceGenerators.XmlCommentGenerator\OpenApiXmlCommentSupport.generated.cs' does not exist (any more).` which can be ignored since this file is generated during build and deleted after tests run. The coverage report will still be generated successfully.

```
cd tests\Ascendion.HNBestStories.Api.Tests
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"..\..\coverage-report" ` -reporttypes:Html
```

#### View Coverage Report
After running the above commands, open `coverage-report/index.html` in your browser.
```
cd ..\..\coverage-report
start index.html
```

---

## 6. Improvements & Enhancements

**Distributed Caching**
- **Current**: IMemoryCache (single-instance only)
- **Enhancement**: Migrate to Redis or similar for multi-instance deployments
- **Impact**: Enable horizontal scaling; shared cache across instances

**C# SDK Client Library**
- **Current**: Consumers must call HTTP endpoint directly
- **Enhancement**: Create Ascendion.HNBestStories.Client NuGet package
- **Impact**: Strongly-typed client for C# applications; easier integration; including: Auto-retry logic, exception handling, models

**Advanced Resilience Configuration**
- **Current**: Hardcoded resilience policy settings
- **Enhancement**: Make retry count, delays, circuit breaker thresholds configurable
- **Impact**: Fine-tune resilience per environment (dev vs. production)

**Database Persistence**
- **Current**: Cache-only, no persistence across restarts
- **Enhancement**: Add optional PostgreSQL/SQL Server for caching beyond TTL
- **Impact**: Reduce external API calls; persist popular stories

**Story Filtering & Sorting Options**
- **Current**: Only sorted by score
- **Enhancement**: Add query parameters for date range, author filter, min score
- **Impact**: More flexible API; better UX

**OpenAPI Security Schemes**
- **Current**: No authentication
- **Enhancement**: Add API key or OAuth2 security definitions
- **Impact**: Production-ready security; rate limiting

**Observability & Metrics**
- **Current**: Basic structured logging
- **Enhancement**: Add Application Insights/Prometheus metrics; including: Request latency, cache hit rates, API call counts
- **Impact**: Production monitoring and alerting

**Performance Optimization**
- **Current**: Sequential batch fetching
- **Enhancement**: Implement adaptive batch sizing based on response times
- **Impact**: Faster responses in varied network conditions

**Integration Tests**
- **Current**: Unit tests with mocks
- **Enhancement**: Add integration tests against real Hacker News API; including: Docker-based test environment; CI/CD pipeline
- **Impact**: Catch real-world API changes early

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                     HTTP Request                        │
└────────────────────────┬────────────────────────────────┘
                         │
                   ┌─────▼───────┐
                   │ BestStories │
                   │ Endpoints   │
                   └─────┬───────┘
                         │
               ┌─────────▼──────────┐
               │ BestStoriesService │
               │  (Orchestration)   │
               └─────────┬──────────┘
                         │
         ┌───────────────┼───────────────┐
         │               │               │
    ┌────▼────┐  ┌───────▼──────┐  ┌─────▼──────┐
    │Validator│  │ Memory Cache │  │ HackerNews │
    │         │  │              │  │ Client     │
    └─────────┘  └──────────────┘  └─────┬──────┘
                                         │
                                   ┌─────▼─────────┐
                                   │ HN API        │
                                   │ (Hacker News) │
                                   └───────────────┘
```

---

## Quick Reference

- **API Endpoint**: `GET /beststories/{numberOfStories}`
- **API Documentation**: `GET /openapi/v1.json` (Swagger/OpenAPI)
- **Configuration File**: `appsettings.json`
- **Environment Variables**: `.env` or system environment
- **Test Suite**: `tests/Ascendion.HNBestStories.Api.Tests`
- **Coverage Report**: `coverage-report/index.html` (after running coverage)

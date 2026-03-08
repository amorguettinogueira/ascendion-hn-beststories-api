# Configuration Guide

This document describes the configuration system for the Hacker News Best Stories API.

## Overview

The application uses the **Options Pattern** - a .NET standard approach for managing configuration. Settings are:
- Loaded from `appsettings.json` and environment-specific overrides
- Bound to strongly-typed configuration classes
- Validated on application startup
- Overridable via environment variables (perfect for Docker)

## Configuration Sections

### HackerNews Settings (`HackerNewsSettings`)

Controls behavior of the Hacker News API client, caching strategy, and resilience policies.

| Setting | Environment Variable | Type | Default | Range | Description |
|---------|----------------------|------|---------|-------|-------------|
| `BestStoriesIdsCacheDurationMinutes` | `HackerNews__BestStoriesIdsCacheDurationMinutes` | int | 5 | 1-60 | How long to cache the list of best story IDs (minutes) |
| `StoryCacheDurationHours` | `HackerNews__StoryCacheDurationHours` | int | 1 | 1-24 | How long to cache individual story details (hours) |
| `MaxStoriesAllowed` | `HackerNews__MaxStoriesAllowed` | int | 30 | 1-200 | Maximum number of stories per API request |
| `MaxParallelRequests` | `HackerNews__MaxParallelRequests` | int | 10 | 1-50 | Max concurrent story fetches (controls load on external API) |
| `RequestTimeoutSeconds` | `HackerNews__RequestTimeoutSeconds` | int | 2 | 1-30 | HTTP request timeout in seconds |
| `MaxRetryAttempts` | `HackerNews__MaxRetryAttempts` | int | 3 | 0-10 | Maximum retry attempts for transient failures (0 = no retries) |
| `RetryDelayMilliseconds` | `HackerNews__RetryDelayMilliseconds` | int | 200 | 1-5000 | Initial retry delay in milliseconds (uses exponential backoff) |
| `CircuitBreakerFailureRatio` | `HackerNews__CircuitBreakerFailureRatio` | double | 0.5 | 0.0-1.0 | Failure ratio threshold for circuit breaker (0.5 = 50%) |
| `CircuitBreakerSamplingDurationSeconds` | `HackerNews__CircuitBreakerSamplingDurationSeconds` | int | 30 | 1-300 | Circuit breaker evaluation window in seconds |
| `CircuitBreakerMinimumThroughput` | `HackerNews__CircuitBreakerMinimumThroughput` | int | 5 | 1-100 | Minimum requests required before circuit breaker evaluates |

### API Settings (`ApiSettings`)

General API behavior and response handling.

| Setting | Environment Variable | Type | Default | Description |
|---------|----------------------|------|---------|-------------|
| `DevelopmentMode` | `Api__DevelopmentMode` | bool | false | Enable development-specific behavior and enhanced logging |
| `DetailedErrorMessages` | `Api__DetailedErrorMessages` | bool | false | Include detailed error messages in responses (disable in production) |

## Configuration Methods

### 1. Configuration Files (appsettings.json)

**Production (appsettings.json):**
```json
{
  "HackerNews": {
    "BestStoriesIdsCacheDurationMinutes": 5,
    "StoryCacheDurationHours": 1,
    "MaxStoriesAllowed": 30,
    "MaxParallelRequests": 10,
    "RequestTimeoutSeconds": 2,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 200,
    "CircuitBreakerFailureRatio": 0.5,
    "CircuitBreakerSamplingDurationSeconds": 30,
    "CircuitBreakerMinimumThroughput": 5
  },
  "Api": {
    "DevelopmentMode": false,
    "DetailedErrorMessages": false
  }
}
```

**Development (appsettings.Development.json):**
```json
{
  "HackerNews": {
    "BestStoriesIdsCacheDurationMinutes": 1,
    "StoryCacheDurationHours": 1,
    "MaxStoriesAllowed": 30,
    "MaxParallelRequests": 5,
    "RequestTimeoutSeconds": 5,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 100,
    "CircuitBreakerFailureRatio": 0.5,
    "CircuitBreakerSamplingDurationSeconds": 30,
    "CircuitBreakerMinimumThroughput": 3
  },
  "Api": {
    "DevelopmentMode": true,
    "DetailedErrorMessages": true
  }
}
```

### 2. Environment Variables (Docker)

Set environment variables when running in Docker:

```bash
docker run -e HackerNews__MaxStoriesAllowed=50 \
           -e HackerNews__MaxParallelRequests=20 \
           -e HackerNews__MaxRetryAttempts=5 \
           -e HackerNews__CircuitBreakerFailureRatio=0.6 \
           -e Api__DetailedErrorMessages=false \
           my-app:latest
```

## Profile-Specific Configuration

The application loads configuration in this order (later overrides earlier):
1. `appsettings.json` (base configuration)
2. `appsettings.{ASPNETCORE_ENVIRONMENT}.json` (environment-specific)
3. User Secrets (local development only)
4. Environment Variables (highest priority)

### Environment Setup

**Development:**
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

**Production:**
```bash
export ASPNETCORE_ENVIRONMENT=Production
```

## Docker Example

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=Production
CMD ["dotnet", "Ascendion.HNBestStories.Api.dll"]
```

### docker-compose.yml
```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:5000"
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      HackerNews__MaxStoriesAllowed: 50
      HackerNews__MaxParallelRequests: 20
      HackerNews__BestStoriesIdsCacheDurationMinutes: 10
      HackerNews__MaxRetryAttempts: 5
      HackerNews__CircuitBreakerFailureRatio: 0.6
      Api__DetailedErrorMessages: "false"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
```

## Validation

All settings are validated on application startup using Data Annotations. Invalid configuration will cause the application to fail with a clear error message.

**Example error:**
```
HackerNews settings validation failed: Max parallel requests must be between 1 and 50
```

## Resilience Policy Configuration

The application uses **Polly** for resilience with three integrated policies:

### Retry Policy
```
MaxRetryAttempts: Number of times to retry failed requests
RetryDelayMilliseconds: Initial delay (multiplied by 2 for each retry - exponential backoff)
```

**Example:** With defaults (3 attempts, 200ms):
- Attempt 1: 200ms
- Attempt 2: 400ms
- Attempt 3: 800ms

### Circuit Breaker Policy
```
CircuitBreakerFailureRatio: Opens circuit when failure rate exceeds this threshold
CircuitBreakerSamplingDurationSeconds: Time window to evaluate failure rate
CircuitBreakerMinimumThroughput: Minimum requests before evaluation
```

**Example:** With defaults:
- If 50% or more requests fail within a 30-second window AND at least 5 requests occurred, the circuit opens
- Prevents cascading failures to external API

## Tuning Recommendations

### High-Traffic Scenario
```json
{
  "HackerNews": {
    "BestStoriesIdsCacheDurationMinutes": 10,
    "StoryCacheDurationHours": 2,
    "MaxParallelRequests": 5,
    "MaxStoriesAllowed": 100,
    "MaxRetryAttempts": 5,
    "CircuitBreakerFailureRatio": 0.6
  }
}
```

### Low-Traffic/Development
```json
{
  "HackerNews": {
    "BestStoriesIdsCacheDurationMinutes": 1,
    "StoryCacheDurationHours": 1,
    "MaxParallelRequests": 20,
    "MaxStoriesAllowed": 30,
    "RequestTimeoutSeconds": 5,
    "MaxRetryAttempts": 3
  }
}
```

### API-Friendly (Minimize Impact on Hacker News)
```json
{
  "HackerNews": {
    "BestStoriesIdsCacheDurationMinutes": 20,
    "StoryCacheDurationHours": 4,
    "MaxParallelRequests": 3,
    "MaxRetryAttempts": 2,
    "RetryDelayMilliseconds": 500
  }
}
```

### Aggressive Resilience
```json
{
  "HackerNews": {
    "MaxRetryAttempts": 10,
    "RetryDelayMilliseconds": 100,
    "CircuitBreakerFailureRatio": 0.7,
    "CircuitBreakerMinimumThroughput": 3
  }
}
```

## Configuration Examples

### Complete .env File
```env
# ===== HACKER NEWS SETTINGS =====
# Caching
HackerNews__BestStoriesIdsCacheDurationMinutes=5
HackerNews__StoryCacheDurationHours=1

# Request Behavior
HackerNews__MaxStoriesAllowed=30
HackerNews__MaxParallelRequests=10
HackerNews__RequestTimeoutSeconds=2

# Resilience: Retry Policy
HackerNews__MaxRetryAttempts=3
HackerNews__RetryDelayMilliseconds=200

# Resilience: Circuit Breaker Policy
HackerNews__CircuitBreakerFailureRatio=0.5
HackerNews__CircuitBreakerSamplingDurationSeconds=30
HackerNews__CircuitBreakerMinimumThroughput=5

# ===== API SETTINGS =====
Api__DevelopmentMode=false
Api__DetailedErrorMessages=false
```

### Example: Production with High Resilience
```env
HackerNews__MaxStoriesAllowed=75
HackerNews__MaxParallelRequests=15
HackerNews__MaxRetryAttempts=5
HackerNews__CircuitBreakerFailureRatio=0.6
HackerNews__CircuitBreakerMinimumThroughput=10
Api__DetailedErrorMessages=false
```

### Example: Development Environment
```env
HackerNews__BestStoriesIdsCacheDurationMinutes=1
HackerNews__RequestTimeoutSeconds=5
HackerNews__MaxRetryAttempts=2
Api__DevelopmentMode=true
Api__DetailedErrorMessages=true
```

## Environment Variable Naming Convention

When setting environment variables:
- Use double underscores (`__`) to represent nested configuration sections
- The format is: `SectionName__PropertyName`
- Examples:
  - `HackerNews__MaxStoriesAllowed`
  - `HackerNews__CircuitBreakerFailureRatio`
  - `Api__DetailedErrorMessages`

## Validation Rules

All settings are validated with the following constraints:

| Setting | Constraint | Error Message |
|---------|-----------|---------------|
| BestStoriesIdsCacheDurationMinutes | 1-60 | Must be between 1 and 60 minutes |
| StoryCacheDurationHours | 1-24 | Must be between 1 and 24 hours |
| MaxStoriesAllowed | 1-200 | Must be between 1 and 200 |
| MaxParallelRequests | 1-50 | Must be between 1 and 50 |
| RequestTimeoutSeconds | 1-30 | Must be between 1 and 30 seconds |
| MaxRetryAttempts | 0-10 | Must be between 0 and 10 |
| RetryDelayMilliseconds | 1-5000 | Must be between 1 and 5000 |
| CircuitBreakerFailureRatio | 0.0-1.0 | Must be between 0.0 and 1.0 |
| CircuitBreakerSamplingDurationSeconds | 1-300 | Must be between 1 and 300 seconds |
| CircuitBreakerMinimumThroughput | 1-100 | Must be between 1 and 100 requests |

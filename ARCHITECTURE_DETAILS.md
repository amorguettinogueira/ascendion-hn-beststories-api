# ARCHITECTURE DETAILS

## Clean Separation of Concerns
```
API Layer
    └── BestStoriesEndpoints (HTTP routing)
        └── IResult responses

Service Layer
    ├── IBestStoriesService
    │   └── BestStoriesService (Orchestration)
    │       ├── IStoriesRequestValidator
    │       ├── IHackerNewsClient
    │       └── DateTimeConverter
    │
    ├── IHackerNewsClient
    │   └── HackerNewsClient (External API)
    │       ├── HttpClient (Resilience)
    │       └── IMemoryCache
    │
    └── IStoriesRequestValidator
        └── StoriesRequestValidator (Validation)

Data Layer
    ├── Models (Value Objects)
    │   ├── BestStory (record)
    │   ├── HackerNewsStory (sealed)
    │   └── ApiResponse (discriminated union)
    │
    └── Settings
        ├── HackerNewsSettings
        └── ApiSettings

Infrastructure
    ├── Extensions
    │   ├── HttpClientExtensions
    │   └── SettingsExtensions
    │
    ├── Utilities
    │   └── DateTimeConverter
    │
    └── Exceptions
        ├── HackerNewsApiException
        └── StoriesValidationException
```

---

## Service Dependencies (Dependency Injection Graph)

### Clean, Testable Design
```
Program.cs
├── Registers: IServiceCollection
│
├── Settings (Validated on startup)
│   ├── HackerNewsSettings
│   └── ApiSettings
│
├── Services
│   ├── BestStoriesService
│   │   ├── IHackerNewsClient
│   │   ├── HackerNewsSettings
│   │   └── IStoriesRequestValidator
│   │
│   ├── HackerNewsClient
│   │   ├── HttpClient (with resilience)
│   │   ├── IMemoryCache
│   │   └── HackerNewsSettings
│   │
│   └── StoriesRequestValidator
│       └── HackerNewsSettings
│
├── Infrastructure
│   └── HttpClientFactory
│       └── Resilience Policies
│           ├── Retry (Exponential backoff)
│           ├── Circuit Breaker
│           └── Timeout
│
└── Endpoints
    └── Minimal API routing
```

---

## Error Handling Flow

### Typed, Specific Handling
```
try
{
    // ... business logic with specific exceptions ...
}
catch (HackerNewsApiException ex)
{
    return new ApiResponse.Error($"Hacker News API error: {ex.Message}");
}
catch (StoriesValidationException ex)
{
    return new ApiResponse.Error($"Validation error: {ex.Message}");
}
catch (Exception ex)
{
    return new ApiResponse.Error($"An unexpected error occurred: {ex.Message}");
}
```

---

## Data Model

### Immutable Record with Validation
```csharp
public sealed record BestStory(
    string Title,
    string Uri,
    string PostedBy,
    DateTime Time,
    int Score,
    int CommentCount)  // ✓ Immutable
{
    // ✓ Default values for nulls
    public string Title { get; } = string.IsNullOrWhiteSpace(Title) 
        ? "No title available" : Title;
    
    public string PostedBy { get; } = string.IsNullOrWhiteSpace(PostedBy) 
        ? "No author available" : PostedBy;

	...

    // ✓ Validation method
    public bool IsValid() => !string.IsNullOrWhiteSpace(Title) && Score >= 0;
}
```

**Benefits:**
- ✅ Immutable (thread-safe)
- ✅ Value-based equality
- ✅ Pattern matching support
- ✅ Record deconstruction
- ✅ Self-validating

---

## Request Handling Pipeline

### Clean, Linear Flow
```
HTTP Request
    ↓
[1] BestStoriesEndpoints.HandleGetBestStories()
    ├─ Receives: numberOfStories (int)
    ├─ Injects: IBestStoriesService, CancellationToken
    └─ Returns: IResult
    ↓
[2] IBestStoriesService.GetBestStoriesAsync()
    ├─ Step A: Validate input
    │         └─→ IStoriesRequestValidator.Validate()
    │             Returns: string? (error message or null)
    │
    ├─ Step B: Get story IDs
    │         └─→ IHackerNewsClient.GetBestStoryIdsAsync()
    │             └─ With caching (5 min default)
    │             └─ With resilience (retry, circuit breaker, timeout)
    │
    ├─ Step C: Fetch stories in parallel batches
    │         └─→ IHackerNewsClient.GetStoryAsync() × N
    │             └─ With caching (1 hour default)
    │             └─ With error handling (returns null on failure)
    │
    ├─ Step D: Convert stories
    │         └─→ DateTimeConverter.FromUnixTimeStamp()
    │         └─→ BestStory constructor (with validation)
    │
    ├─ Step E: Filter valid stories
    │         └─→ story.IsValid()
    │
    └─ Step F: Sort by score
            └─→ OrderByDescending(story => story.Score)
    ↓
[3] ApiResponse (Discriminated Union)
    ├─ Success(IEnumerable<BestStory>) 
    │   └─→ Results.Ok(stories)
    │
    └─ Error(string Message)
        └─→ Results.BadRequest(new { error })
    ↓
HTTP Response (JSON)
```

---

## Resilience Strategy (Hacker News API Protection)

### Multi-Layer Defense
```
┌─────────────────────────────────────────────────────────┐
│ HttpClient with Resilience Policies                     │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [TIMEOUT PROTECTION]                                   │
│  └─ 2 seconds per request (configurable)                │
│                                                         │
│  [RETRY POLICY]                                         │
│  └─ Up to 3 attempts with exponential backoff           │
│     Delay: 200ms → 400ms → 800ms                        │
│                                                         │
│  [CIRCUIT BREAKER]                                      │
│  └─ Opens if 50% of requests fail                       │
│     Sampling: 30-second window                          │
│     Minimum: 5 requests before evaluation               │
│                                                         │
│  [CACHING]                                              │
│  └─ Best IDs: 5 minutes (prevent frequent calls)        │
│  └─ Stories: 1 hour (balance freshness & load)          │
│                                                         │
│  [GRACEFUL DEGRADATION]                                 │
│  └─ Individual story failures don't fail the request    │
│  └─ Returns what's available (partial success)          │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## Configuration Management

### Environment-Based Overrides
```
Development (appsettings.json)
    ↓
Docker (.env file)
    ↓
Environment Variables (override)
    ↓
Validated Settings (HackerNewsSettings instance)
    ↓
Used in Services (DI-injected)
```

### Example: Configuring Max Stories
```
// 1. appsettings.json (default)
"HackerNews": {
  "MaxStoriesAllowed": 30
}

// 2. .env file (override)
HackerNews__MaxStoriesAllowed=50

// 3. Environment variable (final override)
set HackerNews__MaxStoriesAllowed=100

// 4. At runtime
var settings = GetService<HackerNewsSettings>();
// settings.MaxStoriesAllowed == 100 (from env var)
```

---

## SOLID Principles Implementation

### Single Responsibility
```
BestStoriesService        → Orchestration
StoriesRequestValidator   → Validation only
HackerNewsClient          → API communication
DateTimeConverter         → Timestamp conversion
SettingsExtensions        → Configuration setup
HttpClientExtensions      → HTTP client setup
```

### Open/Closed
```
✗ Cannot modify: IHackerNewsClient interface
✓ Can extend: Create new IHackerNewsClient implementation
✓ Can extend: Create new IStoriesRequestValidator implementation
```

### Liskov Substitution
```
Any IHackerNewsClient implementation can replace HackerNewsClient
Any IBestStoriesService implementation can replace BestStoriesService
Any IStoriesRequestValidator implementation can replace StoriesRequestValidator
```

### Interface Segregation
```
Client A only needs: IBestStoriesService (not IHackerNewsClient)
Client B only needs: IHackerNewsClient (not IBestStoriesService)
Validator only needs: HackerNewsSettings (not everything)
```

### Dependency Inversion
```
Programs depend on abstractions:
  - IBestStoriesService (not BestStoriesService)
  - IHackerNewsClient (not HackerNewsClient)
  - IStoriesRequestValidator (not StoriesRequestValidator)
```

---

## Security Boundaries

### Input Validation
```
numberOfStories (path parameter)
    ↓
[StoriesRequestValidator.Validate()]
    ├─ > 0?              (prevents zero/negative)
    ├─ ≤ MaxAllowed?     (prevents abuse/DoS)
    └─ Return: error message if invalid
```

### Configuration Protection
```
Settings validated at startup → Fail fast if invalid
Settings validated → No invalid state possible
Settings defaults are conservative → Safe out-of-box
```

### Error Information Containment
```
Exception Details (Internal)
    ↓
[Error Handler]
    ├─ Logs full details (server logs)
    └─ Returns user-friendly message (API response)
    ↓
API Response: "Hacker News API error"  ← No stack trace
             (Not: "NullReferenceException at line 42")
```

---

## Testing Surface

### Easy to Test
```
Unit Tests:
  ✓ BestStoriesService (mock IHackerNewsClient)
  ✓ StoriesRequestValidator (no dependencies)
  ✓ DateTimeConverter (pure function)
  ✓ BestStory (value object)

Integration Tests:
  ✓ BestStoriesService + real HackerNewsClient
  ✓ Settings + configuration loading

API Tests:
  ✓ Endpoint with test server
  ✓ Full request/response cycle
```

### No Hidden State
```
All dependencies injected → Can mock everything
No static methods → Can test in isolation
Stateless services → No shared state between tests
Settings passed via DI → Can use test settings
```

---

## Performance Characteristics

### Intelligent Batching
```
Requested: 50 stories
MaxParallelRequests: 10

Batch 1: Fetch stories 1-10 (parallel)
Batch 2: Fetch stories 11-20 (parallel)
Batch 3: Fetch stories 21-30 (parallel)
Batch 4: Fetch stories 31-40 (parallel)
Batch 5: Fetch stories 41-50 (parallel)

Total HTTP requests: 1 (IDs) + 50 (stories) = 51
Parallel degree: 10 simultaneous
Prevents: API overload, network saturation
```

### Cache Hit Optimization
```
Without Cache:
  Request 1 → 1 ID request + 50 story requests = 51 calls
  Request 2 → 1 ID request + 50 story requests = 51 calls
  Total: 102 calls

With Cache (5 min IDs, 1 hour stories):
  Request 1 → 51 calls (0 cache hits)
  Request 2 → 0 calls (all cached) ← 100% reduction
  Request 3 → 0 calls (all cached) ← 100% reduction
  ...
  Total: 51 calls (amortized to ~0.4 calls per request)
```
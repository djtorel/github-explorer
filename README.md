# GitHub Explorer

A web application for searching GitHub users by username and exploring their public profile and repositories. Built as a portfolio piece demonstrating professional engineering practices — Clean Architecture, functional error handling, resilience patterns, and responsive UI.

---

## What It Does

1. **Search** — Enter any GitHub username on the home page.
2. **Profile** — View the user's avatar, name, bio, follower count, and public repository count.
3. **Repositories** — Browse a paginated list of public repositories sorted by star count (descending). Each repo shows its description, star count, fork count, and primary language.
4. **Pagination** — Navigate through pages and choose page sizes of 10, 30, or 50.
5. **Direct URLs** — Share or bookmark any profile via `/user/{username}`.
6. **Dark / Light Mode** — Toggle between themes; preference is persisted in `localStorage`.
7. **Error Handling** — Graceful, contextual messages for "user not found", rate limits, network errors, and empty repositories. Includes retry for recoverable errors.
8. **Tested** — 121 automated tests across 6 test suites (backend unit, integration, frontend component, E2E).

---

## How to Run It

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- (Optional) A GitHub Personal Access Token for higher rate limits (60 req/hr unauthenticated, 5000/hr with token)

### 1. Clone & Configure

```bash
git clone <repo-url>
cd GitHubExplorer
```

Optionally add a GitHub token for higher rate limits:

```bash
cd src/GitHubExplorer.Api
dotnet user-secrets set "GitHubApi:Token" "ghp_your_token_here"
```

---

### Option A: One Command (Recommended for Reviewers)

The API project is wired to build the frontend automatically before it starts. From the repository root:

```bash
dotnet run --project src/GitHubExplorer.Api
```

On first run this will:

1. Install frontend dependencies (`npm install`)
2. Build the SPA (`npm run build`)
3. Copy static files into `wwwroot`
4. Start the ASP.NET Core server

Then open **https://localhost:5115** (or check console output for the exact URL).

Or use the convenience script:

```bash
# macOS / Linux
./run.sh

# Windows PowerShell
.\run.ps1
```

> **Note:** This builds the production frontend bundle. For active frontend development with hot reload, use Option B below.

---

### Option B: Separate Frontend + Backend (Active Development)

**Backend** (terminal 1):

```bash
cd src/GitHubExplorer.Api
dotnet run -p:BuildFrontend=false
```

The `-p:BuildFrontend=false` flag skips the SPA build so backend startup is instant.

**Frontend** (terminal 2):

```bash
cd src/frontend
npm install
npm run dev
```

The dev server starts at `http://localhost:5173` and proxies `/api` calls to the backend. Changes hot-reload instantly.

---

### Build for Production Manually

```bash
cd src/frontend
npm run build
```

This outputs static files to `src/GitHubExplorer.Api/wwwroot/`. Then run the backend:

```bash
cd src/GitHubExplorer.Api
dotnet run
```

### Run Tests

```bash
# Backend — full suite (85 tests)
dotnet test

# Backend — per project
dotnet test tests/GitHubExplorer.Domain.Tests          # 25 tests
dotnet test tests/GitHubExplorer.Application.Tests     # 17 tests
dotnet test tests/GitHubExplorer.Infrastructure.Tests  # 31 tests
dotnet test tests/GitHubExplorer.Api.Tests             # 12 integration tests

# Frontend — component tests (31 tests)
cd src/frontend && npm test

# Frontend — E2E tests (5 tests, headless Chromium)
cd src/frontend && npx playwright test

# Frontend — E2E with UI mode
cd src/frontend && npx playwright test --ui
```

---

## Project Structure

```
src/
├── GitHubExplorer.Domain/           # Core entities, interfaces, Result<T> type
│   ├── Models/                      # UserProfile, Repository (sealed records)
│   ├── Results/                     # Result<T>, GitHubError enum, async extensions
│   └── Interfaces/                  # IGitHubClient (abstraction for infra)
│
├── GitHubExplorer.Application/      # Use cases, DTOs, service interfaces
│   ├── DTOs/                        # ApiResponseDto, UserProfileDto, RepositoryDto
│   ├── Interfaces/                  # IGitHubService
│   └── Services/                    # GitHubService (orchestration + mapping)
│
├── GitHubExplorer.Infrastructure/   # External API communication
│   ├── GitHubApiClient.cs           # HttpClient-based GitHub API client
│   ├── ResiliencePolicies.cs        # Polly retry + circuit breaker
│   ├── GitHubApiOptions.cs          # Config binding
│   └── DependencyInjection.cs       # IHttpClientFactory + Polly wiring
│
├── GitHubExplorer.Api/              # ASP.NET Core host
│   ├── Controllers/                 # UsersController, RepositoriesController
│   ├── Program.cs                   # DI, middleware, static files, SPA fallback
│   └── appsettings*.json
│
└── frontend/                        # Svelte 5 + Vite SPA
    ├── src/
    │   ├── lib/
    │   │   ├── api.ts               # Fetch wrappers for backend API
    │   │   ├── state.svelte.ts      # Global reactive state (runes)
    │   │   ├── test-setup.ts        # Vitest setup (matchMedia, localStorage mocks)
    │   │   └── types.ts             # TypeScript domain types
    │   ├── routes/
    │   │   ├── Home.svelte          # Search landing page
    │   │   ├── Profile.svelte       # Profile + repo list page
    │   │   └── NotFound.svelte      # 404 page
    │   ├── components/              # SearchBar, ProfileCard, RepoList, etc.
    │   │   └── __tests__/           # Component test files (Vitest)
    │   └── e2e/                     # Playwright E2E specs
    ├── package.json
    ├── vite.config.ts
    ├── vitest.config.ts
    └── playwright.config.ts

tests/
├── GitHubExplorer.Domain.Tests/        # Result<T>, async extensions, model tests
├── GitHubExplorer.Application.Tests/   # Service logic, mapping tests (NSubstitute mocks)
├── GitHubExplorer.Infrastructure.Tests/# API client, resilience, options validation
└── GitHubExplorer.Api.Tests/           # Integration tests via WebApplicationFactory
```

---

## Backend Decisions

### 1. Clean Architecture (Domain → Application → Infrastructure → API)

**Decision:** Four projects with strict dependency direction. Domain has zero external dependencies.

**Why:** The assignment values "SOLID and architectural discipline." Clean Architecture makes the dependency rules explicit and ensures the core logic is insulated from framework and infrastructure changes. Even in a small project, this demonstrates that structure and testability are intentional, not accidental.

**Trade-off:** More files and projects than a minimal API in a single file. Accepted because the goal is to signal engineering maturity, not minimalism.

### 2. `Result<T>` Instead of Exceptions for Business Errors

**Decision:** A custom `Result<T>` struct with `Map`, `Bind`, `Match`, and `MapError` methods. `GitHubError` is a domain enum (`NotFound`, `RateLimited`, `NetworkError`, `EmptyResult`, `Unknown`).

**Why:** GitHub API errors are _expected_ (rate limits happen, users mistype names). Using exceptions for control flow is an anti-pattern. `Result<T>` makes error paths explicit in the type system, forces callers to handle failures, and composes cleanly via functional operators:

```csharp
client.GetUserAsync(username)
    .MapAsync(MapToDto);
```

**Trade-off:** Slightly more verbose than `try/catch`. Gained: compile-time safety, easier unit testing, no hidden control flow.

### 3. Controllers over Minimal APIs

**Decision:** Traditional `[ApiController]` classes with primary constructors.

**Why:** In a project where demonstrating architecture matters, controllers sit cleanly within Clean Architecture boundaries. Minimal APIs blur layers by allowing inline DI and direct infrastructure access. Controllers enforce the pattern: Controller → Service → Client.

### 4. Polly for Resilience (Retry + Circuit Breaker)

**Decision:** `IHttpClientFactory` with Polly policies — 3 retries on 5xx/transient failures with 2-second delays, and a circuit breaker that opens after 5 failures for 30 seconds.

**Why:** The GitHub API is reliable but transient failures happen. Without retries, a single 502 becomes a user-facing error. The circuit breaker prevents cascading failure storms. Both policies are attached declaratively via `AddPolicyHandler`.

**Trade-off:** Adds the Polly dependency. Gained: production-grade resilience with ~15 lines of configuration.

### 5. DTOs at the API Boundary Only

**Decision:** Domain models (`UserProfile`, `Repository`) flow through the application layer and are mapped to DTOs (`UserProfileDto`, `RepositoryDto`) only in the application service. Controllers return a uniform `ApiResponseDto<T>` envelope.

**Why:** Keeps the domain pure and serialization-agnostic. The `ApiResponseDto<T>` envelope gives the frontend a consistent shape (`{ success, data?, error? }`) regardless of endpoint or success/failure.

### 6. HttpClient with `IHttpClientFactory`

**Decision:** Named `HttpClient` configured via `AddHttpClient<IGitHubClient, GitHubApiClient>`.

**Why:** Prevents socket exhaustion, centralizes header configuration (Accept, User-Agent, optional auth token), and enables Polly integration. A raw `HttpClient` per request would leak sockets under load.

### 7. GitHub API Token Validation

**Decision:** `IValidateOptions<GitHubApiOptions>` validates the base URL (must be HTTPS) and optionally validates token prefixes (`ghp_` or `github_pat_`).

**Why:** Fail fast on startup rather than failing mysteriously at runtime. Token prefix validation catches copy-paste errors early.

### 8. `sealed` Classes and `readonly struct`

**Decision:** All classes are `sealed` by default. `Result<T>` is a `readonly struct`.

**Why:** Prevents accidental inheritance, signals intent, and avoids defensive-null-check boilerplate. `readonly struct` on `Result<T>` prevents defensive copying overhead for a small wrapper type.

### 9. Testing Strategy (Full Pyramid)

**Decision:** Four-layer test pyramid — unit, integration, component, and E2E.

| Layer | Count | Framework | What They Verify |
|-------|-------|-----------|------------------|
| Domain Unit | 25 | xUnit + Shouldly | `Result<T>`, async extensions, model construction |
| Application Unit | 17 | xUnit + Shouldly + NSubstitute | Service logic, mapping, null handling |
| Infrastructure Unit | 31 | xUnit + Shouldly + FakeHttpMessageHandler | `GitHubApiClient`, Polly policies, config validation |
| API Integration | 12 | xUnit + WebApplicationFactory | All endpoints + error codes (404, 429, 503, 400) |
| Frontend Components | 31 | Vitest + jsdom + @testing-library/svelte | Rendering, interaction, accessibility for 6 components |
| Frontend E2E | 5 | Playwright (Chromium) | Full user journey, error states, theme toggle |
| **Total** | **121** | | |

**Why:**

- **Shouldly** over FluentAssertions — FluentAssertions switched to a commercial license in 2025. Shouldly is fully open-source.
- **NSubstitute** over Moq — Cleaner API, no telemetry controversy.
- **FakeHttpMessageHandler** — Tests run fast, deterministic, and offline.
- **WebApplicationFactory** — Integration tests exercise the full HTTP pipeline without hitting real GitHub API.
- **@testing-library/svelte** — Component tests render real Svelte components in jsdom, verifying DOM output and user interactions.
- **Playwright** — E2E tests mock API responses via `page.route()` for deterministic, fast runs (~2s).

### 10. Single-Server Deployment

**Decision:** ASP.NET Core serves the built SPA static files from `wwwroot` and uses `MapFallbackToFile("index.html")` for SPA routing.

**Why:** One process, one port, one Docker container. Eliminates CORS, simplifies deployment, and avoids the operational overhead of separate frontend/backend hosts for a read-only app.

---

## Frontend Decisions

### 1. Svelte 5 + Vite SPA over SvelteKit

**Decision:** Svelte 5 runes (`$state`, `$derived`, `$effect`) with Vite, not SvelteKit.

**Why:** This is a single-page read-only app. SvelteKit's SSR, filesystem routing, and server-side features add complexity without benefit. Vite builds to static files that ASP.NET Core serves directly. Svelte 5's runes provide fine-grained, intuitive reactivity with less boilerplate than React hooks.

**Trade-off:** No SSR means no server-rendered first paint. Accepted because the app is lightweight and the backend is fast.

### 2. No External State Library

**Decision:** Global state managed via Svelte 5 runes in `state.svelte.ts` — no Redux, Zustand, or Pinia.

**Why:** The app has a single global concern ("current user being viewed"). Svelte 5's `$state` is already reactive across module boundaries. Adding a state library would be indirection without value.

```typescript
export const currentUser = $state({
  username: "",
  profile: null as UserProfile | null,
  repos: [] as Repository[],
  totalCount: 0,
  page: 1,
  perPage: 30,
  loading: false,
  error: null as ApiError | null,
});
```

### 3. Tailwind CSS with Custom GitHub-Themed Tokens

**Decision:** Tailwind CSS v4 with a custom `@theme` block mapping semantic colors (`gh-page`, `gh-card`, `gh-text`, `gh-muted`, `gh-border`).

**Why:** Tailwind enables rapid, consistent styling without writing CSS files. The custom tokens ensure the dark/light mode toggle is a single class switch on `<html>` — all components reference the same semantic tokens, so no per-component dark-mode logic is needed.

### 4. API Error Envelope Matching

**Decision:** The frontend's `ApiResponse<T>` type mirrors the backend's `ApiResponseDto<T>` exactly.

**Why:** A single, uniform envelope (`{ success, data?, error? }`) means all API calls are handled the same way. The `ErrorBanner` component switches on `error.code` to show contextual icons, colors, and messages — no per-endpoint error parsing.

### 5. Skeleton Loaders Over Spinners

**Decision:** Custom `Skeleton.svelte` component with variant shapes (avatar, text, repo, profile) instead of generic spinners.

**Why:** Skeleton loaders reduce perceived load time and prevent layout shift. They match the shape of the content that will appear, so the page feels like it's assembling rather than waiting. The `animate-pulse` CSS animation is hardware-accelerated and lightweight.

### 6. SPA Router for Shareable URLs

**Decision:** `svelte-spa-router` with hash-based routing (`/#/user/octocat`).

**Why:** Enables direct links to any user profile. Since ASP.NET Core serves the SPA with `MapFallbackToFile`, the router handles client-side navigation without server round-trips. Hash routing avoids needing server-side rewrite rules.

### 7. Responsive-First Components

**Decision:** Mobile-first Tailwind breakpoints. The search bar, profile card, and pagination all reflow from single-column on mobile to multi-column on desktop.

**Why:** The assignment requires full responsiveness. Doing this from the start with Tailwind's `sm:`, `md:` prefixes is faster than retrofitting. Key patterns: flex direction switches (`flex-col sm:flex-row`), hidden/show toggles (`hidden sm:flex`), and text size scaling.

### 8. Controlled Page Size

**Decision:** Page size is restricted to 10, 30, or 50 on both frontend and backend.

**Why:** GitHub's API supports up to 100 per page, but allowing arbitrary sizes creates edge cases (e.g., `perPage=9999`). Restricting to three sensible values keeps pagination math reliable and the UI simple. Both frontend select and backend controller validate the same set.

---

## API Endpoints

| Endpoint                                               | Description                                          |
| ------------------------------------------------------ | ---------------------------------------------------- |
| `GET /api/users/{username}`                            | Fetch user profile                                   |
| `GET /api/users/{username}/repos?page={n}&perPage={n}` | Fetch paginated repositories (sorted by stars, desc) |

All responses use the envelope:

```json
{
  "success": true,
  "data": { ... },
  "error": null
}
```

On failure:

```json
{
  "success": false,
  "data": null,
  "error": {
    "code": "NotFound",
    "message": "User not found."
  }
}
```

---

## Technology Stack

| Layer      | Technology                          |
| ---------- | ----------------------------------- |
| Backend    | .NET 9, ASP.NET Core                |
| Resilience | Polly (retry + circuit breaker)     |
| Frontend   | Svelte 5, Vite, TypeScript          |
| Styling    | Tailwind CSS v4                     |
| Testing (backend) | xUnit, Shouldly, NSubstitute, coverlet |
| Testing (frontend unit) | Vitest, jsdom, @testing-library/svelte, @testing-library/jest-dom |
| Testing (frontend E2E) | Playwright (Chromium) |
| Build      | Vite (frontend), `dotnet` (backend) |

---

## License

MIT

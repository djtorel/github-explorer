# GitHub User Explorer

## What This Is

A web application that lets users search for any GitHub user by username and explore their public profile and repositories. The app consumes the GitHub REST API to display profile information (name, avatar, bio, follower count) and a paginated, star-sorted list of public repositories. Built as a portfolio piece demonstrating professional engineering practices including Clean Architecture, SOLID principles, and test-driven development.

## Core Value

Users can quickly find any GitHub user and browse their repositories through a fast, responsive, and error-resilient interface — with the app gracefully handling API failures, rate limits, and empty results.

## Requirements

### Validated

(None yet — ship to validate)

### Active

- [ ] **SEARCH-01**: User can enter a GitHub username and search for that user
- [ ] **PROFILE-01**: App displays the user's profile (name, avatar, bio, follower count)
- [ ] **REPO-01**: App displays a list of the user's public repositories
- [ ] **REPO-02**: Repositories are sorted by star count, descending
- [ ] **REPO-03**: Repository list is paginated (does not fetch all repos at once)
- [ ] **ERROR-01**: App handles "user not found" gracefully with clear messaging
- [ ] **ERROR-02**: App handles GitHub API rate limiting with clear messaging
- [ ] **ERROR-03**: App handles network errors gracefully
- [ ] **ERROR-04**: App handles empty results gracefully
- [ ] **UI-01**: App supports dark and light mode toggle
- [ ] **UI-02**: App is fully responsive across desktop, tablet, and mobile
- [ ] **TEST-01**: Backend has unit tests for all business logic
- [ ] **TEST-02**: Backend has integration tests for API endpoints
- [ ] **TEST-03**: Frontend has component-level unit tests
- [ ] **TEST-04**: End-to-end tests cover the core user journey

### Out of Scope

- **Repository detail page** — Clicking a repo for README/language stats would be valuable but adds scope risk; deferred to backlog
- **Language/topic filtering** — Nice-to-have differentiator; deferred to v2
- **Charts or data visualization** — Not essential to core value; deferred
- **User authentication / OAuth** — App is read-only public data; no auth needed
- **Caching layer** — GitHub API responses are relatively fast; can be added later if needed
- **Real-time updates** — Repositories don't change fast enough to justify WebSockets
- **Mobile native app** — Web-first; responsive web handles mobile
- **DevOps / CI-CD pipeline** — Out of scope per user direction; local development and Docker support only

## Context

This is a **portfolio piece for a job prospect**. The assignment explicitly values:
- **Prioritization discipline**: "A complete-but-smaller submission is better than a sprawling, unfinished one"
- **Reasoning**: "Explain in detail why any decisions were made if they are technical"
- **Decision tracking**: "Make sure to track decisions made in addition to where you normally would into our README"
- **Quality over quantity**: Signal Principal Engineer-level judgment through architecture choices, test coverage, and UX polish rather than feature count

The app is read-only and public — no authentication, no state persistence, no database. All data comes from the GitHub REST API. This simplifies the backend but makes error handling and resilience critical.

## Constraints

- **Tech Stack**: C# .NET 9 backend, Svelte 5 + Vite SPA frontend, Tailwind CSS. Locked.
- **Architecture**: Clean Architecture with Controllers (API / Application / Domain / Infrastructure projects). Demonstrates SOLID and separation of concerns.
- **Timeline**: Single milestone. MVP must be complete and testable.
- **Scope**: Pure MVP. Stretch features exist only in backlog.
- **Deployment**: ASP.NET Core serves built SPA static files from `wwwroot`. Docker support included for easy local testing. No cloud DevOps.

## Key Decisions

| Decision | Rationale | Outcome |
|----------|-----------|---------|
| Svelte 5 + Vite SPA over SvelteKit | SvelteKit adds routing/build complexity without benefit for a single-page read-only app. Vite SPA is simpler to serve from ASP.NET Core and reduces scope risk. | - Pending |
| Controllers + Clean Architecture over Minimal APIs | Clean Architecture explicitly demonstrates SOLID, separation of concerns, and testability — all valued in the assignment. Minimal APIs are concise but harder to show architectural discipline in a small project. | - Pending |
| Pure MVP scope | Assignment explicitly prioritizes "complete but smaller." Stretch features deferred to backlog to ensure polished, tested delivery. | - Pending |
| ASP.NET Core serves static SPA files | Enables single-server deployment (no separate frontend host), avoids CORS, and simplifies Docker packaging. | - Pending |
| xUnit + FluentAssertions + NSubstitute | Industry-standard .NET testing stack. FluentAssertions produces readable test code; NSubstitute has a cleaner API than Moq. | - Pending |
| Playwright for E2E | Modern, reliable, and officially recommended by Microsoft for .NET projects. Tests the real user journey across browser engines. | - Pending |

## Evolution

This document evolves at phase transitions and milestone boundaries.

**After each phase transition** (via `/gsd-transition`):
1. Requirements invalidated? → Move to Out of Scope with reason
2. Requirements validated? → Move to Validated with phase reference
3. New requirements emerged? → Add to Active
4. Decisions to log? → Add to Key Decisions
5. "What This Is" still accurate? → Update if drifted

**After each milestone** (via `/gsd-complete-milestone`):
1. Full review of all sections
2. Core Value check — still the right priority?
3. Audit Out of Scope — reasons still valid?
4. Update Context with current state

---
*Last updated: 2026-05-11 after project initialization*

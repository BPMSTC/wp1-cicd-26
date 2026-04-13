# Testing Guide for Students

This document explains the three test layers in this repo and how to run them locally.

## Test types in this project

### Unit tests

**What they are:**
Unit tests verify a small piece of logic (usually one class or one method) in isolation.

**In this repo:**
- Project: `GalacticMissionControl.Tests.Unit`
- Focus: service-level behavior in `MissionService`
- Typical speed: fastest

Use unit tests when you want quick feedback on business logic without running the full web stack.

### Integration tests

**What they are:**
Integration tests verify how components work together (for example, routing + controllers + EF Core + serialization).

**In this repo:**
- Project: `GalacticMissionControl.Tests.Integration`
- Focus: HTTP endpoints and full ASP.NET Core request pipeline
- Uses: isolated SQLite in-memory test database (not live Azure SQL)

Use integration tests when you want confidence that the app wiring and request flow are correct.

### End-to-end (E2E) tests

**What they are:**
E2E tests automate the app the way a user uses it in a browser (click links, submit forms, read page text).

**In this repo:**
- Project: `GalacticMissionControl.Tests.E2E`
- Framework: Playwright for .NET
- Focus: user workflows such as navigating pages and creating missions

Use E2E tests when you want to validate real user behavior across the complete stack.

## Why Arrange-Act-Assert (AAA) matters

Arrange-Act-Assert is a test structure that makes tests easier to read and debug:

1. **Arrange**: Set up inputs, data, and dependencies.
2. **Act**: Execute the behavior under test.
3. **Assert**: Verify the expected result.

Why this helps students:
- Makes test intent obvious at a glance.
- Reduces accidental setup/assertion mixing.
- Makes failures easier to diagnose because each stage is clearly separated.
- Creates consistency across unit, integration, and E2E tests.

## Commands to run tests

From the repository root (`/workspace/wp1-cicd-26`):

### Run all tests in the solution

```bash
dotnet test GalacticMissionControl.sln
```

### Run only unit tests

```bash
dotnet test GalacticMissionControl.Tests.Unit/GalacticMissionControl.Tests.Unit.csproj
```

### Run only integration tests

```bash
dotnet test GalacticMissionControl.Tests.Integration/GalacticMissionControl.Tests.Integration.csproj
```

### Run only E2E tests

```bash
dotnet test GalacticMissionControl.Tests.E2E/GalacticMissionControl.Tests.E2E.csproj
```

## Playwright setup notes

Playwright E2E tests require browser binaries. Do this once after restore:

```bash
dotnet restore GalacticMissionControl.sln
pwsh GalacticMissionControl.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install --with-deps
```

If PowerShell is unavailable, you can try:

```bash
playwright install --with-deps
```

Tip: if `playwright.ps1` is missing, run a build/test of the E2E project first so the script is generated under `bin/Debug/net10.0/`.

## Troubleshooting (common student issues)

### 1) `dotnet` command not found
- Install the .NET 10 SDK.
- Reopen your terminal after installation.
- Verify with:
  ```bash
  dotnet --version
  ```

### 2) E2E tests fail with browser-not-installed errors
- Run Playwright install again:
  ```bash
  pwsh GalacticMissionControl.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install --with-deps
  ```
- Then rerun E2E tests.

### 3) PowerShell (`pwsh`) not found
- Install PowerShell, or use:
  ```bash
  playwright install --with-deps
  ```

### 4) Test failures due to stale build output
- Clean and rebuild:
  ```bash
  dotnet clean GalacticMissionControl.sln
  dotnet build GalacticMissionControl.sln
  ```

### 5) Port/process conflicts when running the web app manually
- Stop old `dotnet` processes and rerun tests.
- Prefer running tests directly (they manage their own host setup).

---
If you are new to testing, start with unit tests, then integration tests, and finally E2E tests.

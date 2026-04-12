# End-to-end tests (Playwright for .NET)

This project provides browser-driven end-to-end tests for student demos.

## What it covers

- Home page load
- Mission list page load
- Navigation from mission board to create page
- Successful mission creation
- Validation messages when required fields are missing

## Setup

1. Install the .NET 10 SDK.
2. Restore packages from the repo root:

   ```bash
   dotnet restore GalacticMissionControl.sln
   ```

3. Install Playwright browsers (run once after restore):

   ```bash
   pwsh GalacticMissionControl.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install --with-deps
   ```

   If you are using bash instead of PowerShell:

   ```bash
   playwright install --with-deps
   ```

## Run the E2E tests

```bash
dotnet test GalacticMissionControl.Tests.E2E/GalacticMissionControl.Tests.E2E.csproj
```

## Stability notes for demos

- Tests run headless Chromium for consistency.
- Each test starts from a freshly seeded SQLite in-memory database.
- Assertions wait on UI state (headings, URLs, and text) to reduce timing flakiness.

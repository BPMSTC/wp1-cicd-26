# GalacticMissionControl

PR 1 scaffolds a .NET 10 ASP.NET Core MVC solution with a single web project named `GalacticMissionControl.Web`.

## Run locally

1. Install the .NET 10 SDK.
2. From the repo root, run:

```bash
dotnet restore GalacticMissionControl.sln
dotnet run --project GalacticMissionControl.Web
```

3. Open `http://localhost:5121` (or the HTTPS URL shown in output).

## End-to-end tests (PR 9)

A Playwright for .NET E2E project is included at `GalacticMissionControl.Tests.E2E`.

### One-time setup

```bash
dotnet restore GalacticMissionControl.sln
pwsh GalacticMissionControl.Tests.E2E/bin/Debug/net10.0/playwright.ps1 install --with-deps
```

### Run E2E tests

```bash
dotnet test GalacticMissionControl.Tests.E2E/GalacticMissionControl.Tests.E2E.csproj
```

## Notes

- MVC is enabled with controllers and views.
- No authentication is configured.
- Integration and E2E tests use isolated SQLite test databases for repeatable demos.

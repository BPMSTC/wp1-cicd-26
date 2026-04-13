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

## Student testing documentation (PR 10)

See [`TESTING.md`](TESTING.md) for a student-friendly guide to unit, integration, and end-to-end testing, including Playwright setup and troubleshooting.

## Dependency security gate (PR 12)

CI now includes a `dependency-security` job in [`.github/workflows/ci.yml`](.github/workflows/ci.yml) that runs:

```bash
dotnet list GalacticMissionControl.sln package --vulnerable --include-transitive --format json
```

What it does:

- Scans NuGet dependencies (including transitive packages) for known vulnerabilities.
- Uploads a `dependency-vulnerability-report.json` artifact in the workflow run.
- Fails the workflow if any **high** or **critical** severity vulnerabilities are detected.

Where students can see results:

- **Pull requests:** the `dependency-security` check status appears with other CI checks.
- **Actions tab:** open a run to see the gate logs and download the vulnerability report artifact.

## Code scanning with CodeQL (PR 13)

This repository is configured with GitHub CodeQL for **C#** in [`.github/workflows/codeql.yml`](.github/workflows/codeql.yml).

Where students can view code scanning results in GitHub:

- **Security tab → Code scanning alerts:** repository-wide open/closed alerts.
- **Pull requests:** CodeQL check status appears alongside other CI checks.
- **Actions tab:** workflow logs for each CodeQL analysis run.

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

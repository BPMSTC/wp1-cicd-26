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

## Notes

- MVC is enabled with controllers and views.
- No authentication is configured.
- No database is configured.

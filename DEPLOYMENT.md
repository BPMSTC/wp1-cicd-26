# Azure App Service deployment setup

This repository deploys `GalacticMissionControl.Web` to Azure App Service using GitHub Actions OIDC (federated credentials), **not** a publish profile.

## 1) What goes where (quick map)

Use this map when you are deciding where to store configuration:

- **Azure App Service → Configuration → Application settings**
  - Use for non-secret runtime settings (for example, `ASPNETCORE_ENVIRONMENT`).
  - Can also store secret values, but for database strings use the dedicated **Connection strings** area below.
- **Azure App Service → Configuration → Connection strings**
  - Store the SQL connection string here.
  - Name it `DefaultConnection` so it maps to `ConnectionStrings:DefaultConnection` in the app.
- **GitHub repo/environment → Variables or Secrets**
  - Store workflow input values used by GitHub Actions (Azure IDs and app name).

> Why `DefaultConnection`? The app reads `builder.Configuration.GetConnectionString("DefaultConnection")` in `Program.cs`.

## 2) Azure prerequisites

1. Create or choose an Azure App Service (Web App) for Linux/Windows that will host the app.
2. Create an Azure AD app registration (or user-assigned managed identity) for GitHub Actions.
3. Grant that identity access to the target Web App (for example, `Contributor` on the App Service resource group).
4. Add a **Federated credential** to that identity:
   - **Issuer:** `https://token.actions.githubusercontent.com`
   - **Subject:** `repo:<OWNER>/<REPO>:environment:production`
   - **Audience:** `api://AzureADTokenExchange`
5. In your App Service, set these runtime values:
   - **Connection strings:** add `DefaultConnection` with your Azure SQL connection string.
   - **Application settings:** add any non-secret app configuration your environment needs.

## 3) GitHub prerequisites (required for workflows)

Create a `production` environment in GitHub, then add the following as **Variables** (recommended) or **Secrets**:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_WEBAPP_NAME`

These values are required by `.github/workflows/deploy-azure-app-service.yml` for `azure/login` and `azure/webapps-deploy`.

## 4) Why secrets must never be committed

Never commit real passwords, tokens, or production connection strings to Git:

- Git history is hard to fully erase once pushed.
- Anyone with repo/history access may recover exposed credentials.
- Secret leaks can lead to database compromise, unexpected cloud costs, and incident response work.

Safer approach:

- Keep repository config files free of real credentials (only placeholders/examples).
- Put production SQL credentials in **Azure App Service Connection strings**.
- Put workflow-sensitive values in **GitHub Secrets/Variables**.

## 5) Deployment behavior

- CI (`.github/workflows/ci.yml`) runs on pushes to `main` and pull requests.
- Deployment (`.github/workflows/deploy-azure-app-service.yml`) runs only when CI completes successfully for a `push` on `main`.
- The workflow publishes `GalacticMissionControl.Web/GalacticMissionControl.Web.csproj` and deploys it to Azure App Service.

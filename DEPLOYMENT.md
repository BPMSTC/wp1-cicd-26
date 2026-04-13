# Azure App Service deployment setup

This repository deploys `GalacticMissionControl.Web` to Azure App Service using GitHub Actions OIDC (federated credentials), **not** a publish profile.

## 1) Azure prerequisites

1. Create or choose an Azure App Service (Web App) for Linux/Windows that will host the app.
2. Create an Azure AD app registration (or user-assigned managed identity) for GitHub Actions.
3. Grant that identity access to the target Web App (for example, `Contributor` on the App Service resource group).
4. Add a **Federated credential** to that identity:
   - **Issuer:** `https://token.actions.githubusercontent.com`
   - **Subject:** `repo:<OWNER>/<REPO>:environment:production`
   - **Audience:** `api://AzureADTokenExchange`

## 2) GitHub prerequisites

Create a `production` environment in GitHub, then add these repository or environment **Variables** (or Secrets):

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_WEBAPP_NAME`

> The deployment workflow reads these values and logs in with OIDC via `azure/login`.

## 3) Deployment behavior

- CI (`.github/workflows/ci.yml`) runs on pushes to `main`.
- Deployment (`.github/workflows/deploy-azure-app-service.yml`) runs only when CI completes successfully for a `push` on `main`.
- The workflow publishes `GalacticMissionControl.Web/GalacticMissionControl.Web.csproj` and deploys it to Azure App Service.

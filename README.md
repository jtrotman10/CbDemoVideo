# CbDemoVideo

This repository demonstrates two deployment and authentication patterns for an ASP.NET Core app hosted in Azure App Service with Azure SQL and Azure Storage.

## Branch strategy

### `main` branch (secret-based)

The `main` branch represents the traditional secret-based setup:

- SQL and Storage connection details (including secrets) are stored in configuration.
- GitHub Actions deploys using a publish profile stored in GitHub Actions secrets.

This is the baseline approach many teams start with.

### `no-secrets` branch (managed identity-based)

The `no-secrets` branch demonstrates a no-secret pattern:

- Deployment to Azure uses GitHub OIDC + federated credentials (no publish profile secret).
- Azure App Service uses Managed Identity to access:
  - Azure SQL
  - Azure Storage (Blob)
- Connection settings in app config contain endpoints/metadata only, not passwords or account keys.

This branch is paired with the `no-secrets` App Service deployment slot to show the end-to-end managed identity flow in isolation.

## What this demo is proving

1. You can run a working App Service + Azure SQL + Blob app with the traditional secret model (`main`).
2. You can move the same app to a managed identity model (`no-secrets`) for both CI/CD authentication and runtime resource access.
3. The managed identity path removes long-lived credentials from source control and pipeline secrets.

# EnterpriseEmployeeManagementAPI

An ASP.NET Core Web API foundation targeting .NET 10 and C# 14.

## Prerequisites

- .NET 10 SDK

## Run locally

```powershell
dotnet restore
dotnet build --no-restore
dotnet run --project EnterpriseEmployeeManagementAPI
```

In Development, Swagger UI is available at `/swagger` and the health endpoint is available at `/health`.

This `main` branch intentionally contains platform configuration only. Business modules and delivery automation are developed on feature branches.

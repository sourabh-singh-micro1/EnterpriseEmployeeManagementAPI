# EnterpriseEmployeeManagementAPI

A production-minded Employee Management REST API built with ASP.NET Core 10, C# 14, Entity Framework Core, and SQL Server.

## Features

- Versioned controller-based REST API
- Employee CRUD and filtered search
- EF Core SQL Server persistence with repository and service layers
- FluentValidation request validation
- Structured Serilog request and application logs
- RFC 7807-style global exception responses
- Swagger/OpenAPI documentation
- Liveness and database-readiness health checks
- Development seed data
- xUnit, Moq, and FluentAssertions unit tests with Cobertura coverage
- GitHub Actions build, test, CodeQL, and dependency vulnerability checks

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server 2019 or newer, SQL Server Express, or LocalDB
- Git

## Project structure

```text
EnterpriseEmployeeManagementAPI.sln
├── EnterpriseEmployeeManagementAPI/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Interfaces/
│   ├── Repositories/
│   ├── Services/
│   ├── Validators/
│   ├── Middleware/
│   ├── HealthChecks/
│   └── Configuration/
└── EnterpriseEmployeeManagementAPI.Tests/
```

## Configuration

The application reads settings from `appsettings.json`, environment-specific files, environment variables, and .NET user secrets. Production secrets must not be committed.

Set the SQL Server connection string with an environment variable:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost,1433;Database=EnterpriseEmployeeManagementAPI;User Id=sa;Password=<strong-password>;TrustServerCertificate=True"
```

Set the JWT configuration placeholder values used by the configuration model:

```powershell
$env:Jwt__Issuer = "EnterpriseEmployeeManagementAPI"
$env:Jwt__Audience = "EnterpriseEmployeeManagementAPI.Clients"
$env:Jwt__SecretKey = "<at-least-32-characters-from-a-secret-store>"
```

The Development profile defaults to Windows LocalDB and seeds three departments and three employees on first start. To disable startup seeding:

```powershell
$env:Database__SeedOnStartup = "false"
```

## Restore, build, and run

```powershell
dotnet restore EnterpriseEmployeeManagementAPI.sln
dotnet build EnterpriseEmployeeManagementAPI.sln --configuration Release --no-restore
dotnet run --project EnterpriseEmployeeManagementAPI
```

The launch profile uses:

- HTTPS: `https://localhost:7199`
- HTTP: `http://localhost:5156`

## API documentation

In Development, Swagger UI is available at:

```text
https://localhost:7199/swagger
```

The Employee endpoints are versioned under `/api/v1/employees`:

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/employees` | Retrieve all employees |
| `GET` | `/api/v1/employees/{id}` | Retrieve an employee by ID |
| `GET` | `/api/v1/employees/search?query=&departmentId=&isActive=` | Search and filter employees |
| `POST` | `/api/v1/employees` | Create an employee |
| `PUT` | `/api/v1/employees/{id}` | Update an employee |
| `DELETE` | `/api/v1/employees/{id}` | Delete an employee |

Successful create requests return `201 Created`; updates and reads return `200 OK`; deletes return `204 No Content`. Invalid requests return `400`, missing records return `404`, duplicate email or employee-number conflicts return `409`, and unhandled failures return an `application/problem+json` response.

## Health checks

- `/health/live` verifies the process is running.
- `/health/ready` verifies that SQL Server is reachable.

## Testing and coverage

Run the complete test suite:

```powershell
dotnet test EnterpriseEmployeeManagementAPI.sln --configuration Release
```

Generate TRX results and Cobertura coverage:

```powershell
dotnet test EnterpriseEmployeeManagementAPI.sln `
  --configuration Release `
  --logger "trx;LogFileName=test-results.trx" `
  --results-directory TestResults `
  --collect "XPlat Code Coverage" `
  -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
```

The tests cover the service layer, EF Core repository behavior, and controller status-code behavior.

## CI/CD workflows

The workflows run for every pull request and every push to `main`:

- `build.yml` restores, builds in Release mode, publishes the API, and uploads the published artifact.
- `test.yml` runs all unit tests, fails on any test failure, and uploads TRX and Cobertura artifacts.
- `codeql.yml` performs C# CodeQL analysis, scans NuGet dependencies, uploads a vulnerability report, and rejects critical dependency findings. Pull requests also receive GitHub dependency review.

Dependabot checks NuGet and GitHub Actions dependencies weekly.

## Contributing

1. Create a focused branch from the latest `main`.
2. Keep controllers thin and place business logic in services.
3. Use asynchronous APIs and pass cancellation tokens through data-access calls.
4. Add or update tests for every behavior change.
5. Run Release build and tests locally.
6. Open a pull request using the repository template and address CODEOWNERS review.

CODEOWNERS currently maps the API, backend, DevOps, and solution-owner responsibilities to the repository owner so review requests work in this personal repository. Replace those entries with organization teams when the repository moves into an organization.

## License

This project is licensed under the [MIT License](LICENSE).

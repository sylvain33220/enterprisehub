# EnterpriseHub

Enterprise-grade **C# / .NET 8** backend API built with **Clean Architecture**, **EF Core**, **Docker**, and **PostgreSQL**.

## Tech Stack
- .NET 8 (ASP.NET Core Web API)
- Clean Architecture (Api / Application / Domain / Infrastructure)
- Entity Framework Core
- PostgreSQL (Docker Compose)
- Signed commits (SSH)

## Project Structure
- `src/EnterpriseHub.Api` — HTTP API (controllers, Swagger, DI)
- `src/EnterpriseHub.Application` — use cases / services / DTOs (coming next)
- `src/EnterpriseHub.Domain` — entities & business rules
- `src/EnterpriseHub.Infrastructure` — EF Core DbContext, persistence, migrations
- `tests/EnterpriseHub.Tests` — unit tests

## Getting Started

### 
1) Start PostgreSQL (Docker)
```bash
docker compose up -d
docker ps

2) Configure the database provider

Edit:
src/EnterpriseHub.Api/appsettings.Development.json

Example (PostgreSQL):

{
  "Database": { "Provider": "postgres" },
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=enterprisehub;Username=postgres;Password=postgres"
  }
}


The database provider is centralized in Infrastructure.Extensions.AddDatabase(...).

3) Apply migrations
dotnet ef database update -p src/EnterpriseHub.Infrastructure -s src/EnterpriseHub.Api

4) Run the API
dotnet run --project src/EnterpriseHub.Api


Swagger: http://localhost:5077/swagger

Health: http://localhost:5077/health

Switching to another DB provider

The DB wiring is centralized in:
EnterpriseHub.Infrastructure/Extensions/DatabaseServiceCollectionExtensions.cs

To enable MySQL, install a compatible EF provider (e.g. Pomelo) and add UseMySql(...).
SQL Server can be enabled by installing Microsoft.EntityFrameworkCore.SqlServer and using UseSqlServer(...).

Roadmap

 Auth (JWT) - register/login/me

 CRUD: clients / projects / tickets

 Validation + error handling

 Unit tests on services

 CI (GitHub Actions)


---


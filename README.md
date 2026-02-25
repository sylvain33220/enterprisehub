# Poteaux sylvain
# poteaux.sylvain@gmail.com
# https://studio-purple.com
# EnterpriseHub

**Enterprise-grade C# / .NET 8 backend API** built with Clean Architecture, Domain-Driven principles, EF Core, JWT authentication, Docker, and PostgreSQL.

This project demonstrates a production-ready backend architecture with:
- strict separation of concerns
- business rules isolated in the Domain layer
- application-level unit tests (no DB coupling)
- fully dockerized environment
- CI integration

---

## ✨ Key Features

- Clean Architecture (Api / Application / Domain / Infrastructure)
- Domain entities with business rules
- JWT authentication (access token)
- EF Core + PostgreSQL
- Dapper (read side for dashboard queries)
- Dockerized environment (API + DB)
- Application-layer unit tests (no EF / no DB)
- Centralized database provider switching
- SSH signed Git commits
- GitHub Actions CI
- Swagger documentation with real payload examples

---

## 🧱 Tech Stack

- .NET 8 — ASP.NET Core Web API
- Entity Framework Core
- Dapper (read queries)
- PostgreSQL (Docker Compose)
- JWT Bearer Authentication
- xUnit + FluentAssertions
- Docker / docker-compose
- GitHub Actions CI
- SSH signed commits

---

## 🗂 Project Structure


src/
├── EnterpriseHub.Api # HTTP API, controllers, auth, Swagger
├── EnterpriseHub.Application # Use cases, services, DTOs, ports
├── EnterpriseHub.Domain # Entities, enums, business rules
└── EnterpriseHub.Infrastructure # EF Core, DbContext, repositories

tests/
└── EnterpriseHub.Application.Tests
├── ClientServiceTests
├── ProjectServiceTests
└── TicketServiceTests

docs/
└── api.md # Complete API documentation


---

# 🚀 Getting Started

## 1️⃣ Start PostgreSQL (Docker)

```bash
docker compose up -d
docker ps
2️⃣ Configure Database Provider

Edit:

src/EnterpriseHub.Api/appsettings.Development.json

Example:

{
  "Database": { "Provider": "postgres" },
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=enterprisehub;Username=postgres;Password=postgres"
  }
}

Database wiring is centralized in:

EnterpriseHub.Infrastructure/Extensions/DatabaseServiceCollectionExtensions.cs
3️⃣ Apply Migrations
dotnet ef database update \
  -p src/EnterpriseHub.Infrastructure \
  -s src/EnterpriseHub.Api
4️⃣ Run the API
dotnet run --project src/EnterpriseHub.Api

Swagger UI:
👉 http://localhost:5077/swagger

Health endpoint:
👉 http://localhost:5077/health

🐳 Docker Usage
Development
docker compose down
docker compose up --build
docker logs -f enterprisehub-api
Reset database
docker compose down -v
docker compose up --build
Production-like Run

Create a .env file at repository root (not committed):

POSTGRES_PASSWORD=change_me
JWT_KEY=change_me_super_secret_32chars_min

Run:

docker compose -f docker-compose.prod.yml up -d --build

Stop:

docker compose -f docker-compose.prod.yml down
📊 Dashboard Endpoints

GET /api/dashboard/overview

GET /api/dashboard/tickets-by-status

Dapper is used for read-side optimized queries.

🗃 Database Naming Conventions

Business tables: users, clients, projects, tickets

EF internal table: __EFMigrationsHistory

Columns: MigrationId, ProductVersion

🧪 Running Tests

Application-layer unit tests only (no EF, no DB).

dotnet test tests/EnterpriseHub.Application.Tests

Covered services:

ClientService

ProjectService

TicketService

🔁 Switching Database Provider

Database provider selection is centralized.

Supported:

PostgreSQL (default)

MySQL (Pomelo)

To switch:

Install EF provider

Update AddDatabase(...)

Update configuration

No application or domain code needs to change.

🔐 Security

JWT Bearer authentication

Role-based user model

No secrets committed

.env support for production

Proper .gitignore configuration

📌 Project Purpose

This repository demonstrates:

Clean backend architecture

Professional .NET coding standards

Testable business logic

Production-ready Docker setup

CI integration

Enterprise-level backend design

It serves as a portfolio and technical reference project.
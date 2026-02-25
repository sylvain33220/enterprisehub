# Poteaux sylvain
# poteaux.sylvain@gmail.com

# EnterpriseHub

Enterprise-grade **C# / .NET 8** backend API built with **Clean Architecture**, Domain-driven principles, **EF Core**, **JWT authentication**, **Docker**, and **PostgreSQL**.

This project demonstrates a production-ready backend architecture with clear separation of concerns, business rules in the domain, application-level unit tests, and fully documented APIs.

**✨ Key Features**
Clean Architecture (Api / Application / Domain / Infrastructure)

Domain entities with business rules

JWT authentication (access token)

Entity Framework Core + PostgreSQL

Dockerized database

Application-layer unit tests (no EF / no DB)

Centralized database provider switching

Signed Git commits (SSH)

API documentation with real payload examples

**🧱 Tech Stack**

.NET 8 — ASP.NET Core Web API

Entity Framework Core

PostgreSQL (Docker Compose)

JWT Bearer Authentication

xUnit + FluentAssertions (unit tests)

Docker

GitHub + SSH signed commits

**🗂 Project Structure**
src/
 ├── EnterpriseHub.Api           # HTTP API, controllers, auth, Swagger
 ├── EnterpriseHub.Application   # Use cases, services, DTOs, ports
 ├── EnterpriseHub.Domain        # Entities, enums, business rules
 └── EnterpriseHub.Infrastructure# EF Core, DbContext, repositories

tests/
 └── EnterpriseHub.Application.Tests
     ├── ClientServiceTests
     ├── ProjectServiceTests
     └── TicketServiceTests

docs/
 └── api.md                      # Complete API documentation

**🚀 Getting Started**
1) Start PostgreSQL (Docker)
docker compose up -d
docker ps

2) Configure database provider

Edit:

src/EnterpriseHub.Api/appsettings.Development.json


Example (PostgreSQL):

{
  "Database": { "Provider": "postgres" },
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=enterprisehub;Username=postgres;Password=postgres"
  }
}


Database wiring is centralized in:

EnterpriseHub.Infrastructure/Extensions/DatabaseServiceCollectionExtensions.cs

3) Apply migrations
dotnet ef database update \
  -p src/EnterpriseHub.Infrastructure \
  -s src/EnterpriseHub.Api

4) Run the API
dotnet run --project src/EnterpriseHub.Api

5) docker
docker compose down
docker compose up --build
docker logs -f enterprisehub-api

Swagger UI:
👉 http://localhost:5077/swagger

Health endpoint:
👉 http://localhost:5077/health

**🧪 Running Tests**

Unit tests cover application services only (no EF, no DB, no mocks).

dotnet test tests/EnterpriseHub.Application.Tests


Covered services:

ClientService

ProjectService

TicketService

Each test validates real business rules (uniqueness, dependencies, state changes).

**📘 API Documentation**

Full API documentation with:

endpoints

payloads

responses

error formats

**📄 See:**

docs/api.md

**🔁 Switching Database Provider**

Database provider selection is centralized.

Supported / ready:

PostgreSQL (default)

MySQL (Pomelo)

SQL Server

To switch:

Install EF provider

Update AddDatabase(...)

Change config

No application or domain code needs to change.

**🔐 Security**

JWT Bearer authentication

Role-based user model

No secrets committed

.gitignore configured for local configs

**📌 Project Purpose**

This repository was built to demonstrate:

clean backend architecture

professional .NET coding standards

testable business logic

API design clarity

It is intended as a portfolio / technical reference project.
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

### 1) Start PostgreSQL (Docker)
```bash
docker compose up -d
docker ps

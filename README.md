# TodoApp — Microsoft To-Do style application

Full-stack To-Do application: **ASP.NET Core Web API (.NET 10)** + **MS SQL Server** + **Angular** + **Tailwind CSS**.

> 🚧 Work in progress. Full setup instructions, screenshots and feature list will be added at the end.

## Architecture

The backend is split into **4 layers**, each one is a separate project:

| Project | Responsibility |
|---|---|
| `TodoApp.Api` | Controllers, `Program.cs`, configuration, middleware. Composition root (DI registration). |
| `TodoApp.Services` | Business logic — implementations of service interfaces. |
| `TodoApp.Interfaces` | Contracts: service and repository interfaces, DTOs, entities, custom exceptions. |
| `TodoApp.DataAccess` | EF Core `DbContext`, entity configurations, migrations, repository implementations. |

### Project dependencies

```
                 ┌───────────────────┐
                 │    TodoApp.Api    │
                 │ (composition root)│
                 └──┬─────────────┬──┘
                    │             │
                    ▼             ▼
       ┌────────────────┐   ┌────────────────────┐
       │TodoApp.Services│   │ TodoApp.DataAccess │
       │ business logic │   │ EF Core, SQL Server│
       └───────┬────────┘   └─────────┬──────────┘
               │                      │
               ▼                      ▼
             ┌──────────────────────────┐
             │    TodoApp.Interfaces    │
             │ interfaces, DTOs, entities│
             │  (depends on nothing)    │
             └──────────────────────────┘
```

All arrows point **towards `TodoApp.Interfaces`** (Dependency Inversion Principle):

- `Services` depends only on abstractions (`ITaskRepository`, ...) and knows nothing about EF Core or SQL.
- `DataAccess` implements the repository interfaces declared in `Interfaces`.
- `Api` references everything only to wire implementations into the DI container.
- There are no circular references — the compiler enforces it.

## Running locally (current state)

Requirements: **.NET 10 SDK**, **Docker Desktop**.

```bash
# 1. Start SQL Server
docker compose up -d

# 2. Build and run the API
cd server
dotnet build
dotnet run --project TodoApp.Api   # http://localhost:5000
```

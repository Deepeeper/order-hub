# OrderHub

Internal order management for steel products. Reference app on Ovako's tech stack.

## Stack

- **Frontend**: Blazor Server (.NET 10, Interactive Server)
- **Backend**: ASP.NET Core 10, Minimal APIs
- **Persistence**: Entity Framework Core 10 against SQLite (local file)
- **Architecture**: Modular monolith, clean architecture (Domain → Application → Infrastructure → Api)

## Structure

```
OrderHub/
├── src/
│   ├── Backend/
│   │   ├── OrderHub.Domain/          # Entities, value types
│   │   ├── OrderHub.Application/     # Repository interfaces
│   │   ├── OrderHub.Infrastructure/  # EF Core, DbContext, repositories, seed
│   │   └── OrderHub.Api/             # Minimal APIs
│   └── Frontend/
│       └── OrderHub.Blazor/          # Blazor Server app
├── tests/
│   ├── OrderHub.Api.Tests/           # WebApplicationFactory-based integration tests
│   └── OrderHub.Domain.Tests/        # (no tests yet)
├── data/                              # SQLite database lives here
└── OrderHub.sln
```

## Running locally

Two processes are needed — the API and the Blazor frontend.

### Option 1 — Rider

The solution ships with a Compound run configuration named **OrderHub (full stack)** in `.run/`. Pick it from the run-configuration dropdown and press play — both projects start at once.

### Option 2 — CLI, two terminals

**Terminal 1 — API**

```bash
dotnet run --project src/Backend/OrderHub.Api
```

The API listens on `http://localhost:5101`. On first startup the SQLite database is created and seeded in `data/orderhub.db`.

**Terminal 2 — Blazor**

```bash
dotnet run --project src/Frontend/OrderHub.Blazor
```

The frontend listens on `http://localhost:5102` and calls the API.

### Running tests

```bash
dotnet test
```

## Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET    | `/`                              | Service info |
| GET    | `/api/orders?status=`            | List orders, optional status filter |
| GET    | `/api/orders/{id}`               | Get one order with lines |
| POST   | `/api/orders`                    | Create a new order |
| PATCH  | `/api/orders/{id}/status`        | Update order status |
| GET    | `/api/customers`                 | List customers |
| GET    | `/api/customers/{id}`            | Get one customer |
| GET    | `/api/products`                  | List products |
| GET    | `/api/products/{id}`             | Get one product |

## Sample data

On first run the database is seeded with:
- 8 customers
- 8 products (round, flat and hexagonal bars in various steel grades)
- 24 orders with 1–3 lines each, spread across all four statuses

The random seed is fixed so the data looks the same every time the database is recreated.

## Status

This is a **reference app under construction**. There are intentional gaps and rough edges — the app is not production-ready, and is not meant to be.

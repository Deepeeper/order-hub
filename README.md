# OrderHub

Intern orderhantering för stålprodukter. Referensapp i Ovakos teknikstack.

## Stack

- **Frontend**: Blazor Server (.NET 9, Interactive Server)
- **Backend**: ASP.NET Core 9, Minimal APIs
- **Persistens**: Entity Framework Core 9 mot SQLite (lokal fil)
- **Arkitektur**: Modulär monolit, clean architecture (Domain → Application → Infrastructure → Api)

## Struktur

```
OrderHub/
├── src/
│   ├── Backend/
│   │   ├── OrderHub.Domain/          # Entities, value types
│   │   ├── OrderHub.Application/     # Repository-interfaces
│   │   ├── OrderHub.Infrastructure/  # EF Core, DbContext, repositories, seed
│   │   └── OrderHub.Api/             # Minimal APIs
│   └── Frontend/
│       └── OrderHub.Blazor/          # Blazor Server-app
├── tests/
│   ├── OrderHub.Api.Tests/           # WebApplicationFactory-baserade integrationstester
│   └── OrderHub.Domain.Tests/        # (saknar tester)
├── data/                              # SQLite-databasen hamnar här
└── OrderHub.sln
```

## Köra lokalt

Två processer behövs — API:t och Blazor-frontend.

### Terminal 1 – API

```bash
dotnet run --project src/Backend/OrderHub.Api
```

API:t lyssnar på `http://localhost:5101`. Vid första uppstart skapas och seedas SQLite-databasen i `data/orderhub.db`.

### Terminal 2 – Blazor

```bash
dotnet run --project src/Frontend/OrderHub.Blazor
```

Frontend lyssnar på `http://localhost:5102` och anropar API:t.

### Köra tester

```bash
dotnet test
```

## Endpoints

| Metod | Path | Beskrivning |
|-------|------|-------------|
| GET   | `/`                              | Service-info |
| GET   | `/api/orders?status=`            | Lista ordrar, frivilligt filter |
| GET   | `/api/orders/{id}`               | Hämta en order med rader |
| POST  | `/api/orders`                    | Skapa ny order |
| PATCH | `/api/orders/{id}/status`        | Uppdatera orderstatus |
| GET   | `/api/customers`                 | Lista kunder |
| GET   | `/api/customers/{id}`            | Hämta kund |
| GET   | `/api/products`                  | Lista produkter |
| GET   | `/api/products/{id}`             | Hämta produkt |

## Sample data

Vid första körning seedas:
- 8 kunder
- 8 produkter (rundstänger och plattstång i olika stålgrader)
- 24 ordrar med 1-3 rader vardera, fördelade över alla fyra statusar

Frö för slumptal är fixerat så datan ser likadan ut varje gång databasen återskapas.

## Status

Detta är en **referensapp under uppbyggnad**. Det finns medvetna brister och oavslutade detaljer — appen är inte produktionsfärdig och ska inte heller vara det.

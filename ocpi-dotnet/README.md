# OCPI .NET API (Minimal Implementation)

This project contains a minimal OCPI-compliant API implemented in C# using ASP.NET Core with **SimSharp-based synthetic data generation** for realistic demo scenarios.

## Features

### Implemented endpoints (OCPI 2.2 compliant):
- GET /ocpi/versions — returns a list of supported OCPI versions.
- GET /ocpi/{version}/locations — returns ACME fleet charging locations with **pagination & filtering**
  - Query parameters: `date_from`, `date_to`, `offset`, `limit`
  - Returns `Link` header for next page
  - Returns `X-Total-Count` and `X-Limit` headers
- GET /ocpi/{version}/locations/{id} — returns a single location.
- GET /ocpi/{version}/cdrs — returns charge detail records with **pagination & filtering**
  - Query parameters: `date_from`, `date_to`, `offset`, `limit`
  - Returns `Link` header for next page
  - Returns `X-Total-Count` and `X-Limit` headers
- POST /ocpi/{version}/cdrs — post a CDR (charging data record).

### Synthetic Data Generation (ACME Fleet Demo)
- **SimSharp discrete event simulation** generates realistic charging scenarios
- **5 charging locations** across Netherlands (Amsterdam, Rotterdam, Utrecht, The Hague, Eindhoven)
- **20 fleet vehicles** with randomized charging sessions over 1 week
- **100+ CDRs** with realistic energy consumption and costs
- All records include `last_updated` timestamps for proper OCPI filtering
- Inspired by the GasStationRefueling example from SimSharp samples
- Uses **Bogus** library for realistic fake data (addresses, etc.)

### OCPI REST Standards Compliance
- **Pagination**: `offset` and `limit` query parameters (default limit=25, max=100)
- **Date Filtering**: `date_from` and `date_to` parameters filter by `last_updated` field
- **Link Headers**: `Link` header with `rel="next"` for pagination navigation
- **Metadata Headers**: `X-Total-Count` and `X-Limit` headers provide result info
- **RFC3339 Timestamps**: All datetime fields use ISO 8601 format

How to run locally (Windows PowerShell):

```powershell
cd C:\source\ocpi\ocpi-dotnet\Ocpi.Api
dotnet run --project Ocpi.Api.csproj
```

Open http://localhost:5000/swagger to see and test endpoints.

Run tests:

```powershell
cd C:\source\ocpi\ocpi-dotnet
dotnet test
```

This is a starting point and intentionally minimal. It is not a complete OCPI implementation but demonstrates how to start implementing OCPI module endpoints in a .NET API.

# OCPI C# Implementation

A fully functional **OCPI 2.2** (Open Charge Point Interface) compliant REST API built with **ASP.NET Core 8.0**, featuring synthetic data generation using **SimSharp** discrete event simulation and an **AdminLTE4** web dashboard.

## 🚀 Features

### OCPI 2.2 Compliance
- ✅ **Versions Endpoint**: `/ocpi/2.2/versions` - API version discovery
- ✅ **Locations Endpoint**: `/ocpi/2.2/locations` - Charging location data
- ✅ **CDRs Endpoint**: `/ocpi/2.2/cdrs` - Charge Detail Records
- ✅ **Pagination Support**: `offset`, `limit` parameters (OCPI standard)
- ✅ **Date Filtering**: `date_from`, `date_to` on `last_updated` field
- ✅ **HTTP Headers**: `Link` (RFC 5988), `X-Total-Count`, `X-Limit`
- ✅ **CORS Enabled**: Cross-origin requests for web dashboard

### Synthetic Data Generation
- 🎲 **SimSharp Simulation**: Discrete event simulation inspired by [SimPy](https://simpy.readthedocs.io/)
- 🚗 **ACME Fleet**: 20 electric vehicles charging across 5 locations
- 📍 **Real Locations**: Amsterdam, Rotterdam, Utrecht, The Hague, Eindhoven
- 📊 **7 Days of Data**: 100+ realistic charge sessions with:
  - Arrival patterns (15-120 min intervals)
  - Battery levels (10-40% on arrival, charge to 80%)
  - Charging speed (50 kW)
  - Pricing (€0.35/kWh)
  - Session durations and costs

### Web Dashboard
- 📈 **AdminLTE4**: Modern, responsive dashboard
- 🏠 **Dashboard**: Real-time statistics and recent sessions
- 📍 **Locations View**: Interactive table with Google Maps links
- 💰 **CDRs View**: Filterable charge records with OCPI pagination
- 🔄 **Live Data**: Fetches from API with CORS support

## 🏗️ Project Structure

```
ocpi/
├── ocpi-dotnet/              # ASP.NET Core Web API
│   └── Ocpi.Api/
│       ├── Controllers/      # OCPI endpoints (Versions, Locations, CDRs)
│       ├── Models/          # Data models (Location, Cdr, OcpiResponse)
│       ├── Services/        # FleetChargingSimulation (SimSharp)
│       └── Program.cs       # API configuration with CORS
├── www/                     # AdminLTE4 Dashboard
│   ├── index.html          # Dashboard homepage
│   ├── locations.html      # Locations viewer
│   ├── cdrs.html          # CDRs with filtering
│   └── README.md          # Dashboard documentation
├── OCPI/                   # OCPI specification (submodule)
└── README.md              # This file
```

## 🚦 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- Git

### Run the API
```powershell
cd ocpi-dotnet/Ocpi.Api
dotnet restore
dotnet run
```

API runs at: `http://localhost:5055`

### View the Dashboard
Open `www/index.html` in your browser, or serve with:
```powershell
cd www
python -m http.server 8000
```
Dashboard available at: `http://localhost:8000`

### Test the API
```bash
# Get all locations
curl http://localhost:5055/ocpi/2.2/locations

# Get CDRs with pagination
curl http://localhost:5055/ocpi/2.2/cdrs?limit=10&offset=0

# Filter by date
curl "http://localhost:5055/ocpi/2.2/cdrs?date_from=2025-11-05T00:00:00Z"
```

Or use Swagger UI: `http://localhost:5055/swagger`

## 📦 Dependencies

### Backend
- **ASP.NET Core 8.0** - Web framework
- **SimSharp 3.4.2** - Discrete event simulation
- **Bogus 35.6.5** - Fake data generation
- **Swashbuckle** - Swagger/OpenAPI documentation

### Frontend
- **AdminLTE 3.2** (CDN) - Dashboard UI framework
- **Bootstrap 4.6.2** - UI components
- **jQuery 3.7.0** - AJAX and DOM manipulation
- **DataTables 1.13.6** - Advanced table features
- **Font Awesome 6.4.0** - Icons

## 🎯 API Examples

### Locations
```json
GET /ocpi/2.2/locations?limit=2

{
  "data": [
    {
      "id": "ACME-LOC-001",
      "name": "ACME Fleet Charging - Amsterdam",
      "address": "123 Dam Square",
      "city": "Amsterdam",
      "country": "NL",
      "latitude": 52.370216,
      "longitude": 4.895168,
      "last_updated": "2025-11-05T00:00:00Z"
    }
  ],
  "status_code": 1000,
  "timestamp": "2025-11-12T14:30:00Z"
}
```

### CDRs with Pagination
```json
GET /ocpi/2.2/cdrs?offset=0&limit=1

Response Headers:
  Link: <http://localhost:5055/ocpi/2.2/cdrs?offset=1&limit=1>; rel="next"
  X-Total-Count: 150
  X-Limit: 1

{
  "data": [
    {
      "id": "CDR-abc123",
      "start_date_time": "2025-11-05T08:30:00Z",
      "end_date_time": "2025-11-05T10:15:00Z",
      "total_energy": 45.5,
      "total_cost": 15.93,
      "last_updated": "2025-11-05T10:17:00Z"
    }
  ],
  "status_code": 1000,
  "timestamp": "2025-11-12T14:30:00Z"
}
```

## 🧪 SimSharp Simulation Details

The `FleetChargingSimulation` class generates realistic charging patterns:

```csharp
// Configuration
NumberOfVehicles = 20;
NumberOfChargingStations = 5;
SimulationDuration = 7 days;
ChargingSpeed = 50 kW;
BatteryCapacity = 75 kWh;

// Vehicle behavior
- Arrival: UNIF(15min, 120min)
- Initial battery: UNIF(10%, 40%)
- Target battery: 80%
- Pricing: €0.35/kWh
```

Each vehicle follows a discrete event process:
1. Wait for random interval
2. Select random charging station
3. Request charging spot (queue if busy)
4. Calculate energy needed
5. Charge for required duration
6. Release spot and create CDR

## 📚 Documentation

- **[OCPI REST Standards](ocpi-dotnet/OCPI-REST-STANDARDS.md)** - Implementation details
- **[Simulation Technical Summary](ocpi-dotnet/SIMULATION-TECHNICAL-SUMMARY.md)** - SimSharp design
- **[Dashboard README](www/README.md)** - Web interface documentation
- **[API README](ocpi-dotnet/README.md)** - Backend documentation

## 🔗 OCPI Specification

This implementation follows [OCPI 2.2 specification](https://github.com/ocpi/ocpi). The spec is included as a Git submodule in the `OCPI/` directory.

## 🛠️ Development

### Build
```powershell
cd ocpi-dotnet
dotnet build
```

### Run Tests (if available)
```powershell
dotnet test
```

### Generate More Data
Modify simulation parameters in `Services/FleetChargingSimulation.cs`:
- `NumberOfVehicles` - Fleet size
- `SimulationDuration` - Time period
- `ChargingSpeed` - kW charging rate
- `VehicleArrival` - Arrival distribution

## 📄 License

MIT License - See LICENSE file for details

## 🤝 Contributing

Contributions welcome! This is a demonstration project showcasing:
- OCPI 2.2 compliance
- SimSharp discrete event simulation
- Modern web dashboard integration
- Clean architecture with ASP.NET Core

## 🔮 Future Enhancements

- [ ] Authentication/Authorization (OCPI tokens)
- [ ] More OCPI modules (Sessions, Tariffs, Commands)
- [ ] Real-time updates via SignalR
- [ ] Database persistence (Entity Framework)
- [ ] Docker containerization
- [ ] Unit tests and integration tests
- [ ] CI/CD pipeline

---

**Built with** ❤️ **using SimSharp, ASP.NET Core, and AdminLTE**

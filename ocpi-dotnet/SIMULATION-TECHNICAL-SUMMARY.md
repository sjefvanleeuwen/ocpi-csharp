# ACME Fleet Charging Simulation - Technical Summary

## Overview
Created a SimSharp-based discrete event simulation that generates realistic charging data for the ACME fleet demonstration.

## Implementation Details

### Technology Stack
- **SimSharp 3.4.2**: Discrete event simulation framework (inspired by SimPy)
- **Bogus 35.6.5**: Fake data generation for addresses and names
- **ASP.NET Core 8.0**: Web API hosting

### Simulation Design (FleetChargingSimulation.cs)
Based on the GasStationRefueling example from SimSharp samples:

```
┌─────────────────────────────────────────────────────────────┐
│  ACME Fleet Charging Simulation (7 days)                    │
├─────────────────────────────────────────────────────────────┤
│  • 5 Charging Locations (Resource with capacity=2 each)     │
│  • 20 Fleet Vehicles (ACME-FLEET-001 through ACME-FLEET-020)│
│  • Vehicle arrival: Uniform(15 min, 120 min)                │
│  • Battery level on arrival: Uniform(10%, 40%)              │
│  • Target charge level: 80%                                 │
│  • Charging speed: 50 kW                                    │
│  • Pricing: €0.35 per kWh                                   │
└─────────────────────────────────────────────────────────────┘
```

### Generated Data

#### Locations (5 stations)
- ACME-LOC-001: Amsterdam (52.370216, 4.895168)
- ACME-LOC-002: Rotterdam (51.924420, 4.477733)
- ACME-LOC-003: Utrecht (52.092876, 5.104480)
- ACME-LOC-004: The Hague (52.078663, 4.288788)
- ACME-LOC-005: Eindhoven (51.441642, 5.469722)

Each location has:
- Realistic street addresses (via Bogus)
- GPS coordinates
- Capacity for 2 simultaneous charging sessions

#### CDRs (Charge Detail Records)
Approximately 100-150 CDRs generated over 7-day simulation period, each containing:
- Unique CDR ID
- Start and end timestamps
- Total energy consumed (kWh)
- Total cost (€)

### Key Simulation Patterns

1. **Resource Contention**: Vehicles wait if both charging spots are occupied
2. **Stochastic Arrivals**: Vehicle arrivals follow uniform distribution
3. **Variable Charging Sessions**: Duration depends on battery level and charging speed
4. **Discrete Events**: Simulation tracks:
   - Vehicle arrival at station
   - Request charging spot
   - Begin charging
   - Complete charging
   - Release charging spot

### API Integration

Controllers automatically initialize with simulated data on first load:
- `LocationsController`: Returns all 5 ACME charging locations
- `CdrsController`: Returns all generated CDRs + accepts new POSTs

## Demo Usage

```bash
# Start the API
cd c:\source\ocpi\ocpi-dotnet\Ocpi.Api
dotnet run

# Test endpoints (see ACME-Fleet-Demo.http)
curl http://localhost:5055/ocpi/2.2/locations
curl http://localhost:5055/ocpi/2.2/cdrs
```

## Benefits for Demo

✅ **Realistic Data**: Discrete event simulation models real-world charging behavior  
✅ **Reproducible**: Same random seed generates identical datasets  
✅ **Scalable**: Easy to adjust parameters (more vehicles, longer duration, etc.)  
✅ **OCPI Compliant**: Data structure matches OCPI 2.2 specification  
✅ **Production-Ready Pattern**: Shows how to integrate simulation for testing/demos  

## Future Enhancements

- Add tariff information
- Include session status (in-progress, completed, interrupted)
- Model different vehicle types (different battery capacities)
- Add peak/off-peak charging patterns
- Include authentication tokens
- Track individual vehicle usage patterns

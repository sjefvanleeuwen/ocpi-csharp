using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using SimSharp;
using Ocpi.Api.Models;
using static SimSharp.Distributions;

namespace Ocpi.Api.Services
{
    /// <summary>
    /// SimSharp-based simulation for generating synthetic ACME fleet charging data.
    /// Inspired by the GasStationRefueling example from SimSharp samples.
    /// </summary>
    public class FleetChargingSimulation
    {
        private const int RandomSeed = 42;
        private const int NumberOfChargingStations = 5;
        private const int NumberOfVehicles = 20;
        private static readonly TimeSpan SimulationDuration = TimeSpan.FromDays(7); // 1 week of data
        private static readonly UniformTime VehicleArrival = UNIF(TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(120));
        private const double ChargingSpeed = 50.0; // kW
        private static readonly Uniform BatteryLevel = UNIF(10, 40); // % battery when arriving
        private const double BatteryCapacity = 75.0; // kWh

        private readonly List<Location> _locations = new();
        private readonly List<Cdr> _cdrs = new();

        public (List<Location> Locations, List<Cdr> Cdrs) Generate()
        {
            // Generate charging locations using Bogus
            GenerateLocations();

            // Run the SimSharp simulation
            var env = new Simulation(DateTime.UtcNow.Date.AddDays(-7), RandomSeed);
            var chargingStations = _locations.Select(loc => new Resource(env, capacity: 2)).ToArray();

            // Start vehicle processes
            for (int i = 0; i < NumberOfVehicles; i++)
            {
                env.Process(Vehicle($"ACME-FLEET-{i:D3}", env, chargingStations));
            }

            // Run simulation
            env.Run(SimulationDuration);

            return (_locations, _cdrs);
        }

        private void GenerateLocations()
        {
            var faker = new Faker();
            
            // ACME fleet has 5 charging locations across Netherlands
            var cityNames = new[] { "Amsterdam", "Rotterdam", "Utrecht", "The Hague", "Eindhoven" };
            var coordinates = new[]
            {
                (52.370216, 4.895168),   // Amsterdam
                (51.924420, 4.477733),   // Rotterdam
                (52.092876, 5.104480),   // Utrecht
                (52.078663, 4.288788),   // The Hague
                (51.441642, 5.469722)    // Eindhoven
            };

            for (int i = 0; i < NumberOfChargingStations; i++)
            {
                _locations.Add(new Location
                {
                    Id = $"ACME-LOC-{i + 1:D3}",
                    Name = $"ACME Fleet Charging - {cityNames[i]}",
                    Address = faker.Address.StreetAddress(),
                    City = cityNames[i],
                    Country = "NL",
                    Latitude = coordinates[i].Item1,
                    Longitude = coordinates[i].Item2,
                    LastUpdated = DateTime.UtcNow.AddDays(-7).AddHours(i) // Stagger initial creation
                });
            }
        }

        private IEnumerable<Event> Vehicle(string vehicleId, Simulation env, Resource[] chargingStations)
        {
            var faker = new Faker();

            while (true)
            {
                // Wait before next charge session
                yield return env.Timeout(VehicleArrival);

                // Select a random charging station
                var stationIndex = faker.Random.Int(0, chargingStations.Length - 1);
                var station = chargingStations[stationIndex];
                var location = _locations[stationIndex];

                // Battery level when arriving (10-40%)
                var initialBatteryLevel = env.Rand(BatteryLevel);
                var energyNeeded = BatteryCapacity * ((80.0 - initialBatteryLevel) / 100.0); // Charge to 80%

                var sessionStart = env.Now;

                using (var req = station.Request())
                {
                    yield return req;

                    // Charging duration based on energy needed and charging speed
                    var chargingDuration = TimeSpan.FromHours(energyNeeded / ChargingSpeed);
                    yield return env.Timeout(chargingDuration);

                    var sessionEnd = env.Now;

                    // Calculate cost (€0.35 per kWh)
                    var totalCost = (decimal)(energyNeeded * 0.35);

                    // Create CDR
                    var cdr = new Cdr
                    {
                        Id = $"CDR-{Guid.NewGuid().ToString()[..8]}",
                        StartDateTime = sessionStart,
                        EndDateTime = sessionEnd,
                        TotalEnergy = (decimal)energyNeeded,
                        TotalCost = totalCost,
                        LastUpdated = sessionEnd.AddMinutes(2) // CDR updated shortly after session ends
                    };

                    _cdrs.Add(cdr);
                }
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Ocpi.Api.Models;
using Ocpi.Api.Services;
using System.Collections.Generic;
using System.Linq;

namespace Ocpi.Api.Controllers
{
    [ApiController]
    [Route("ocpi/{version}/[controller]")]
    public class LocationsController : ControllerBase
    {
        private static readonly List<Location> _data;

        static LocationsController()
        {
            // Generate synthetic ACME fleet charging data using SimSharp simulation
            var simulation = new FleetChargingSimulation();
            var (locations, _) = simulation.Generate();
            _data = locations;
        }

        [HttpGet]
        public ActionResult<OcpiResponse<IEnumerable<Location>>> Get(
            string version,
            [FromQuery] DateTime? date_from = null,
            [FromQuery] DateTime? date_to = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 25)
        {
            var queryParams = new OcpiQueryParameters
            {
                DateFrom = date_from,
                DateTo = date_to,
                Offset = offset,
                Limit = limit
            };
            queryParams.Validate();

            // Filter by date range if specified
            var filteredData = _data.AsEnumerable();
            if (queryParams.DateFrom.HasValue)
                filteredData = filteredData.Where(l => l.LastUpdated >= queryParams.DateFrom.Value);
            if (queryParams.DateTo.HasValue)
                filteredData = filteredData.Where(l => l.LastUpdated < queryParams.DateTo.Value);

            // Apply pagination
            var totalCount = filteredData.Count();
            var paginatedData = filteredData
                .OrderBy(l => l.LastUpdated)
                .Skip(queryParams.Offset)
                .Take(queryParams.Limit)
                .ToList();

            // Add Link header for pagination (OCPI standard)
            if (queryParams.Offset + queryParams.Limit < totalCount)
            {
                var nextOffset = queryParams.Offset + queryParams.Limit;
                var nextUrl = $"{Request.Scheme}://{Request.Host}{Request.Path}?offset={nextOffset}&limit={queryParams.Limit}";
                if (queryParams.DateFrom.HasValue)
                    nextUrl += $"&date_from={queryParams.DateFrom.Value:O}";
                if (queryParams.DateTo.HasValue)
                    nextUrl += $"&date_to={queryParams.DateTo.Value:O}";
                
                Response.Headers["Link"] = $"<{nextUrl}>; rel=\"next\"";
            }

            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Limit"] = queryParams.Limit.ToString();

            var res = new OcpiResponse<IEnumerable<Location>>
            {
                StatusCode = 1000,
                Data = paginatedData,
                Timestamp = DateTime.UtcNow.ToString("O")
            };
            return Ok(res);
        }

        [HttpGet("{id}")]
        public ActionResult<OcpiResponse<Location?>> GetById(string version, string id)
        {
            var item = _data.FirstOrDefault(l => l.Id == id);
            if (item == null)
                return NotFound(new OcpiResponse<Location?> { StatusCode = 2001, StatusMessage = "Not found", Data = null });

            return Ok(new OcpiResponse<Location?> { StatusCode = 1000, Data = item });
        }
    }
}

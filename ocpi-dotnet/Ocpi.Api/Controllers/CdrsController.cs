using Microsoft.AspNetCore.Mvc;
using Ocpi.Api.Models;
using Ocpi.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Ocpi.Api.Controllers
{
    [ApiController]
    [Route("ocpi/{version}/[controller]")]
    public class CdrsController : ControllerBase
    {
        private static readonly List<Cdr> _store;

        static CdrsController()
        {
            // Generate synthetic ACME fleet charging data using SimSharp simulation
            var simulation = new FleetChargingSimulation();
            var (_, cdrs) = simulation.Generate();
            _store = cdrs;
        }

        [HttpGet]
        public ActionResult<OcpiResponse<IEnumerable<Cdr>>> Get(
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
            var filteredData = _store.AsEnumerable();
            if (queryParams.DateFrom.HasValue)
                filteredData = filteredData.Where(c => c.LastUpdated >= queryParams.DateFrom.Value);
            if (queryParams.DateTo.HasValue)
                filteredData = filteredData.Where(c => c.LastUpdated < queryParams.DateTo.Value);

            // Apply pagination
            var totalCount = filteredData.Count();
            var paginatedData = filteredData
                .OrderBy(c => c.LastUpdated)
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

            var res = new OcpiResponse<IEnumerable<Cdr>>
            {
                StatusCode = 1000,
                Data = paginatedData,
                Timestamp = DateTime.UtcNow.ToString("O")
            };
            return Ok(res);
        }

        [HttpPost]
        public ActionResult<OcpiResponse<Cdr>> Post(string version, [FromBody] Cdr cdr)
        {
            if (cdr == null)
            {
                return BadRequest(new OcpiResponse<Cdr> { StatusCode = 2002, StatusMessage = "Invalid payload" });
            }

            cdr.Id = string.IsNullOrEmpty(cdr.Id) ? Guid.NewGuid().ToString() : cdr.Id;
            _store.Add(cdr);

            var created = new OcpiResponse<Cdr>
            {
                StatusCode = 1000,
                StatusMessage = "CDR stored",
                Data = cdr
            };

            return CreatedAtAction(nameof(GetById), new { version, id = cdr.Id }, created);
        }

        [HttpGet("{id}")]
        public ActionResult<OcpiResponse<Cdr?>> GetById(string version, string id)
        {
            var cdr = _store.Find(x => x.Id == id);
            if (cdr == null)
                return NotFound(new OcpiResponse<Cdr?> { StatusCode = 2001, StatusMessage = "Not found", Data = null });

            return Ok(new OcpiResponse<Cdr?> { StatusCode = 1000, Data = cdr });
        }
    }
}

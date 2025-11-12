using Microsoft.AspNetCore.Mvc;
using Ocpi.Api.Models;
using System.Collections.Generic;

namespace Ocpi.Api.Controllers
{
    [ApiController]
    [Route("ocpi/[controller]")]
    public class VersionsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<OcpiResponse<IEnumerable<VersionInfo>>> Get()
        {
            var versions = new[]
            {
                new VersionInfo { Version = "2.2", Url = "https://localhost/ocpi/2.2/" }
            };

            var res = new OcpiResponse<IEnumerable<VersionInfo>>
            {
                StatusCode = 1000,
                StatusMessage = "Success",
                Data = versions
            };

            return Ok(res);
        }
    }
}

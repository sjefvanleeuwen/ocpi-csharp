using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using Ocpi.Api.Models;
using System.Threading.Tasks;

namespace Ocpi.Api.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_Versions_Returns_Success()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/ocpi/versions");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<OcpiResponse<VersionInfo[]>>();
            Assert.NotNull(data?.Data);
            Assert.True(data!.StatusCode == 1000);
        }

        [Fact]
        public async Task Get_Locations_Returns_List()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/ocpi/2.2/locations");
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadFromJsonAsync<OcpiResponse<Location[]>>();
            Assert.NotNull(data?.Data);
            Assert.True(data!.Data!.Length >= 1);
        }

        [Fact]
        public async Task Post_Cdr_Returns_Created()
        {
            var client = _factory.CreateClient();
            var cdr = new Cdr
            {
                StartDateTime = System.DateTime.UtcNow.AddMinutes(-30),
                EndDateTime = System.DateTime.UtcNow,
                TotalEnergy = 15.2m,
                TotalCost = 3.5m
            };

            var response = await client.PostAsJsonAsync("/ocpi/2.2/cdrs", cdr);
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            var data = await response.Content.ReadFromJsonAsync<OcpiResponse<Cdr>>();
            Assert.Equal(1000, data!.StatusCode);
            Assert.NotNull(data.Data?.Id);
        }
    }
}

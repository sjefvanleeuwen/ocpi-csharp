using System.Text.Json.Serialization;
using System;

namespace Ocpi.Api.Models
{
    public class Cdr
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("start_date_time")]
        public DateTime StartDateTime { get; set; }

        [JsonPropertyName("end_date_time")]
        public DateTime EndDateTime { get; set; }

        [JsonPropertyName("total_energy")]
        public decimal TotalEnergy { get; set; }

        [JsonPropertyName("total_cost")]
        public decimal TotalCost { get; set; }

        [JsonPropertyName("last_updated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}

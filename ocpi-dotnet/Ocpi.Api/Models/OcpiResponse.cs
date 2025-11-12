using System;
using System.Text.Json.Serialization;

namespace Ocpi.Api.Models
{
    public class OcpiResponse<T>
    {
        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; } = "Success";

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}

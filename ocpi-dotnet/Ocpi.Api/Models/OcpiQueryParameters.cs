using System;

namespace Ocpi.Api.Models
{
    /// <summary>
    /// OCPI pagination and filtering parameters
    /// </summary>
    public class OcpiQueryParameters
    {
        /// <summary>
        /// Only return objects that have last_updated after or equal to this value (RFC3339 format)
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Only return objects that have last_updated up to this value (RFC3339 format)
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// The offset of the first object returned. Default is 0.
        /// </summary>
        public int Offset { get; set; } = 0;

        /// <summary>
        /// Maximum number of objects to GET. Default is 25, maximum is 100.
        /// </summary>
        public int Limit { get; set; } = 25;

        public void Validate()
        {
            if (Limit <= 0) Limit = 25;
            if (Limit > 100) Limit = 100;
            if (Offset < 0) Offset = 0;
        }
    }
}

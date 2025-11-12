# OCPI REST Standards Implementation

This document describes the OCPI 2.2 REST standards implemented in this API.

## Pagination

All list endpoints (`GET /locations` and `GET /cdrs`) support pagination through query parameters:

### Query Parameters

- **`offset`** (integer, default: 0): The offset of the first object returned
- **`limit`** (integer, default: 25, max: 100): Maximum number of objects to return

### Example Requests

```http
# Get first page (default 25 items)
GET /ocpi/2.2/locations

# Get first 10 items
GET /ocpi/2.2/locations?limit=10

# Get items 11-20
GET /ocpi/2.2/locations?offset=10&limit=10

# Get page 3 (items 51-75)
GET /ocpi/2.2/locations?offset=50&limit=25
```

### Response Headers

The API returns the following pagination headers:

- **`Link`**: Contains the URL for the next page (if available)
  - Format: `<https://api.example.com/ocpi/2.2/locations?offset=25&limit=25>; rel="next"`
- **`X-Total-Count`**: Total number of items matching the query
- **`X-Limit`**: The limit used for this request

### Example Response

```http
HTTP/1.1 200 OK
Link: <http://localhost:5055/ocpi/2.2/cdrs?offset=10&limit=10>; rel="next"
X-Total-Count: 125
X-Limit: 10
Content-Type: application/json

{
  "status_code": 1000,
  "timestamp": "2025-11-12T10:30:00Z",
  "data": [...]
}
```

## Date Filtering

All list endpoints support filtering by the `last_updated` field:

### Query Parameters

- **`date_from`** (datetime, RFC3339): Only return objects updated after or equal to this value
- **`date_to`** (datetime, RFC3339): Only return objects updated before this value

### Example Requests

```http
# Get locations updated after November 5, 2025
GET /ocpi/2.2/locations?date_from=2025-11-05T00:00:00Z

# Get CDRs within a specific date range
GET /ocpi/2.2/cdrs?date_from=2025-11-05T00:00:00Z&date_to=2025-11-08T00:00:00Z

# Combine date filtering with pagination
GET /ocpi/2.2/cdrs?date_from=2025-11-05T00:00:00Z&offset=0&limit=25
```

### Date Format

All dates must be in **RFC3339 format** (ISO 8601):
- Format: `YYYY-MM-DDTHH:MM:SSZ`
- Example: `2025-11-12T14:30:00Z`
- Timezone: Always UTC (indicated by 'Z')

## Combined Example

```http
# Get the second page of CDRs from the last week, with 20 items per page
GET /ocpi/2.2/cdrs?date_from=2025-11-05T00:00:00Z&offset=20&limit=20

Response Headers:
Link: <http://localhost:5055/ocpi/2.2/cdrs?date_from=2025-11-05T00:00:00Z&offset=40&limit=20>; rel="next"
X-Total-Count: 87
X-Limit: 20
```

## Response Format

All responses follow the OCPI standard format:

```json
{
  "status_code": 1000,
  "status_message": "Success",
  "timestamp": "2025-11-12T14:30:00Z",
  "data": [...]
}
```

### Status Codes

- **1000**: Success
- **2001**: Not Found
- **2002**: Invalid Payload

## Data Model Fields

### Location

Every location includes:
```json
{
  "id": "ACME-LOC-001",
  "name": "ACME Fleet Charging - Amsterdam",
  "address": "123 Main Street",
  "city": "Amsterdam",
  "country": "NL",
  "latitude": 52.370216,
  "longitude": 4.895168,
  "last_updated": "2025-11-05T08:30:00Z"
}
```

### CDR (Charge Detail Record)

Every CDR includes:
```json
{
  "id": "CDR-a1b2c3d4",
  "start_date_time": "2025-11-12T10:00:00Z",
  "end_date_time": "2025-11-12T11:30:00Z",
  "total_energy": 45.5,
  "total_cost": 15.93,
  "last_updated": "2025-11-12T11:32:00Z"
}
```

## Implementation Notes

### Default Behavior

- If no `limit` is specified, the API defaults to 25 items
- Maximum `limit` is 100 items
- If `offset` is negative, it's treated as 0
- If no date filters are specified, all records are returned

### Performance

- The API maintains items sorted by `last_updated` for efficient date-range queries
- Pagination is implemented using `Skip()` and `Take()` on filtered collections
- Total count is calculated after filtering but before pagination

### Standards Compliance

This implementation follows:
- **OCPI 2.2 specification** for REST endpoints
- **RFC3339** for datetime formatting
- **HTTP Link header specification** (RFC 5988) for pagination
- **Standard HTTP headers** for metadata (X-Total-Count, X-Limit)

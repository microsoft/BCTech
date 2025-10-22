# Directions Connector API Reference

Complete API documentation for the workshop server hosted on Azure.

---

## Base URL

```
https://[workshop-server].azurewebsites.net/
```

**Note**: The actual URL will be provided by the instructor at the start of the workshop.

---

## Authentication

All endpoints (except `/register`) require authentication via the `X-Service-Key` header.

```http
X-Service-Key: your-api-key-here
```

The API key is obtained by calling the `/register` endpoint.

---

## Endpoints

### 1. Register User

Register a new user and receive an API key.

**Endpoint**: `POST /register`

**Authentication**: None

**Request**:
```http
POST /register HTTP/1.1
Content-Type: application/json

{
  "name": "participant-name"
}
```

**Response** (200 OK):
```json
{
  "status": "ok",
  "key": "12345678-1234-1234-1234-123456789abc"
}
```

**Example (cURL)**:
```bash
curl -X POST https://workshop-server.azurewebsites.net/register \
  -H "Content-Type: application/json" \
  -d '{"name": "john-doe"}'
```

**Example (PowerShell)**:
```powershell
$body = @{ name = "john-doe" } | ConvertTo-Json
Invoke-RestMethod -Uri "https://workshop-server.azurewebsites.net/register" `
  -Method Post `
  -Body $body `
  -ContentType "application/json"
```

**Notes**:
- Each name should be unique to avoid conflicts
- The API key is returned only once - save it securely
- If you lose your key, you need to register again with a different name

---

### 2. Send Document (Enqueue)

Add a document to your queue.

**Endpoint**: `POST /enqueue`

**Authentication**: Required (`X-Service-Key` header)

**Request**:
```http
POST /enqueue HTTP/1.1
X-Service-Key: your-api-key
Content-Type: application/json

{
  "documentType": "Invoice",
  "documentNo": "SI-001",
  "customerNo": "C001",
  "customerName": "Contoso Ltd.",
  "postingDate": "2025-10-21",
  "currencyCode": "USD",
  "totalAmount": 1250.00,
  "lines": [
    {
      "lineNo": 1,
      "type": "Item",
      "no": "ITEM-001",
      "description": "Item A",
      "quantity": 5,
      "unitPrice": 250.00,
      "lineAmount": 1250.00
    }
  ]
}
```

**Response** (200 OK):
```json
{
  "status": "ok",
  "queued_count": 3
}
```

**Example (cURL)**:
```bash
curl -X POST https://workshop-server.azurewebsites.net/enqueue \
  -H "X-Service-Key: your-api-key" \
  -H "Content-Type: application/json" \
  -d '{
    "documentType": "Invoice",
    "documentNo": "SI-001",
    "customerNo": "C001",
    "customerName": "Contoso Ltd.",
    "postingDate": "2025-10-21",
    "currencyCode": "USD",
    "totalAmount": 1250.00,
    "lines": [
      {
        "lineNo": 1,
        "type": "Item",
        "no": "ITEM-001",
        "description": "Item A",
        "quantity": 5,
        "unitPrice": 250.00,
        "lineAmount": 1250.00
      }
    ]
  }'
```

**Example (PowerShell)**:
```powershell
$headers = @{
    "X-Service-Key" = "your-api-key"
    "Content-Type" = "application/json"
}

$body = @{
    documentType = "Invoice"
    documentNo = "SI-001"
    customerNo = "C001"
    customerName = "Contoso Ltd."
    postingDate = "2025-10-21"
    currencyCode = "USD"
    totalAmount = 1250.00
    lines = @(
        @{
            lineNo = 1
            type = "Item"
            no = "ITEM-001"
            description = "Item A"
            quantity = 5
            unitPrice = 250.00
            lineAmount = 1250.00
        }
    )
} | ConvertTo-Json -Depth 10

Invoke-RestMethod -Uri "https://workshop-server.azurewebsites.net/enqueue" `
  -Method Post `
  -Headers $headers `
  -Body $body
```

**Notes**:
- The document is added to your personal queue (isolated by API key)
- You can enqueue any valid JSON document structure
- The `queued_count` returns the total number of documents in your queue

---

### 3. Check Queue (Peek)

View all documents in your queue without removing them.

**Endpoint**: `GET /peek`

**Authentication**: Required (`X-Service-Key` header)

**Request**:
```http
GET /peek HTTP/1.1
X-Service-Key: your-api-key
```

**Response** (200 OK):
```json
{
  "queued_count": 2,
  "items": [
    {
      "documentType": "Invoice",
      "documentNo": "SI-001",
      "customerNo": "C001",
      "customerName": "Contoso Ltd.",
      "postingDate": "2025-10-21",
      "currencyCode": "USD",
      "totalAmount": 1250.00,
      "lines": [...]
    },
    {
      "documentType": "Invoice",
      "documentNo": "SI-002",
      "customerNo": "C002",
      "customerName": "Fabrikam Inc.",
      "postingDate": "2025-10-21",
      "currencyCode": "EUR",
      "totalAmount": 850.00,
      "lines": [...]
    }
  ]
}
```

**Example (cURL)**:
```bash
curl -X GET https://workshop-server.azurewebsites.net/peek \
  -H "X-Service-Key: your-api-key"
```

**Example (PowerShell)**:
```powershell
$headers = @{ "X-Service-Key" = "your-api-key" }
Invoke-RestMethod -Uri "https://workshop-server.azurewebsites.net/peek" `
  -Method Get `
  -Headers $headers
```

**Example (Browser)**:
You can also use browser extensions like "Modify Header Value" to add the header and view in browser:
```
https://workshop-server.azurewebsites.net/peek
Header: X-Service-Key: your-api-key
```

**Notes**:
- Documents remain in the queue after peeking
- Useful for debugging and verifying document submission
- Returns all documents in your queue (FIFO order)

---

### 4. Retrieve Document (Dequeue)

Retrieve and remove the first document from your queue.

**Endpoint**: `GET /dequeue`

**Authentication**: Required (`X-Service-Key` header)

**Request**:
```http
GET /dequeue HTTP/1.1
X-Service-Key: your-api-key
```

**Response** (200 OK):
```json
{
  "document": {
    "documentType": "Invoice",
    "documentNo": "SI-001",
    "customerNo": "C001",
    "customerName": "Contoso Ltd.",
    "postingDate": "2025-10-21",
    "currencyCode": "USD",
    "totalAmount": 1250.00,
    "lines": [
      {
        "lineNo": 1,
        "type": "Item",
        "no": "ITEM-001",
        "description": "Item A",
        "quantity": 5,
        "unitPrice": 250.00,
        "lineAmount": 1250.00
      }
    ]
  }
}
```

**Response** (404 Not Found) - Queue Empty:
```json
{
  "detail": "Queue empty"
}
```

**Example (cURL)**:
```bash
curl -X GET https://workshop-server.azurewebsites.net/dequeue \
  -H "X-Service-Key: your-api-key"
```

**Example (PowerShell)**:
```powershell
$headers = @{ "X-Service-Key" = "your-api-key" }
Invoke-RestMethod -Uri "https://workshop-server.azurewebsites.net/dequeue" `
  -Method Get `
  -Headers $headers
```

**Notes**:
- Documents are removed from the queue after dequeue (FIFO)
- Returns 404 if the queue is empty
- Once dequeued, the document cannot be retrieved again

---

### 5. Clear Queue

Clear all documents from your queue.

**Endpoint**: `DELETE /clear`

**Authentication**: Required (`X-Service-Key` header)

**Request**:
```http
DELETE /clear HTTP/1.1
X-Service-Key: your-api-key
```

**Response** (200 OK):
```json
{
  "status": "cleared"
}
```

**Example (cURL)**:
```bash
curl -X DELETE https://workshop-server.azurewebsites.net/clear \
  -H "X-Service-Key: your-api-key"
```

**Example (PowerShell)**:
```powershell
$headers = @{ "X-Service-Key" = "your-api-key" }
Invoke-RestMethod -Uri "https://workshop-server.azurewebsites.net/clear" `
  -Method Delete `
  -Headers $headers
```

**Notes**:
- Removes all documents from your queue
- Useful for testing and cleanup
- Cannot be undone

---

## Error Responses

All endpoints may return error responses in the following format:

### 400 Bad Request
```json
{
  "detail": "Missing name"
}
```
Returned when required parameters are missing or invalid.

### 401 Unauthorized
```json
{
  "detail": "Unauthorized or invalid key"
}
```
Returned when:
- The `X-Service-Key` header is missing
- The API key is invalid
- The API key doesn't match any registered user

### 404 Not Found
```json
{
  "detail": "Queue empty"
}
```
Returned when trying to dequeue from an empty queue.

---

## Rate Limits

Currently, there are **no rate limits** enforced. However, please be considerate of other workshop participants and:
- Don't spam the API with excessive requests
- Use reasonable document sizes (< 1 MB)
- Clean up your queue after testing

---

## Data Persistence

**Important**: 
- All data is stored **in-memory only**
- If the server restarts, all queues and registrations are lost
- Don't rely on this API for production use
- This is a workshop server only

---

## JSON Document Structure

While you can send any valid JSON, the recommended structure for this workshop is:

```json
{
  "documentType": "Invoice",           // Type of document
  "documentNo": "string",              // Document number
  "customerNo": "string",              // Customer/Vendor number
  "customerName": "string",            // Customer/Vendor name
  "postingDate": "YYYY-MM-DD",        // ISO date format
  "currencyCode": "string",            // Currency (USD, EUR, etc.)
  "totalAmount": 0.00,                // Decimal number
  "lines": [                           // Array of line items
    {
      "lineNo": 0,                    // Integer line number
      "type": "string",               // Item, G/L Account, etc.
      "no": "string",                 // Item/Account number
      "description": "string",        // Line description
      "quantity": 0.00,               // Decimal quantity
      "unitPrice": 0.00,              // Decimal unit price
      "lineAmount": 0.00              // Decimal line amount
    }
  ]
}
```

---

## Testing Tips

### Using Browser Developer Tools

1. Open browser developer tools (F12)
2. Go to Network tab
3. Call the API endpoints
4. Inspect request/response headers and bodies

### Using Postman

1. Import the endpoints into Postman
2. Set up environment variables for base URL and API key
3. Create a collection for easy testing

### Using VS Code REST Client Extension

Create a `.http` file:

```http
### Variables
@baseUrl = https://workshop-server.azurewebsites.net
@apiKey = your-api-key-here

### Register
POST {{baseUrl}}/register
Content-Type: application/json

{
  "name": "test-user"
}

### Enqueue Document
POST {{baseUrl}}/enqueue
X-Service-Key: {{apiKey}}
Content-Type: application/json

{
  "documentType": "Invoice",
  "documentNo": "SI-001",
  "customerNo": "C001"
}

### Peek Queue
GET {{baseUrl}}/peek
X-Service-Key: {{apiKey}}

### Dequeue Document
GET {{baseUrl}}/dequeue
X-Service-Key: {{apiKey}}

### Clear Queue
DELETE {{baseUrl}}/clear
X-Service-Key: {{apiKey}}
```

---

## Workshop Scenarios

### Scenario 1: Solo Testing
1. Register with your name
2. Send documents from BC
3. Peek to verify they arrived
4. Dequeue them back into BC

### Scenario 2: Partner Exchange
1. Each partner registers separately
2. Partner A sends documents
3. Partner B receives and processes them
4. Swap roles

### Scenario 3: Batch Processing
1. Enqueue multiple documents
2. Peek to see the count
3. Dequeue all at once
4. Process in BC

---

## Support

If you encounter issues with the API:
1. Check your API key is correct
2. Verify the base URL is accessible
3. Check request headers and body format
4. Look at the error response details
5. Ask the instructor for help

---

## Server Implementation

The server is built with FastAPI (Python) and is extremely simple:
- In-memory storage (dictionary and deques)
- No database
- No authentication beyond the API key
- No encryption (workshop use only)

See `server/server.py` for the complete source code.

---

**Happy Testing!** 🚀

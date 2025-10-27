# E-Document Connector Workshop - Implementation Plan

## Workshop Overview
**Duration**: 90 minutes  
**Format**: Hands-on coding workshop  
**Goal**: Build a complete E-Document solution with SimpleJson format and DirectionsConnector integration

---

## Timeline

| Time | Duration | Activity |
|------|----------|----------|
| 00:00-00:10 | 10 min | Introduction (using VS Code as presentation) |
| 00:10-00:40 | 30 min | Exercise 1 - SimpleJson Format Implementation |
| 00:40-01:10 | 30 min | Exercise 2 - DirectionsConnector Integration |
| 01:10-01:25 | 15 min | Testing & Live Demo |
| 01:25-01:30 | 5 min | Wrap-up & Q&A |

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Business Central                          │
│  ┌─────────────────┐         ┌──────────────────────┐      │
│  │  Sales Invoice  │────────▶│  E-Document Core     │      │
│  └─────────────────┘         └──────────────────────┘      │
│                                        │                     │
│                                        ▼                     │
│                              ┌──────────────────────┐       │
│                              │  SimpleJson Format   │       │
│                              │  (Exercise 1)        │       │
│                              └──────────────────────┘       │
│                                        │                     │
│                                        ▼                     │
│                                   JSON Blob                  │
│                                        │                     │
│                                        ▼                     │
│                              ┌──────────────────────┐       │
│                              │ DirectionsConnector  │       │
│                              │  (Exercise 2)        │       │
│                              └──────────────────────┘       │
└──────────────────────────────────────┼───────────────────────┘
                                       │ HTTP POST
                                       ▼
                        ┌─────────────────────────────┐
                        │   Azure API Server          │
                        │   (Pre-deployed)            │
                        │                             │
                        │  Endpoints:                 │
                        │  - POST /register           │
                        │  - POST /enqueue            │
                        │  - GET  /peek               │
                        │  - GET  /dequeue            │
                        └─────────────────────────────┘
```

---

## What Participants Will Build

### Exercise 1: SimpleJson Format (30 minutes)
Implement the **"E-Document" Interface** to convert Business Central documents to/from JSON format.

The interface has two main sections:
1. **Outgoing**: Convert BC documents to E-Document blobs (`Check`, `Create`, `CreateBatch`)
2. **Incoming**: Parse E-Document blobs to BC documents (`GetBasicInfoFromReceivedDocument`, `GetCompleteInfoFromReceivedDocument`)

**Participants will implement:**
- ✅ `Check()` method - Validate required fields before document creation (5 min)
- ✅ `Create()` method - Generate JSON from Sales Invoice (15 min)
- ✅ `GetBasicInfoFromReceivedDocument()` method - Parse incoming JSON metadata (5 min)
- ✅ `GetCompleteInfoFromReceivedDocument()` method - Create Purchase Invoice from JSON (5 min)

**Pre-written boilerplate includes:**
- Extension setup (app.json, dependencies)
- Enum extensions
- Helper methods for JSON generation and parsing
- Error handling framework

### Exercise 2: DirectionsConnector Integration (30 minutes)
Implement the **Integration Interface** to send and receive documents via the Azure API server.

**Participants will implement:**
- ✅ Connection setup and registration (5 min)
- ✅ `Send()` method - POST document to /enqueue endpoint (10 min)
- ✅ `ReceiveDocuments()` method - GET documents from /peek endpoint (10 min)
- ✅ `DownloadDocument()` method - GET single document from /dequeue endpoint (5 min)

**Pre-written boilerplate includes:**
- Setup table and page UI
- Authentication helper
- HTTP request builders
- Error logging

---

## Sample JSON Format

Participants will generate JSON in this structure:

```json
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

---

## Azure API Server

**Base URL**: `https://[workshop-server].azurewebsites.net` (will be provided)

### Endpoints

#### 1. Register User
```http
POST /register
Content-Type: application/json

{
  "name": "participant-name"
}

Response:
{
  "status": "ok",
  "key": "uuid-api-key"
}
```

#### 2. Send Document (Enqueue)
```http
POST /enqueue
X-Service-Key: your-api-key
Content-Type: application/json

{
  "documentType": "Invoice",
  "documentNo": "SI-001",
  ...
}

Response:
{
  "status": "ok",
  "queued_count": 1
}
```

#### 3. Check Queue
```http
GET /peek
X-Service-Key: your-api-key

Response:
{
  "queued_count": 1,
  "items": [...]
}
```

#### 4. Retrieve Document (Dequeue)
```http
GET /dequeue
X-Service-Key: your-api-key

Response:
{
  "document": {...}
}
```

---

## Project Structure

```
DirectionsEMEA2025/
├── WORKSHOP_INTRO.md          # VS Code presentation deck
├── WORKSHOP_GUIDE.md          # Step-by-step exercises
├── API_REFERENCE.md           # Azure API documentation
│
├── application/
│   ├── simple_json/           # Exercise 1: Format Extension
│   │   ├── app.json
│   │   ├── SimpleJsonFormat.EnumExt.al
│   │   ├── SimpleJsonFormat.Codeunit.al       # ⚠️ TODO sections
│   │   └── SimpleJsonHelper.Codeunit.al       # ✅ Pre-written
│   │
│   └── directions_connector/  # Exercise 2: Integration Extension
│       ├── app.json
│       ├── DirectionsIntegration.EnumExt.al
│       ├── DirectionsIntegration.Codeunit.al  # ⚠️ TODO sections
│       ├── DirectionsSetup.Table.al           # ✅ Pre-written
│       ├── DirectionsSetup.Page.al            # ✅ Pre-written
│       ├── DirectionsAuth.Codeunit.al         # ✅ Pre-written
│       └── DirectionsRequests.Codeunit.al     # ✅ Pre-written
│
├── solution/                  # Complete working solution
│   ├── simple_json/           # Reference implementation
│   └── directions_connector/  # Reference implementation
│
└── server/
    ├── server.py              # FastAPI server (for reference)
    ├── requirements.txt
    └── README.md              # Deployment info (Azure hosted)
```

---

## Deliverables

### 1. WORKSHOP_INTRO.md
VS Code presentation covering:
- **Slide 1**: Welcome & Goals
- **Slide 2**: E-Document Framework Overview
- **Slide 3**: Architecture Diagram
- **Slide 4**: SimpleJson Format Introduction
- **Slide 5**: DirectionsConnector Overview
- **Slide 6**: Azure API Server
- **Slide 7**: Exercise Overview
- **Slide 8**: Success Criteria

### 2. WORKSHOP_GUIDE.md
Step-by-step instructions:
- Prerequisites & setup
- **Exercise 1**: SimpleJson Format (implements "E-Document" interface)
  - Part A: Implement Check() (5 min)
  - Part B: Implement Create() (15 min)
  - Part C: Implement GetBasicInfoFromReceivedDocument() (5 min)
  - Part D: Implement GetCompleteInfoFromReceivedDocument() (5 min)
  - Validation steps
- **Exercise 2**: DirectionsConnector
  - Part A: Register & Setup (5 min)
  - Part B: Implement Send() (10 min)
  - Part C: Implement ReceiveDocuments() (10 min)
  - Part D: Implement DownloadDocument() (5 min)
  - Validation steps
- Testing instructions
- Troubleshooting guide

### 3. API_REFERENCE.md
Complete API documentation:
- Base URL and authentication
- All endpoint specifications
- Request/response examples
- Error handling
- Rate limits (if any)

### 4. SimpleJson Format Boilerplate
Files with TODO sections:
- `app.json` - Extension manifest with dependencies
- `SimpleJsonFormat.EnumExt.al` - Enum extension (pre-written)
- `SimpleJsonFormat.Codeunit.al` - Format implementation with TODOs:
  ```al
  // TODO: Exercise 1.A - Implement validation
  procedure Check(...)
  begin
      // TODO: Validate Customer No.
      // TODO: Validate Posting Date
      // TODO: Validate at least one line exists
  end;

  // TODO: Exercise 1.B - Generate JSON
  procedure Create(...)
  begin
      // TODO: Create JSON header
      // TODO: Add lines array
      // TODO: Calculate totals
  end;

  // TODO: Exercise 1.C - Parse incoming JSON (Basic Info)
  procedure GetBasicInfoFromReceivedDocument(...)
  begin
      // TODO: Parse JSON from TempBlob
      // TODO: Set EDocument."Document Type"
      // TODO: Set EDocument."Bill-to/Pay-to No."
      // TODO: Set EDocument."Bill-to/Pay-to Name"
      // TODO: Set EDocument."Document Date"
      // TODO: Set EDocument."Currency Code"
  end;

  // TODO: Exercise 1.D - Create Purchase Invoice (Complete Info)
  procedure GetCompleteInfoFromReceivedDocument(...)
  begin
      // TODO: Read JSON from TempBlob
      // TODO: Create Purchase Header record
      // TODO: Set header fields from JSON
      // TODO: Create Purchase Lines from JSON array
      // TODO: Return via RecordRef parameters
  end;
  ```
- `SimpleJsonHelper.Codeunit.al` - Helper methods (pre-written):
  - `AddJsonProperty()` - Add property to JSON
  - `StartJsonObject()` - Start JSON object
  - `StartJsonArray()` - Start JSON array
  - `ParseJsonValue()` - Get value from JSON
  - `GetJsonToken()` - Get JSON token by path
  - etc.

### 5. DirectionsConnector Boilerplate
Files with TODO sections:
- `app.json` - Extension manifest
- `DirectionsIntegration.EnumExt.al` - Enum extension (pre-written)
- `DirectionsIntegration.Codeunit.al` - Integration implementation with TODOs:
  ```al
  // TODO: Exercise 2.A - Setup connection
  local procedure RegisterUser(...) // Provided with TODOs

  // TODO: Exercise 2.B - Send document
  procedure Send(...)
  begin
      // TODO: Get connection setup
      // TODO: Prepare HTTP request
      // TODO: Set authorization header
      // TODO: Send POST request
      // TODO: Handle response
  end;

  // TODO: Exercise 2.C - Receive documents list
  procedure ReceiveDocuments(...)
  begin
      // TODO: Get connection setup
      // TODO: Call /peek endpoint
      // TODO: Parse response and create metadata blobs
  end;

  // TODO: Exercise 2.D - Download single document
  procedure DownloadDocument(...)
  begin
      // TODO: Read document ID from metadata
      // TODO: Call /dequeue endpoint
      // TODO: Store document content in TempBlob
  end;
  ```
- Pre-written helper files:
  - `DirectionsSetup.Table.al` - Connection settings (URL, API Key)
  - `DirectionsSetup.Page.al` - Setup UI
  - `DirectionsAuth.Codeunit.al` - Authentication helper
  - `DirectionsRequests.Codeunit.al` - HTTP request builders

### 6. Solution Folder
Complete working implementations for instructor reference:
- `/solution/simple_json/` - Fully implemented format
- `/solution/directions_connector/` - Fully implemented connector
- These are NOT given to participants initially

### 7. Server Documentation
- `server/README.md` - Overview of the API server
  - Architecture explanation
  - Endpoint documentation
  - Deployment notes (Azure hosted)
  - No setup required (server is pre-deployed)

---

## TODO Marking Convention

In code files, use clear TODO markers with timing:

```al
// ============================================================================
// TODO: Exercise 1.A (10 minutes)
// Validate that required fields are filled before creating the document
// 
// Instructions:
// 1. Validate that Customer No. is not empty
// 2. Validate that Posting Date is set
// 3. Validate that at least one line exists
//
// Hints:
// - Use SourceDocumentHeader.Field(FieldNo).TestField()
// - Use EDocumentErrorHelper.LogSimpleErrorMessage() for custom errors
// - Check the README.md for the Check() method example
// ============================================================================
procedure Check(var SourceDocumentHeader: RecordRef; EDocumentService: Record "E-Document Service"; EDocumentProcessingPhase: Enum "E-Document Processing Phase")
begin
    // TODO: Your code here
end;
```

---

## Success Criteria

By the end of the workshop, participants should be able to:
- ✅ Create a sales invoice in BC
- ✅ See it converted to JSON via SimpleJson format
- ✅ Send it to Azure API server via DirectionsConnector
- ✅ Verify it appears in the queue (via /peek endpoint)
- ✅ Receive documents from the Azure API server
- ✅ Parse incoming JSON and extract metadata
- ✅ Create purchase invoices from received E-Documents
- ✅ Understand the complete E-Document round-trip flow (outgoing and incoming)
- ✅ Understand the E-Document framework architecture
- ✅ Know how to extend with additional features

---

## Bonus/Homework Ideas

For advanced participants or post-workshop:

**Format Interface:**
- Implement `CreateBatch()` for batch processing multiple documents
- Add support for Sales Credit Memos and Purchase Credit Memos
- Add more sophisticated field mappings (dimensions, custom fields)
- Implement validation rules for incoming documents
- Support for attachments (PDF, XML)

**Integration Interface:**
- Implement `IDocumentResponseHandler` with `GetResponse()` for async status checking
- Implement `ISentDocumentActions` for approval/cancellation workflows
- Implement `IDocumentAction` for custom actions
- Add comprehensive error handling and retry logic
- Add batch sending support

---

## Notes for Implementation

### Key Simplifications for 90-Minute Workshop
1. **Format**: Full round-trip (Create and PrepareDocument), but simplified field mapping
2. **Connector**: Full round-trip (Send and Receive), but simplified response handling
3. **Validation**: Basic field checks only
4. **Error Handling**: Use pre-written helpers
5. **UI**: Minimal - focus on code logic
6. **Document Types**: Only Sales Invoice outgoing, only Purchase Invoice incoming

### Pre-Written vs TODO
**Participants write** (~60% of time):
- Business validation logic
- JSON structure generation
- HTTP request preparation
- Response handling

**Pre-written boilerplate** (~40% setup time saved):
- Extension setup and dependencies
- Enum extensions
- Setup tables/pages
- Helper methods (JSON, HTTP, Auth)
- Error logging framework

### Testing Strategy

**Outgoing Flow (Send):**
1. Create test sales invoice with known data
2. Post and verify E-Document created
3. Check JSON blob content in E-Document log
4. Verify document sent to Azure API
5. Use /peek endpoint to confirm receipt in queue

**Incoming Flow (Receive):**
6. Use another participant's queue or test data in API
7. Run "Get Documents" action in BC
8. Verify E-Documents appear in E-Document list with status "Imported"
9. Check downloaded JSON content in E-Document log
10. Verify basic info parsed correctly (vendor, amount, date)
11. Create Purchase Invoice from E-Document
12. Verify purchase invoice created with correct data
13. Confirm document removed from queue (via /peek)

---

## Files to Create

### Immediate (for workshop)
- [ ] `WORKSHOP_INTRO.md` - VS Code presentation
- [ ] `WORKSHOP_GUIDE.md` - Exercise instructions
- [ ] `API_REFERENCE.md` - Azure API docs
- [ ] `application/simple_json/` - Format boilerplate
- [ ] `application/directions_connector/` - Connector boilerplate

### Reference (for instructor)
- [ ] `solution/simple_json/` - Complete format
- [ ] `solution/directions_connector/` - Complete connector
- [ ] `server/README.md` - Server documentation

### Nice-to-have
- [ ] `TROUBLESHOOTING.md` - Common issues
- [ ] `HOMEWORK.md` - Post-workshop exercises
- [ ] Sample test data scripts

---

## Next Steps

1. Review and approve this plan
2. Get Azure server URL and deployment details
3. Create the workshop introduction (WORKSHOP_INTRO.md)
4. Create the boilerplate code with TODOs
5. Create the complete solution for reference
6. Test the full workshop flow
7. Prepare any PowerPoint backup slides (if needed)

---

**Ready to implement?** Let me know if you want to adjust anything before we start building!

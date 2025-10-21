# 🎯 E-Document Connector Workshop
## Directions EMEA 2025

---

## 👋 Welcome!

In the next **90 minutes**, you'll build a complete E-Document integration solution.

**What you'll learn:**
- How the E-Document Core framework works
- How to implement format interfaces (JSON)
- How to implement integration interfaces (HTTP API)
- Complete round-trip document exchange

**What you'll build:**
- ✅ SimpleJson Format - Convert documents to/from JSON
- ✅ DirectionsConnector - Send/receive via HTTP API

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                      Business Central                            │
│                                                                   │
│  ┌──────────────┐    ┌────────────────────┐                    │
│  │ Sales Invoice│───▶│  E-Document Core   │                    │
│  └──────────────┘    └────────────────────┘                    │
│                              │                                   │
│                              ▼                                   │
│                     ┌─────────────────┐                         │
│                     │ Format Interface│◀─── You implement!      │
│                     │  (SimpleJson)   │                         │
│                     └─────────────────┘                         │
│                              │                                   │
│                              ▼                                   │
│                         JSON Blob                                │
│                              │                                   │
│                              ▼                                   │
│                  ┌───────────────────────┐                      │
│                  │ Integration Interface │◀─── You implement!   │
│                  │ (DirectionsConnector) │                      │
│                  └───────────────────────┘                      │
└──────────────────────────────┼───────────────────────────────────┘
                               │ HTTPS
                               ▼
                    ┌──────────────────────┐
                    │   Azure API Server   │
                    │                      │
                    │  Queue Management    │
                    │  Document Storage    │
                    └──────────────────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │  Another Company /   │
                    │  Trading Partner     │
                    └──────────────────────┘
```

---

## 📦 E-Document Core Framework

The framework provides:

### 1️⃣ **Document Interface** (`"E-Document"`)
Convert documents to/from Business Central

**Outgoing** (BC → External):
- `Check()` - Validate before sending
- `Create()` - Convert to format (JSON, XML, etc.)
- `CreateBatch()` - Batch multiple documents

**Incoming** (External → BC):
- `GetBasicInfoFromReceivedDocument()` - Parse metadata
- `GetCompleteInfoFromReceivedDocument()` - Create BC document

### 2️⃣ **Integration Interfaces**
Send/receive documents via various channels

**IDocumentSender**:
- `Send()` - Send document to external service

**IDocumentReceiver**:
- `ReceiveDocuments()` - Get list of available documents
- `DownloadDocument()` - Download specific document

**Others** (Advanced):
- `IDocumentResponseHandler` - Async status checking
- `ISentDocumentActions` - Approval/cancellation
- `IDocumentAction` - Custom actions

---

## 🎨 What is SimpleJson?

A simple JSON format for E-Documents designed for this workshop.

### Example JSON Structure:

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

**Why JSON?**
- ✅ Human-readable
- ✅ Easy to parse
- ✅ Widely supported
- ✅ Perfect for learning

---

## 🔌 What is DirectionsConnector?

An HTTP-based integration that sends/receives documents via REST API.

### API Endpoints:

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/register` | POST | Get API key |
| `/enqueue` | POST | Send document |
| `/peek` | GET | View queue |
| `/dequeue` | GET | Receive document |
| `/clear` | DELETE | Clear queue |

### Authentication:
```http
X-Service-Key: your-api-key-here
```

### Why HTTP API?
- ✅ Simple and universal
- ✅ Easy to test (browser, Postman)
- ✅ Real-world scenario
- ✅ Stateless and scalable

---

## 🔄 Complete Flow

### Outgoing (Sending):
```
Sales Invoice (BC)
    ↓ Post
E-Document Created
    ↓ Format: SimpleJson.Create()
JSON Blob
    ↓ Integration: DirectionsConnector.Send()
HTTP POST /enqueue
    ↓
Azure API Queue
```

### Incoming (Receiving):
```
Azure API Queue
    ↓ Integration: DirectionsConnector.ReceiveDocuments()
HTTP GET /peek (list)
    ↓ Integration: DirectionsConnector.DownloadDocument()
HTTP GET /dequeue (download)
    ↓
JSON Blob
    ↓ Format: SimpleJson.GetBasicInfo()
E-Document Created (Imported)
    ↓ Format: SimpleJson.GetCompleteInfo()
Purchase Invoice (BC)
```

---

## ⏱️ Workshop Timeline

| Time | Duration | Activity |
|------|----------|----------|
| 00:00 | 10 min | ← You are here! (Introduction) |
| 00:10 | 30 min | **Exercise 1**: Implement SimpleJson Format |
| 00:40 | 30 min | **Exercise 2**: Implement DirectionsConnector |
| 01:10 | 15 min | Testing & Live Demo |
| 01:25 | 5 min | Wrap-up & Q&A |

---

## 📝 Exercise 1: SimpleJson Format (30 min)

Implement the **"E-Document" interface**

### Part A: Check() - 5 minutes
Validate required fields before creating document
```al
procedure Check(var SourceDocumentHeader: RecordRef; ...)
begin
    // TODO: Validate Customer No.
    // TODO: Validate Posting Date
end;
```

### Part B: Create() - 15 minutes
Convert Sales Invoice to JSON
```al
procedure Create(...; var TempBlob: Codeunit "Temp Blob")
begin
    // TODO: Generate JSON from Sales Invoice
    // TODO: Include header and lines
end;
```

### Part C: GetBasicInfo() - 5 minutes
Parse incoming JSON metadata
```al
procedure GetBasicInfoFromReceivedDocument(var EDocument: Record "E-Document"; ...)
begin
    // TODO: Parse JSON
    // TODO: Set document type, number, date
end;
```

### Part D: GetCompleteInfo() - 5 minutes
Create Purchase Invoice from JSON
```al
procedure GetCompleteInfoFromReceivedDocument(...)
begin
    // TODO: Parse JSON
    // TODO: Create Purchase Header & Lines
end;
```

---

## 🔌 Exercise 2: DirectionsConnector (30 min)

Implement **IDocumentSender** and **IDocumentReceiver** interfaces

### Part A: Setup - 5 minutes
Configure connection in BC
- API Base URL
- Register to get API Key
- Test connection

### Part B: Send() - 10 minutes
Send document to API
```al
procedure Send(var EDocument: Record "E-Document"; ...)
begin
    // TODO: Get JSON from SendContext
    // TODO: POST to /enqueue endpoint
    // TODO: Handle response
end;
```

### Part C: ReceiveDocuments() - 10 minutes
Get list of available documents
```al
procedure ReceiveDocuments(...; DocumentsMetadata: Codeunit "Temp Blob List"; ...)
begin
    // TODO: GET from /peek endpoint
    // TODO: Parse items array
    // TODO: Add each to DocumentsMetadata list
end;
```

### Part D: DownloadDocument() - 5 minutes
Download specific document
```al
procedure DownloadDocument(var EDocument: Record "E-Document"; ...)
begin
    // TODO: GET from /dequeue endpoint
    // TODO: Parse response
    // TODO: Store in TempBlob
end;
```

---

## 🎯 Success Criteria

By the end, you should be able to:

✅ **Create** a Sales Invoice in BC  
✅ **Convert** it to JSON via SimpleJson format  
✅ **Send** it to Azure API via DirectionsConnector  
✅ **Verify** it appears in the queue  
✅ **Receive** documents from the API  
✅ **Parse** JSON and extract metadata  
✅ **Create** Purchase Invoices from received documents  

**You'll have built a complete E-Document integration!** 🎉

---

## 🛠️ What's Pre-Written?

To save time, these are already implemented:

### SimpleJson Format:
- ✅ Extension setup (app.json)
- ✅ Enum extension
- ✅ Helper methods for JSON operations
- ✅ Error handling framework

### DirectionsConnector:
- ✅ Connection Setup table & page
- ✅ Authentication helpers
- ✅ HTTP request builders
- ✅ Registration logic

**You focus on the business logic!**

---

## 📂 Your Workspace

```
application/
  ├── simple_json/
  │   ├── app.json                     ✅ Pre-written
  │   ├── SimpleJsonFormat.EnumExt.al  ✅ Pre-written
  │   ├── SimpleJsonFormat.Codeunit.al ⚠️  TODO sections
  │   └── SimpleJsonHelper.Codeunit.al ✅ Pre-written
  │
  └── directions_connector/
      ├── app.json                           ✅ Pre-written
      ├── DirectionsIntegration.EnumExt.al   ✅ Pre-written
      ├── DirectionsIntegration.Codeunit.al  ⚠️  TODO sections
      ├── DirectionsConnectionSetup.Table.al ✅ Pre-written
      ├── DirectionsConnectionSetup.Page.al  ✅ Pre-written
      ├── DirectionsAuth.Codeunit.al         ✅ Pre-written
      └── DirectionsRequests.Codeunit.al     ✅ Pre-written
```

---

## 📚 Resources Available

During the workshop:

1. **WORKSHOP_GUIDE.md** - Step-by-step instructions with full code
2. **API_REFERENCE.md** - Complete API documentation
3. **README.md** (E-Document Core) - Framework reference
4. **Instructor** - Available for questions!

After the workshop:
- Complete solution in `/solution/` folder
- Homework exercises
- Additional resources

---

## 💡 Tips for Success

1. **Read the TODO comments** - They contain hints and instructions
2. **Use the helper methods** - They're pre-written to save time
3. **Test incrementally** - Don't wait until the end
4. **Check the logs** - E-Document framework logs everything
5. **Ask questions** - The instructor is here to help!
6. **Have fun!** - This is a hands-on learning experience

---

## 🐛 Common Pitfalls

Watch out for:
- ❌ Missing commas in JSON
- ❌ Forgetting to add authentication headers
- ❌ Not handling empty lines/arrays
- ❌ Incorrect RecordRef table numbers
- ❌ Not logging HTTP requests/responses

**The workshop guide has solutions for all of these!**

---

## 🎓 Beyond the Workshop

After mastering the basics, explore:

**Advanced Format Features:**
- Support multiple document types
- Add custom field mappings
- Implement validation rules
- Handle attachments (PDF, XML)

**Advanced Integration Features:**
- Async status checking (`IDocumentResponseHandler`)
- Approval workflows (`ISentDocumentActions`)
- Batch processing
- Error handling and retry logic
- Custom actions (`IDocumentAction`)

**Real-World Scenarios:**
- PEPPOL format
- Avalara integration
- Custom XML formats
- EDI integrations

---

## 🤝 Workshop Collaboration

### Partner Up!
- Work with a neighbor
- Share your API key to exchange documents
- Test each other's implementations

### Group Testing
- Send documents to the group queue
- Everyone receives and processes them
- Great way to test at scale!

---

## 🚀 Let's Get Started!

1. **Open** `WORKSHOP_GUIDE.md` for step-by-step instructions
2. **Navigate** to `application/simple_json/SimpleJsonFormat.Codeunit.al`
3. **Find** the first TODO section in the `Check()` method
4. **Start coding!**

**Timer starts... NOW!** ⏰

---

## ❓ Questions?

Before we start:
- ❓ Is everyone able to access the API URL?
- ❓ Does everyone have their development environment ready?
- ❓ Any questions about the architecture or flow?

---

## 📞 Need Help?

During the workshop:
- 🙋 Raise your hand
- 💬 Ask your neighbor
- 📖 Check the WORKSHOP_GUIDE.md
- 🔍 Look at the API_REFERENCE.md
- 👨‍🏫 Ask the instructor

**We're all here to learn together!**

---

# 🎉 Good Luck!

**Remember**: The goal is to learn, not to finish first.  
Take your time, experiment, and enjoy the process!

**Now let's build something awesome!** 💪

---

## Quick Links

- 📘 [Workshop Guide](./WORKSHOP_GUIDE.md) - Step-by-step instructions
- 🔌 [API Reference](./API_REFERENCE.md) - Endpoint documentation  
- 📝 [Workshop Plan](./WORKSHOP_PLAN.md) - Overview and structure
- 📚 [E-Document Core README](../../../NAV_1/App/BCApps/src/Apps/W1/EDocument/App/README.md) - Framework docs

**API Base URL**: `[Will be provided by instructor]`

---

*Press `Ctrl+Shift+V` in VS Code to view this file in preview mode with formatting!*

# E-Document Connector Workshop
## Directions EMEA 2025

Welcome to the E-Document Connector Workshop! This hands-on workshop teaches you how to build complete E-Document integrations using the Business Central E-Document Core framework.

---

## 🎯 Workshop Overview

**Duration**: 90 minutes (10 min intro + 80 min hands-on)

**What You'll Build**:
1. **SimpleJson Format** - E-Document format implementation for JSON documents
2. **DirectionsConnector** - HTTP API integration for sending/receiving documents
3. **Complete Round-Trip** - Full document exchange between Business Central and external systems

**What You'll Learn**:
- How the E-Document Core framework works
- How to implement format interfaces
- How to implement integration interfaces
- Best practices for E-Document solutions

---

## 📂 Workshop Structure

```
workshop/
├── COMPLETE_WORKSHOP_GUIDE.md  # ⭐ Start here! Complete step-by-step guide
├── API_REFERENCE.md            # API endpoint documentation
├── README.md                   # This file - overview and quick reference
├── WORKSHOP_INTRO.md           # Presentation slides (background material)
├── WORKSHOP_GUIDE.md           # Original exercise instructions
└── WORKSHOP_PLAN.md            # Implementation plan (for reference)

application/
├── simple_json/                # Exercise 1: Format implementation
│   ├── SimpleJsonFormat.Codeunit.al        # ⚠️ Your code here
│   ├── SimpleJsonFormat.EnumExt.al         # ✅ Pre-written
│   ├── SimpleJsonHelper.Codeunit.al        # ✅ Helper methods
│   ├── SimpleJsonTest.Codeunit.al          # ✅ Automated tests
│   └── app.json
│
└── directions_connector/       # Exercise 2: Integration implementation
    ├── ConnectorIntegration.Codeunit.al    # ⚠️ Your code here
    ├── ConnectorIntegration.EnumExt.al     # ✅ Pre-written
    ├── ConnectorAuth.Codeunit.al           # ✅ Helper methods
    ├── ConnectorRequests.Codeunit.al       # ✅ Helper methods
    ├── ConnectorConnectionSetup.Table.al   # ✅ Setup table
    ├── ConnectorConnectionSetup.Page.al    # ✅ Setup UI
    ├── ConnectorTests.Codeunit.al          # ✅ Automated tests
    └── app.json

server/
├── app.py                      # API server implementation (Python/FastAPI)
├── requirements.txt            # Python dependencies
├── startup.sh                  # Deployment script
└── README.md                   # Server documentation
```

---

## 🚀 Getting Started

### For Participants

**Quick Start:**

1. **Open** `COMPLETE_WORKSHOP_GUIDE.md` - This is your main guide!
2. **Ensure** you have:
   - Business Central development environment
   - VS Code with AL Language extension
   - API Base URL from instructor
3. **Follow** the guide step-by-step through both exercises
4. **Reference** `API_REFERENCE.md` when working with HTTP endpoints

**Workshop Timeline:**
- 00:00-00:10: Introduction & Architecture (optional: review `WORKSHOP_INTRO.md`)
- 00:10-00:40: Exercise 1 - SimpleJson Format
- 00:40-01:10: Exercise 2 - DirectionsConnector
- 01:10-01:25: Testing & Live Demo
- 01:25-01:30: Wrap-up & Q&A

---

## ⏱️ Workshop Timeline

| Time | Duration | Activity | File |
|------|----------|----------|------|
| 00:00-00:10 | 10 min | Introduction & Architecture | WORKSHOP_INTRO.md |
| 00:10-00:40 | 30 min | Exercise 1: SimpleJson Format | WORKSHOP_GUIDE.md |
| 00:40-01:10 | 30 min | Exercise 2: DirectionsConnector | WORKSHOP_GUIDE.md |
| 01:10-01:25 | 15 min | Testing & Live Demo | WORKSHOP_GUIDE.md |
| 01:25-01:30 | 5 min | Wrap-up & Q&A | - |

---

## 📋 Prerequisites

### Required
- Business Central development environment (Sandbox or Docker)
- VS Code with AL Language extension
- Basic AL programming knowledge
- API Base URL (provided by instructor)

### Helpful
- Understanding of REST APIs
- JSON format familiarity
- Postman or similar API testing tool

---

## 🎓 Learning Objectives

By the end of this workshop, participants will be able to:

1. ✅ Understand the E-Document Core framework architecture
2. ✅ Implement the "E-Document" interface for custom formats
3. ✅ Implement IDocumentSender and IDocumentReceiver interfaces
4. ✅ Configure and use E-Document Services in Business Central
5. ✅ Test complete document round-trips (send and receive)
6. ✅ Troubleshoot common integration issues
7. ✅ Know where to find resources for further learning

---

## 📚 Key Concepts

### E-Document Format Interface
Converts Business Central documents to/from external formats:
- **Outgoing**: `Check()`, `Create()`
- **Incoming**: `GetBasicInfoFromReceivedDocument()`, `GetCompleteInfoFromReceivedDocument()`

### Integration Interfaces
Communicates with external systems:
- **IDocumentSender**: `Send()` - Send documents
- **IDocumentReceiver**: `ReceiveDocuments()`, `DownloadDocument()` - Receive documents

### SimpleJson Format
- Simple JSON structure for learning
- Header fields: document type, number, customer, date, amount
- Lines array: line items with quantities and prices
- Human-readable and easy to debug

### Connector
- REST API integration via HTTP
- Authentication via API key header
- Queue-based document exchange
- Stateless and scalable

---

## 📖 Additional Resources

### Workshop Materials
- [Workshop Introduction](WORKSHOP_INTRO.md) - Slide deck presentation
- [Workshop Guide](WORKSHOP_GUIDE.md) - Detailed instructions with code
- [API Reference](API_REFERENCE.md) - Complete endpoint documentation
- [Workshop Plan](WORKSHOP_PLAN.md) - Implementation overview

### E-Document Framework
- [E-Document Core README](https://github.com/microsoft/BCApps/blob/main/src/Apps/W1/EDocument/App/README.md) - Official framework documentation
- [E-Document Interface](https://github.com/microsoft/BCApps/blob/main/src/Apps/W1/EDocument/App/src/Document/Interfaces/EDocument.Interface.al) - Interface source code

### External Resources
- [Business Central Documentation](https://learn.microsoft.com/dynamics365/business-central/)
- [AL Language Reference](https://learn.microsoft.com/dynamics365/business-central/dev-itpro/developer/devenv-reference-overview)


## 🎯 Quick Start

### 5-Minute Setup

1. **Get API Access:**
   - Instructor provides API Base URL: `https://[server].azurewebsites.net/`
   - Open Business Central
   - Search "Connector Connection Setup"
   - Enter API Base URL and your unique name
   - Click "Register" to get API key

2. **Create E-Document Service:**
   - Search "E-Document Services"
   - Create new service with:
     - Code: `CONNECTOR`
     - Document Format: `Simple JSON Format - Exercise 1`
     - Service Integration V2: `Connector`
   - Enable the service

3. **Start Coding:**
   - Open `application/simple_json/SimpleJsonFormat.Codeunit.al`
   - Find first TODO comment
   - Follow `COMPLETE_WORKSHOP_GUIDE.md`

### Quick Test

**Outgoing (Send):**
```
1. Post a Sales Invoice
2. Open E-Documents → Find your invoice
3. Click "Send"
4. Check status changes to "Sent"
```

**Incoming (Receive):**
```
1. E-Document Services → Select CONNECTOR
2. Click "Get Documents"
3. E-Documents → Filter Status: "Imported"
4. Click "Create Document" → Opens Purchase Invoice
```

---

## 📖 Documentation Quick Reference

| Document | Purpose | When to Use |
|----------|---------|-------------|
| [**COMPLETE_WORKSHOP_GUIDE.md**](./COMPLETE_WORKSHOP_GUIDE.md) | ⭐ Main guide with all exercises and solutions | Start here - your primary reference |
| [**API_REFERENCE.md**](./API_REFERENCE.md) | Complete API endpoint documentation | When implementing HTTP calls |
| [**WORKSHOP_INTRO.md**](./WORKSHOP_INTRO.md) | Presentation slides (architecture overview) | For understanding concepts |
| [**WORKSHOP_GUIDE.md**](./WORKSHOP_GUIDE.md) | Original exercise instructions | Alternative detailed guide |
| [**WORKSHOP_PLAN.md**](./WORKSHOP_PLAN.md) | Implementation plan and structure | For instructors/reference |

---

## 🏆 Success Criteria

You've completed the workshop successfully if you can:

- ✅ Post a Sales Invoice and see it as an E-Document
- ✅ View the JSON content in the E-Document log
- ✅ Send the E-Document to the API
- ✅ Verify the document appears in the API queue (using `/peek`)
- ✅ Receive documents from the API queue
- ✅ Create Purchase Invoices from received E-Documents
- ✅ Verify all data is mapped correctly (vendor, dates, lines, amounts)
- ✅ Understand the complete round-trip flow

---

## 🔧 Code Locations

### Exercise 1: SimpleJson Format

**File:** `application/simple_json/SimpleJsonFormat.Codeunit.al`

**TODOs:**
- Line ~30: `Check()` - Add Posting Date validation
- Line ~93: `CreateSalesInvoiceJson()` - Add customer fields to header
- Line ~110: `CreateSalesInvoiceJson()` - Add description and quantity to lines
- Line ~165: `GetBasicInfoFromReceivedDocument()` - Parse vendor info and amount
- Line ~195: `GetCompleteInfoFromReceivedDocument()` - Set vendor and dates
- Line ~215: `GetCompleteInfoFromReceivedDocument()` - Set line details

### Exercise 2: DirectionsConnector

**File:** `application/directions_connector/ConnectorIntegration.Codeunit.al`

**TODOs:**
- Line ~45: `Send()` - Get TempBlob and read JSON
- Line ~50: `Send()` - Create POST request to enqueue
- Line ~55: `Send()` - Send HTTP request
- Line ~85: `ReceiveDocuments()` - Create GET request to peek
- Line ~90: `ReceiveDocuments()` - Send HTTP request
- Line ~105: `ReceiveDocuments()` - Add documents to metadata list
- Line ~120: `DownloadDocument()` - Create GET request to dequeue
- Line ~125: `DownloadDocument()` - Send HTTP request

---

## 🧪 Testing Your Implementation

### Automated Tests

Run the built-in tests to verify your implementation:

1. Open **Test Tool** in Business Central
2. Select Codeunit **50113 "SimpleJson Test"**
3. Run all tests - should pass:
   - `TestExercise1_CheckValidation`
   - `TestExercise1_CheckCreate`
   - `TestExercise2_OutgoingMethodsWork`
   - `TestExercise2_GetBasicInfoFromJSON`
   - `TestExercise2_CreatePurchaseInvoiceFromJSON`

### Manual Testing

**Test Outgoing:**
```powershell
# After sending from BC, check the queue:
$headers = @{ "X-Service-Key" = "your-api-key" }
Invoke-RestMethod -Uri "https://[API-URL]/peek" -Headers $headers
```

**Test Incoming:**
```powershell
# Check how many documents are waiting:
$headers = @{ "X-Service-Key" = "your-api-key" }
$response = Invoke-RestMethod -Uri "https://[API-URL]/peek" -Headers $headers
Write-Host "Documents in queue: $($response.queued_count)"
```

---

## 🐛 Common Issues & Solutions

| Issue | Solution |
|-------|----------|
| "Unauthorized or invalid key" | Re-register in Connector Connection Setup |
| "Failed to parse JSON" | Check JSON structure in E-Document log, verify all required fields |
| "Vendor does not exist" | Create vendor with matching number, or modify JSON to use existing vendor |
| "Document type not supported" | Verify "Simple JSON Format" is selected in E-Document Service |
| "Queue empty" | Send documents first, or ask partner/instructor for test data |
| "Connection failed" | Check API Base URL (must end with `/`), verify server is running |

---

## 💡 Tips for Success

1. **Read TODO comments carefully** - They contain important hints
2. **Use the helper methods** - Pre-written functions save time
3. **Test incrementally** - Don't wait until everything is done
4. **Check logs** - E-Document logs show JSON and HTTP details
5. **Use API_REFERENCE.md** - Complete endpoint documentation
6. **Ask questions** - Instructor is here to help!
7. **Collaborate** - Exchange documents with other participants

---

## 📚 Learning Resources

### E-Document Framework
- **Core README**: [E-Document Core Documentation](https://github.com/microsoft/BCApps/blob/main/src/Apps/W1/EDocument/App/README.md)
- **Interface Source**: [E-Document Interface Code](https://github.com/microsoft/BCApps/blob/main/src/Apps/W1/EDocument/App/src/Document/Interfaces/EDocument.Interface.al)
- **Integration Interfaces**: [Integration Interface Code](https://github.com/microsoft/BCApps/tree/main/src/Apps/W1/EDocument/App/src/Integration/Interfaces)

### Business Central Development
- [BC Developer Documentation](https://learn.microsoft.com/dynamics365/business-central/dev-itpro/)
- [AL Language Reference](https://learn.microsoft.com/dynamics365/business-central/dev-itpro/developer/devenv-reference-overview)
- [AL Samples Repository](https://github.com/microsoft/AL)

### Workshop Materials
- **This Repository**: [BCTech E-Document Samples](https://github.com/microsoft/BCTech/tree/main/samples/EDocument)
- **API Server Code**: `server/app.py` (Python FastAPI implementation)

---

## 🎓 Advanced Topics (Post-Workshop)

After completing the basics, explore:

**Format Enhancements:**
- Support for Credit Memos
- Support for Purchase Orders
- Custom field mappings
- Validation rules
- Document attachments

**Integration Enhancements:**
- Batch operations
- Async status checking (`IDocumentResponseHandler`)
- Approval workflows (`ISentDocumentActions`)
- Custom actions (`IDocumentAction`)
- Error handling and retry logic

**Real-World Examples:**
- PEPPOL format implementation
- Avalara connector integration
- Custom XML formats
- EDI integrations

---

## 🤝 Workshop Collaboration

**Share with Others:**
- Exchange your API key with a partner
- Send documents to each other's queues
- Test receiving from different sources

**Group Activities:**
- Create a shared test queue
- Send documents to the group
- Practice handling various document formats

---

**Happy Coding!** 🚀

*For questions or feedback, contact the workshop instructor.*

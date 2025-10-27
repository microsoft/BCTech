# E-Document Connector Workshop - Complete Guide
## Directions EMEA 2025

Welcome! This comprehensive guide will take you through building a complete E-Document integration solution in **90 minutes**.

---

## 📋 Table of Contents

1. [Workshop Overview](#workshop-overview)
2. [Prerequisites & Setup](#prerequisites--setup)
3. [Understanding the Architecture](#understanding-the-architecture)
4. [Exercise 1: SimpleJson Format (30 min)](#exercise-1-simplejson-format-30-minutes)
5. [Exercise 2: DirectionsConnector (30 min)](#exercise-2-directionsconnector-30-minutes)
6. [Testing Complete Flow (15 min)](#testing-complete-flow-15-minutes)
7. [Troubleshooting](#troubleshooting)
8. [What's Next](#whats-next)

---

## 🎯 Workshop Overview

### What You'll Build

By the end of this workshop, you will have created:

1. **SimpleJson Format** - An E-Document format implementation that:
   - Validates outgoing documents
   - Converts Sales Invoices to JSON
   - Parses incoming JSON documents
   - Creates Purchase Invoices from received data

2. **DirectionsConnector** - An HTTP API integration that:
   - Sends documents to an Azure-hosted API
   - Receives documents from the API queue
   - Handles authentication and error responses
   - Enables complete document round-trips

### What You'll Learn

- ✅ How the E-Document Core framework works
- ✅ How to implement the "E-Document" interface
- ✅ How to implement IDocumentSender and IDocumentReceiver interfaces
- ✅ Best practices for E-Document integrations
- ✅ Testing and debugging E-Document flows

### Timeline

| Time | Duration | Activity |
|------|----------|----------|
| 00:00-00:10 | 10 min | Introduction & Architecture Overview |
| 00:10-00:40 | 30 min | **Exercise 1**: SimpleJson Format |
| 00:40-01:10 | 30 min | **Exercise 2**: DirectionsConnector |
| 01:10-01:25 | 15 min | Testing & Live Demo |
| 01:25-01:30 | 5 min | Wrap-up & Q&A |

---

## 📦 Prerequisites & Setup

### Required

- ✅ Business Central development environment (Sandbox or Docker)
- ✅ VS Code with AL Language extension
- ✅ Basic AL programming knowledge
- ✅ API Base URL (provided by instructor)

### Workspace Structure

Your workspace contains these folders:

```
application/
  ├── simple_json/          # Exercise 1: Format implementation
  │   ├── SimpleJsonFormat.Codeunit.al      # ⚠️ TODO sections
  │   ├── SimpleJsonFormat.EnumExt.al       # ✅ Pre-written
  │   ├── SimpleJsonHelper.Codeunit.al      # ✅ Pre-written helpers
  │   └── app.json
  │
  └── directions_connector/ # Exercise 2: Integration implementation
      ├── ConnectorIntegration.Codeunit.al  # ⚠️ TODO sections
      ├── ConnectorIntegration.EnumExt.al   # ✅ Pre-written
      ├── ConnectorAuth.Codeunit.al         # ✅ Pre-written helpers
      ├── ConnectorRequests.Codeunit.al     # ✅ Pre-written helpers
      ├── ConnectorConnectionSetup.Table.al # ✅ Pre-written
      ├── ConnectorConnectionSetup.Page.al  # ✅ Pre-written
      └── app.json
```

### What's Pre-Written vs. What You'll Implement

**Pre-written (to save time):**
- Extension setup and dependencies
- Enum extensions
- Helper methods for JSON and HTTP operations
- Authentication and connection setup
- UI pages and tables

**You'll implement (core business logic):**
- Document validation
- JSON creation and parsing
- HTTP request handling
- Document sending and receiving

---

## 🏗️ Understanding the Architecture

### E-Document Core Framework

The framework provides a standardized way to integrate Business Central with external systems:

```
┌─────────────────────────────────────────────────────────────┐
│                    Business Central                          │
│                                                               │
│  ┌──────────────┐      ┌────────────────────┐              │
│  │Sales Invoice │─────▶│ E-Document Core    │              │
│  └──────────────┘      └────────────────────┘              │
│                                 │                            │
│                                 ▼                            │
│                    ┌─────────────────────┐                  │
│                    │ Format Interface    │◀─── Exercise 1   │
│                    │  (SimpleJson)       │                  │
│                    └─────────────────────┘                  │
│                                 │                            │
│                                 ▼                            │
│                            JSON Blob                         │
│                                 │                            │
│                                 ▼                            │
│                    ┌─────────────────────┐                  │
│                    │Integration Interface│◀─── Exercise 2   │
│                    │ (Connector)         │                  │
│                    └─────────────────────┘                  │
└─────────────────────────────────┼───────────────────────────┘
                                  │ HTTPS
                                  ▼
                   ┌────────────────────────┐
                   │  Azure API Server      │
                   │  Queue Management      │
                   └────────────────────────┘
```

### Two Key Interfaces

#### 1. Format Interface ("E-Document")
Converts documents between Business Central and external formats.

**Outgoing (BC → External):**
- `Check()` - Validate before sending
- `Create()` - Convert to format (JSON, XML, etc.)

**Incoming (External → BC):**
- `GetBasicInfoFromReceivedDocument()` - Parse metadata
- `GetCompleteInfoFromReceivedDocument()` - Create BC document

#### 2. Integration Interface (IDocumentSender/IDocumentReceiver)
Handles communication with external systems.

**Sending:**
- `Send()` - Send document to external service

**Receiving:**
- `ReceiveDocuments()` - Get list of available documents
- `DownloadDocument()` - Download specific document

### SimpleJson Format Structure

```json
{
  "documentType": "Invoice",
  "documentNo": "SI-001",
  "customerNo": "C001",
  "customerName": "Contoso Ltd.",
  "postingDate": "2025-10-27",
  "currencyCode": "USD",
  "totalAmount": 1250.00,
  "lines": [
    {
      "lineNo": 10000,
      "type": "Item",
      "no": "ITEM-001",
      "description": "Widget",
      "quantity": 5,
      "unitPrice": 250.00,
      "lineAmount": 1250.00
    }
  ]
}
```

### API Endpoints

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/register` | POST | Get API key |
| `/enqueue` | POST | Send document |
| `/peek` | GET | View queue |
| `/dequeue` | GET | Receive & remove document |
| `/clear` | DELETE | Clear queue |

**Authentication:** All endpoints (except `/register`) require:
```
X-Service-Key: your-api-key-here
```

---

## 🚀 Exercise 1: SimpleJson Format (30 minutes)

### Overview

You'll implement the **"E-Document" interface** to convert Business Central documents to/from JSON format.

**File:** `application/simple_json/SimpleJsonFormat.Codeunit.al`

---

### Part A: Validate Outgoing Documents (5 minutes)

**Goal:** Ensure required fields are filled before creating the document.

**Find:** The `Check()` procedure (around line 27)

**Task:** Add validation for the Posting Date field.

**Hint:** Use the same pattern as the Customer No. validation above, using `TestField()` on the Posting Date field.

---

### Part B: Create JSON from Sales Invoice (15 minutes)

**Goal:** Generate a JSON representation of a Sales Invoice with header and lines.

**Find:** The `CreateSalesInvoiceJson()` procedure (around line 68)

**Task:** Complete the TODOs to add:
- customerNo and customerName to the header
- description and quantity to lines

---

### Part C: Parse Incoming JSON (Basic Info) (5 minutes)

**Goal:** Extract basic information from incoming JSON to populate E-Document fields.

**Find:** The `GetBasicInfoFromReceivedDocument()` procedure (around line 140)

**Task:** Complete the TODOs to extract:
- Vendor number (from "customerNo" in JSON)
- Vendor name (from "customerName" in JSON)
- Total amount (from "totalAmount" in JSON)


---

### Part D: Create Purchase Invoice from JSON (5 minutes)

**Goal:** Create a complete Purchase Invoice record from JSON data.

**Find:** The `GetCompleteInfoFromReceivedDocument()` procedure (around line 178)

**Task:** Complete all the TODO sections with '???' placeholders:
- Set vendor number from JSON (from "customerNo")
- Set posting date from JSON (from "postingDate")
- Set line description from JSON (from "description")
- Set line quantity from JSON (from "quantity")
- Set line unit cost from JSON (from "unitPrice")

---

### ✅ Exercise 1 Complete!

You've now implemented the complete E-Document format interface. Build and deploy your extension before moving to Exercise 2.

---

## 🔌 Exercise 2: DirectionsConnector (30 minutes)

### Overview

You'll implement the **IDocumentSender** and **IDocumentReceiver** interfaces to send/receive documents via HTTP API.

**File:** `application/directions_connector/ConnectorIntegration.Codeunit.al`

---

### Part A: Setup Connection (5 minutes)

Before implementing code, you need to configure the connection in Business Central.

**Steps:**

1. **Open Business Central** in your browser

2. **Search** for "Connector Connection Setup"

3. **Enter Configuration:**
   - **API Base URL**: `[Provided by instructor]`
     - Example: `https://edocument-workshop.azurewebsites.net/`
   - **User Name**: Your unique name (e.g., "john-smith")
   
4. **Click "Register"** to get your API key
   - The system will call the API and save your key automatically
   
5. **Click "Test Connection"** to verify
   - You should see "Connection test successful!"

**✅ Test:** You should now have a valid API key stored in the setup.

---

### Part B: Send Document (10 minutes)

**Goal:** Send an E-Document to the API `/enqueue` endpoint.

**Find:** The `Send()` procedure (around line 31)

**Task:** Complete the TODOs to:
- Get the temp blob with JSON from SendContext
- Create POST request to 'enqueue' endpoint
- Send the HTTP request and handle the response

**Hints:**
- Use `SendContext.GetTempBlob()` to get the TempBlob
- Use `ConnectorRequests.ReadJsonFromBlob()` to read JSON text from blob
- Build the endpoint URL: `ConnectorSetup."API Base URL" + 'enqueue'`
- Use `ConnectorRequests.CreatePostRequest()` to create the request
- Use `ConnectorAuth.AddAuthHeader()` to add authentication
- Use `HttpClient.Send()` to send the request
- Use `SendContext.Http().SetHttpRequestMessage()` and `SetHttpResponseMessage()` to log
- Use `ConnectorRequests.CheckResponseSuccess()` to verify success

**✅ Test:** This will be tested when sending a Sales Invoice as an E-Document.

---

### Part C: Receive Documents List (10 minutes)

**Goal:** Retrieve the list of available documents from the API `/peek` endpoint.

**Find:** The `ReceiveDocuments()` procedure (around line 72)

**Task:** Complete the TODOs to:
- Create GET request to 'peek' endpoint
- Send the HTTP request and handle the response
- Add each document TempBlob to DocumentsMetadata list

**Hints:**
- Build endpoint URL: `ConnectorSetup."API Base URL" + 'peek'`
- Use `ConnectorRequests.CreateGetRequest()` for GET requests
- Use `ConnectorAuth.AddAuthHeader()` to add authentication
- Use `HttpClient.Send()` to send the request
- Use `ReceiveContext.Http().Set...()` to log the request/response
- Parse response: `JsonObject.ReadFrom(ResponseText)`
- Get items array: `JsonObject.Get('items', JsonToken)` then `JsonToken.AsArray()`
- For each document in array, write to TempBlob and add to `DocumentsMetadata.Add(TempBlob)`

**✅ Test:** This will be tested when using "Get Documents" action in BC.

---

### Part D: Download Single Document (5 minutes)

**Goal:** Download a single document from the API `/dequeue` endpoint.

**Find:** The `DownloadDocument()` procedure (around line 135)

**Task:** Complete the TODOs to:
- Create GET request to 'dequeue' endpoint
- Send the HTTP request and handle the response

**Hints:**
- Build endpoint URL: `ConnectorSetup."API Base URL" + 'dequeue'`
- Use `ConnectorRequests.CreateGetRequest()` for GET requests
- Use `ConnectorAuth.AddAuthHeader()` to add authentication
- Use `HttpClient.Send()` to send the request
- Use `ReceiveContext.Http().Set...()` to log
- Parse response: `JsonObject.ReadFrom(ResponseText)`
- Get document: `JsonObject.Get('document', JsonToken)`
- Store in TempBlob: `TempBlob := ReceiveContext.GetTempBlob()` then use `ConnectorRequests.WriteTextToBlob()`

**✅ Test:** This will be tested in the complete flow when receiving documents.

---

### ✅ Exercise 2 Complete!

You've now implemented the complete E-Document integration interface. Build and deploy your extension before testing.

---

## 🧪 Testing Complete Flow (15 minutes)

### Setup E-Document Service

1. **Open Business Central**

2. **Search** for "E-Document Services"

3. **Create New Service:**
   - **Code**: `CONNECTOR`
   - **Description**: `Directions Connector Workshop`
   - **Document Format**: `Simple JSON Format - Exercise 1`
   - **Service Integration V2**: `Connector`

4. **Click "Setup"** to open connection setup and verify configuration

5. **Enable the service** by toggling the "Enabled" field

---

### Test Outgoing Flow (Sending Documents)

#### Step 1: Create Sales Invoice

1. **Search** for "Sales Invoices"
2. **Create New** invoice:
   - **Customer**: Any customer (e.g., "10000")
   - **Posting Date**: Today
3. **Add Lines**:
   - Type: Item
   - No.: Any item (e.g., "1000")
   - Quantity: 5
   - Unit Price: 250
4. **Post** the invoice

#### Step 2: Send E-Document

1. **Search** for "E-Documents"
2. **Find** your posted invoice (Status: "Processed")
3. **Click "Send"** action
4. **Verify** status changes to "Sent"
5. **Open the E-Document** and check:
   - "E-Document Log" shows the JSON content
   - "Integration Log" shows the HTTP request/response

#### Step 3: Verify in API

You can verify the document reached the API:

**Option 1 - Browser with extension:**
- Install a browser extension like "ModHeader" or "Modify Header Value"
- Add header: `X-Service-Key: [your-api-key]`
- Navigate to: `[API-Base-URL]/peek`
- You should see your document in the "items" array

**Option 2 - PowerShell:**
```powershell
$headers = @{ "X-Service-Key" = "your-api-key-here" }
Invoke-RestMethod -Uri "https://[API-URL]/peek" -Headers $headers
```

**✅ Success:** You should see your Sales Invoice data in JSON format in the queue!

---

### Test Incoming Flow (Receiving Documents)

#### Step 1: Ensure Documents in Queue

Make sure there are documents in the API queue:
- Either use documents you sent earlier
- Or have a partner send documents to your queue
- Or the instructor can provide test documents

#### Step 2: Receive Documents

1. **Open** "E-Document Services"
2. **Select** your CONNECTOR service
3. **Click** "Get Documents" action
4. **Wait** for processing to complete

#### Step 3: View Received E-Documents

1. **Search** for "E-Documents"
2. **Filter** by Status: "Imported"
3. **Open** a received E-Document
4. **Verify**:
   - Document No. is populated
   - Vendor No. is populated
   - Vendor Name is populated
   - Document Date is set
   - Amount is correct
5. **Check logs**:
   - "E-Document Log" shows the received JSON
   - "Integration Log" shows the HTTP request/response

#### Step 4: Create Purchase Invoice

1. **From the E-Document**, click "Create Document"
2. **Open** the created Purchase Invoice
3. **Verify**:
   - Vendor is set correctly
   - Posting Date matches
   - Currency matches (if specified)
   - Lines are populated with:
     - Item numbers
     - Descriptions
     - Quantities
     - Unit costs
     - Line amounts

#### Step 5: Verify Queue is Empty

Check that documents were removed from the queue:
- Use the `/peek` endpoint again
- The queue should now be empty (or have fewer documents)

**✅ Success:** You've completed a full document round-trip!

---

## 🐛 Troubleshooting


### Additional Resources

**E-Document Framework:**
- [E-Document Core Documentation](https://github.com/microsoft/BCApps/blob/main/src/Apps/W1/EDocument/App/README.md)
- [E-Document Interface Source](https://github.com/microsoft/BCApps/blob/main/src/Apps/W1/EDocument/App/src/Document/Interfaces/EDocument.Interface.al)
- [Integration Interfaces Source](https://github.com/microsoft/BCApps/tree/main/src/Apps/W1/EDocument/App/src/Integration/Interfaces)

**Business Central:**
- [BC Developer Documentation](https://learn.microsoft.com/dynamics365/business-central/dev-itpro/)
- [AL Language Reference](https://learn.microsoft.com/dynamics365/business-central/dev-itpro/developer/devenv-reference-overview)
- [AL Samples Repository](https://github.com/microsoft/AL)

**Workshop Materials:**
- [API Reference](./API_REFERENCE.md) - Complete API documentation
- [Server README](../server/README.md) - API server details

---

## 🎉 Congratulations!

You've successfully completed the E-Document Connector Workshop!

### What You've Accomplished

- ✅ Implemented a complete E-Document format (SimpleJson)
- ✅ Implemented a complete E-Document integration (DirectionsConnector)
- ✅ Sent documents from Business Central to an external API
- ✅ Received documents from an external API into Business Central
- ✅ Converted between BC documents and JSON format
- ✅ Understood the E-Document framework architecture
- ✅ Gained hands-on experience with real-world integration patterns

### Skills Gained

- Understanding of the E-Document Core framework
- Experience implementing format interfaces
- Experience implementing integration interfaces
- HTTP API integration best practices
- JSON data mapping and transformation
- Testing and debugging E-Document flows

---

**Thank you for participating!** We hope you found this workshop valuable and are excited to build E-Document integrations in your projects.

**Happy Coding!** 🚀

---

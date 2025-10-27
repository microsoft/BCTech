# Directions EMEA 2025 - E-Document Connector Workshop

## Workshop Guide

Welcome to the E-Document Connector Workshop! In this hands-on session, you'll build a complete E-Document solution that integrates Business Central with an external API using the E-Document Core framework.

---

## 🎯 What You'll Build

By the end of this workshop, you will have:
1. **SimpleJson Format** - Convert Sales Invoices to JSON and parse incoming Purchase Invoices
2. **DirectionsConnector** - Send and receive documents via HTTP API
3. **Complete Integration** - Full round-trip document exchange

---

## ⏱️ Timeline

- **Exercise 1** (30 min): Implement SimpleJson Format
- **Exercise 2** (30 min): Implement DirectionsConnector
- **Testing** (15 min): End-to-end validation

---

## 📋 Prerequisites

### Required
- Business Central environment (Sandbox or Docker)
- VS Code with AL Language extension
- API Base URL: `[Provided by instructor]`

### Workshop Files
Your workspace contains:
```
application/
  ├── simple_json/          # Exercise 1
  └── directions_connector/ # Exercise 2
```

---

## 🚀 Exercise 1: SimpleJson Format (30 minutes)

In this exercise, you'll implement the **"E-Document" interface** to convert Business Central documents to/from JSON format.

### Part A: Validate Outgoing Documents (5 minutes)

**File**: `application/simple_json/SimpleJsonFormat.Codeunit.al`

**Find**: The `Check()` procedure (around line 27)

**Task**: Add validation to ensure required fields are filled before creating the document.

**Implementation**:
```al
procedure Check(var SourceDocumentHeader: RecordRef; EDocumentService: Record "E-Document Service"; EDocumentProcessingPhase: Enum "E-Document Processing Phase")
var
    SalesInvoiceHeader: Record "Sales Invoice Header";
begin
    case SourceDocumentHeader.Number of
        Database::"Sales Invoice Header":
            begin
                // Validate Customer No. is filled
                SourceDocumentHeader.Field(SalesInvoiceHeader.FieldNo("Sell-to Customer No.")).TestField();
                
                // Validate Posting Date is filled
                SourceDocumentHeader.Field(SalesInvoiceHeader.FieldNo("Posting Date")).TestField();
            end;
    end;
end;
```

**✅ Validation**: Try posting a Sales Invoice - it should validate required fields.

---

### Part B: Create JSON from Sales Invoice (15 minutes)

**File**: `application/simple_json/SimpleJsonFormat.Codeunit.al`

**Find**: The `CreateSalesInvoiceJson()` procedure (around line 93)

**Task**: Generate JSON representation of a Sales Invoice with header and lines.

**Implementation**:
```al
local procedure CreateSalesInvoiceJson(var SourceDocumentHeader: RecordRef; var SourceDocumentLines: RecordRef; var OutStr: OutStream)
var
    SalesInvoiceHeader: Record "Sales Invoice Header";
    SalesInvoiceLine: Record "Sales Invoice Line";
    RootObject: JsonObject;
    LinesArray: JsonArray;
    LineObject: JsonObject;
    JsonText: Text;
begin
    // Get the actual records from RecordRef
    SourceDocumentHeader.SetTable(SalesInvoiceHeader);
    SourceDocumentLines.SetTable(SalesInvoiceLine);

    // Add header fields to JSON object
    RootObject.Add('documentType', 'Invoice');
    RootObject.Add('documentNo', SalesInvoiceHeader."No.");
    RootObject.Add('customerNo', SalesInvoiceHeader."Sell-to Customer No.");
    RootObject.Add('customerName', SalesInvoiceHeader."Sell-to Customer Name");
    RootObject.Add('postingDate', Format(SalesInvoiceHeader."Posting Date", 0, '<Year4>-<Month,2>-<Day,2>'));
    RootObject.Add('currencyCode', SalesInvoiceHeader."Currency Code");
    RootObject.Add('totalAmount', SalesInvoiceHeader."Amount Including VAT");
    
    // Create lines array
    if SalesInvoiceLine.FindSet() then
        repeat
            // Create line object
            Clear(LineObject);
            LineObject.Add('lineNo', SalesInvoiceLine."Line No.");
            LineObject.Add('type', Format(SalesInvoiceLine.Type));
            LineObject.Add('no', SalesInvoiceLine."No.");
            LineObject.Add('description', SalesInvoiceLine.Description);
            LineObject.Add('quantity', SalesInvoiceLine.Quantity);
            LineObject.Add('unitPrice', SalesInvoiceLine."Unit Price");
            LineObject.Add('lineAmount', SalesInvoiceLine."Amount Including VAT");
            LinesArray.Add(LineObject);
        until SalesInvoiceLine.Next() = 0;
    
    // Add lines array to root object
    RootObject.Add('lines', LinesArray);
    
    // Write JSON to stream
    RootObject.WriteTo(JsonText);
    OutStr.WriteText(JsonText);
end;
```

**✅ Validation**: 
1. Create and post a Sales Invoice
2. Open the E-Document list
3. View the E-Document and check the JSON content in the log

---

### Part C: Parse Incoming JSON (Basic Info) (5 minutes)

**File**: `application/simple_json/SimpleJsonFormat.Codeunit.al`

**Find**: The `GetBasicInfoFromReceivedDocument()` procedure (around line 151)

**Task**: Extract basic information from incoming JSON to populate E-Document fields.

**Implementation**:
```al
procedure GetBasicInfoFromReceivedDocument(var EDocument: Record "E-Document"; var TempBlob: Codeunit "Temp Blob")
var
    JsonObject: JsonObject;
    JsonToken: JsonToken;
    SimpleJsonHelper: Codeunit "SimpleJson Helper";
begin
    // Parse JSON from blob
    if not SimpleJsonHelper.ReadJsonFromBlob(TempBlob, JsonObject) then
        Error('Failed to parse JSON document');
    
    // Set document type to Purchase Invoice
    EDocument."Document Type" := EDocument."Document Type"::"Purchase Invoice";
    
    // Extract document number
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'documentNo', JsonToken) then
        EDocument."Document No." := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Document No."));
    
    // Extract vendor number (from customerNo in JSON)
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'customerNo', JsonToken) then
        EDocument."Bill-to/Pay-to No." := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Bill-to/Pay-to No."));
    
    // Extract vendor name
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'customerName', JsonToken) then
        EDocument."Bill-to/Pay-to Name" := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Bill-to/Pay-to Name"));
    
    // Extract posting date
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'postingDate', JsonToken) then
        EDocument."Document Date" := SimpleJsonHelper.GetJsonTokenDate(JsonToken);
    
    // Extract currency code
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'currencyCode', JsonToken) then
        EDocument."Currency Code" := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(EDocument."Currency Code"));
end;
```

**✅ Validation**: This will be tested in Exercise 2 when receiving documents.

---

### Part D: Create Purchase Invoice from JSON (5 minutes)

**File**: `application/simple_json/SimpleJsonFormat.Codeunit.al`

**Find**: The `GetCompleteInfoFromReceivedDocument()` procedure (around line 188)

**Task**: Create a Purchase Invoice record from JSON data.

**Implementation**:
```al
procedure GetCompleteInfoFromReceivedDocument(var EDocument: Record "E-Document"; var CreatedDocumentHeader: RecordRef; var CreatedDocumentLines: RecordRef; var TempBlob: Codeunit "Temp Blob")
var
    PurchaseHeader: Record "Purchase Header";
    PurchaseLine: Record "Purchase Line";
    JsonObject: JsonObject;
    JsonToken: JsonToken;
    JsonArray: JsonArray;
    JsonLineToken: JsonToken;
    SimpleJsonHelper: Codeunit "SimpleJson Helper";
    LineNo: Integer;
begin
    // Parse JSON from blob
    if not SimpleJsonHelper.ReadJsonFromBlob(TempBlob, JsonObject) then
        Error('Failed to parse JSON document');
    
    // Create Purchase Header
    PurchaseHeader.Init();
    PurchaseHeader."Document Type" := PurchaseHeader."Document Type"::Invoice;
    PurchaseHeader.Insert(true);
    
    // Set vendor from JSON
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'customerNo', JsonToken) then
        PurchaseHeader.Validate("Buy-from Vendor No.", SimpleJsonHelper.GetJsonTokenValue(JsonToken));
    
    // Set posting date
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'postingDate', JsonToken) then
        PurchaseHeader.Validate("Posting Date", SimpleJsonHelper.GetJsonTokenDate(JsonToken));
    
    // Set currency code (if not blank)
    if SimpleJsonHelper.SelectJsonToken(JsonObject, 'currencyCode', JsonToken) then begin
        if SimpleJsonHelper.GetJsonTokenValue(JsonToken) <> '' then
            PurchaseHeader.Validate("Currency Code", SimpleJsonHelper.GetJsonTokenValue(JsonToken));
    end;
    
    PurchaseHeader.Modify(true);
    
    // Create Purchase Lines from JSON array
    if JsonObject.Get('lines', JsonToken) then begin
        JsonArray := JsonToken.AsArray();
        LineNo := 10000;
        
        foreach JsonLineToken in JsonArray do begin
            JsonObject := JsonLineToken.AsObject();
            
            PurchaseLine.Init();
            PurchaseLine."Document Type" := PurchaseHeader."Document Type";
            PurchaseLine."Document No." := PurchaseHeader."No.";
            PurchaseLine."Line No." := LineNo;
            
            // Set type (default to Item)
            PurchaseLine.Type := PurchaseLine.Type::Item;
            
            // Set item number
            if SimpleJsonHelper.SelectJsonToken(JsonObject, 'no', JsonToken) then
                PurchaseLine.Validate("No.", SimpleJsonHelper.GetJsonTokenValue(JsonToken));
            
            // Set description
            if SimpleJsonHelper.SelectJsonToken(JsonObject, 'description', JsonToken) then
                PurchaseLine.Description := CopyStr(SimpleJsonHelper.GetJsonTokenValue(JsonToken), 1, MaxStrLen(PurchaseLine.Description));
            
            // Set quantity
            if SimpleJsonHelper.SelectJsonToken(JsonObject, 'quantity', JsonToken) then
                PurchaseLine.Validate(Quantity, SimpleJsonHelper.GetJsonTokenDecimal(JsonToken));
            
            // Set unit price
            if SimpleJsonHelper.SelectJsonToken(JsonObject, 'unitPrice', JsonToken) then
                PurchaseLine.Validate("Direct Unit Cost", SimpleJsonHelper.GetJsonTokenDecimal(JsonToken));
            
            PurchaseLine.Insert(true);
            LineNo += 10000;
        end;
    end;
    
    // Return via RecordRef
    CreatedDocumentHeader.GetTable(PurchaseHeader);
    CreatedDocumentLines.GetTable(PurchaseLine);
end;
```

**✅ Validation**: This will be tested in Exercise 2 when creating documents from received E-Documents.

---

## 🔌 Exercise 2: DirectionsConnector (30 minutes)

In this exercise, you'll implement the **IDocumentSender** and **IDocumentReceiver** interfaces to send/receive documents via HTTP API.

### Part A: Setup Connection (5 minutes)

**Manual Setup**:
1. Open Business Central
2. Search for "Directions Connection Setup"
3. Enter the API Base URL: `[Provided by instructor]`
4. Enter your name (unique identifier)
5. Click "Register" to get your API key
6. Click "Test Connection" to verify

**✅ Validation**: You should see "Connection test successful!"

---

### Part B: Send Document (10 minutes)

**File**: `application/directions_connector/DirectionsIntegration.Codeunit.al`

**Find**: The `Send()` procedure (around line 31)

**Task**: Send an E-Document to the API /enqueue endpoint.

**Implementation**:
```al
procedure Send(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; SendContext: Codeunit SendContext)
var
    DirectionsSetup: Record "Directions Connection Setup";
    DirectionsAuth: Codeunit "Directions Auth";
    DirectionsRequests: Codeunit "Directions Requests";
    HttpClient: HttpClient;
    HttpRequest: HttpRequestMessage;
    HttpResponse: HttpResponseMessage;
    TempBlob: Codeunit "Temp Blob";
    JsonContent: Text;
begin
    // Get connection setup
    DirectionsAuth.GetConnectionSetup(DirectionsSetup);
    
    // Get document content from SendContext
    SendContext.GetTempBlob(TempBlob);
    JsonContent := DirectionsRequests.ReadJsonFromBlob(TempBlob);
    
    // Create POST request to /enqueue
    DirectionsRequests.CreatePostRequest(DirectionsSetup."API Base URL" + 'enqueue', JsonContent, HttpRequest);
    
    // Add authentication header
    DirectionsAuth.AddAuthHeader(HttpRequest, DirectionsSetup);
    
    // Log request (for E-Document framework logging)
    SendContext.Http().SetHttpRequestMessage(HttpRequest);
    
    // Send request
    if not HttpClient.Send(HttpRequest, HttpResponse) then
        Error('Failed to send document to API.');
    
    // Log response and check success
    SendContext.Http().SetHttpResponseMessage(HttpResponse);
    DirectionsRequests.CheckResponseSuccess(HttpResponse);
end;
```

**✅ Validation**:
1. Create and post a Sales Invoice
2. Open the E-Document list
3. Send the E-Document (it should succeed)
4. Use the /peek endpoint in a browser to verify the document is in the queue

---

### Part C: Receive Documents List (10 minutes)

**File**: `application/directions_connector/DirectionsIntegration.Codeunit.al`

**Find**: The `ReceiveDocuments()` procedure (around line 72)

**Task**: Retrieve the list of available documents from the API /peek endpoint.

**Implementation**:
```al
procedure ReceiveDocuments(var EDocumentService: Record "E-Document Service"; DocumentsMetadata: Codeunit "Temp Blob List"; ReceiveContext: Codeunit ReceiveContext)
var
    DirectionsSetup: Record "Directions Connection Setup";
    DirectionsAuth: Codeunit "Directions Auth";
    DirectionsRequests: Codeunit "Directions Requests";
    HttpClient: HttpClient;
    HttpRequest: HttpRequestMessage;
    HttpResponse: HttpResponseMessage;
    JsonObject: JsonObject;
    JsonToken: JsonToken;
    JsonArray: JsonArray;
    TempBlob: Codeunit "Temp Blob";
    ResponseText: Text;
    DocumentJson: Text;
begin
    // Get connection setup
    DirectionsAuth.GetConnectionSetup(DirectionsSetup);
    
    // Create GET request to /peek
    DirectionsRequests.CreateGetRequest(DirectionsSetup."API Base URL" + 'peek', HttpRequest);
    
    // Add authentication
    DirectionsAuth.AddAuthHeader(HttpRequest, DirectionsSetup);
    
    // Log request
    ReceiveContext.Http().SetHttpRequestMessage(HttpRequest);
    
    // Send request
    if not HttpClient.Send(HttpRequest, HttpResponse) then
        Error('Failed to retrieve documents from API.');
    
    // Log response and check success
    ReceiveContext.Http().SetHttpResponseMessage(HttpResponse);
    DirectionsRequests.CheckResponseSuccess(HttpResponse);
    
    // Parse response and extract documents
    ResponseText := DirectionsRequests.GetResponseText(HttpResponse);
    if JsonObject.ReadFrom(ResponseText) then begin
        if JsonObject.Get('items', JsonToken) then begin
            JsonArray := JsonToken.AsArray();
            foreach JsonToken in JsonArray do begin
                // Create a TempBlob for each document metadata
                Clear(TempBlob);
                JsonToken.WriteTo(DocumentJson);
                DirectionsRequests.WriteTextToBlob(DocumentJson, TempBlob);
                DocumentsMetadata.Add(TempBlob);
            end;
        end;
    end;
end;
```

**✅ Validation**: This will be tested when running "Get Documents" action in BC.

---

### Part D: Download Single Document (5 minutes)

**File**: `application/directions_connector/DirectionsIntegration.Codeunit.al`

**Find**: The `DownloadDocument()` procedure (around line 135)

**Task**: Download a single document from the API /dequeue endpoint.

**Implementation**:
```al
procedure DownloadDocument(var EDocument: Record "E-Document"; var EDocumentService: Record "E-Document Service"; DocumentMetadata: codeunit "Temp Blob"; ReceiveContext: Codeunit ReceiveContext)
var
    DirectionsSetup: Record "Directions Connection Setup";
    DirectionsAuth: Codeunit "Directions Auth";
    DirectionsRequests: Codeunit "Directions Requests";
    HttpClient: HttpClient;
    HttpRequest: HttpRequestMessage;
    HttpResponse: HttpResponseMessage;
    JsonObject: JsonObject;
    JsonToken: JsonToken;
    TempBlob: Codeunit "Temp Blob";
    ResponseText: Text;
    DocumentJson: Text;
begin
    // Get connection setup
    DirectionsAuth.GetConnectionSetup(DirectionsSetup);
    
    // Create GET request to /dequeue
    DirectionsRequests.CreateGetRequest(DirectionsSetup."API Base URL" + 'dequeue', HttpRequest);
    
    // Add authentication
    DirectionsAuth.AddAuthHeader(HttpRequest, DirectionsSetup);
    
    // Log request
    ReceiveContext.Http().SetHttpRequestMessage(HttpRequest);
    
    // Send request
    if not HttpClient.Send(HttpRequest, HttpResponse) then
        Error('Failed to download document from API.');
    
    // Log response and check success
    ReceiveContext.Http().SetHttpResponseMessage(HttpResponse);
    DirectionsRequests.CheckResponseSuccess(HttpResponse);
    
    // Parse response and extract document
    ResponseText := DirectionsRequests.GetResponseText(HttpResponse);
    if JsonObject.ReadFrom(ResponseText) then begin
        if JsonObject.Get('document', JsonToken) then begin
            JsonToken.WriteTo(DocumentJson);
            ReceiveContext.GetTempBlob(TempBlob);
            DirectionsRequests.WriteTextToBlob(DocumentJson, TempBlob);
        end else
            Error('No document found in response.');
    end;
end;
```

**✅ Validation**: This will be tested in the complete flow below.

---

## 🧪 Testing - Complete Flow (15 minutes)

### Setup E-Document Service

1. Open "E-Document Services"
2. Create a new service:
   - **Code**: DIRECTIONS
   - **Description**: Directions Connector
   - **Document Format**: Simple JSON Format
   - **Service Integration**: Directions Connector
3. Click "Setup Service Integration" and verify your connection
4. Enable the service

### Test Outgoing Flow

1. **Create Sales Invoice**:
   - Customer: Any customer
   - Add at least one line item
   - Post the invoice

2. **Send E-Document**:
   - Open "E-Documents" list
   - Find your posted invoice
   - Action: "Send"
   - Status should change to "Sent"

3. **Verify in API**:
   - Open browser: `[API Base URL]/peek`
   - Add header: `X-Service-Key: [Your API Key]`
   - You should see your document in the "items" array

### Test Incoming Flow

1. **Receive Documents**:
   - Open "E-Document Services"
   - Select your DIRECTIONS service
   - Action: "Get Documents"
   - New E-Documents should appear with status "Imported"

2. **View Received Document**:
   - Open the received E-Document
   - Check the JSON content in the log
   - Verify basic info is populated (vendor, date, etc.)

3. **Create Purchase Invoice**:
   - Action: "Create Document"
   - A Purchase Invoice should be created
   - Open the Purchase Invoice and verify:
     - Vendor is set correctly
     - Lines are populated with items, quantities, prices
     - Dates and currency match

4. **Verify Queue**:
   - Check /peek endpoint again
   - The queue should be empty (documents were dequeued)

---

## 🎉 Success Criteria

You have successfully completed the workshop if:
- ✅ You can post a Sales Invoice and see it as an E-Document
- ✅ The E-Document contains valid JSON
- ✅ You can send the E-Document to the API
- ✅ The document appears in the API queue
- ✅ You can receive documents from the API
- ✅ Purchase Invoices are created from received documents
- ✅ All data is mapped correctly

---

## 🐛 Troubleshooting

### "Failed to connect to API"
- Check the API Base URL in setup
- Verify the API server is running
- Check firewall settings

### "Unauthorized or invalid key"
- Re-register in the setup page
- Verify the API key is saved
- Check that you're using the correct key header

### "Document type not supported"
- Verify you selected "Simple JSON Format" in E-Document Service
- Check the format enum extension is compiled

### "Failed to parse JSON"
- Check the JSON structure in E-Document log
- Verify all required fields are present
- Look for syntax errors (missing commas, brackets)

### "Vendor does not exist"
- Create a vendor with the same number as the customer in the JSON
- Or modify the JSON to use an existing vendor number

---

## 📚 Additional Resources

- [E-Document Core README](../../README.md)
- [API Reference](../API_REFERENCE.md)
- [Workshop Plan](../WORKSHOP_PLAN.md)

---

## 🎓 Homework / Advanced Exercises

Want to learn more? Try these:
1. Add support for Credit Memos
2. Implement `GetResponse()` for async status checking
3. Add custom fields to the JSON format
4. Implement validation rules for incoming documents
5. Add error handling and retry logic
6. Create a batch send function

---

**Congratulations!** 🎊 You've completed the E-Document Connector Workshop!

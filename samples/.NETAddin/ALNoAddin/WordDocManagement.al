// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------
codeunit 50110 WordDocManagement
{
    // Create the customer list word document by calling
    // the Azure Function App
    procedure CreateCustomerDirectoryWordDocument()
    var
        FileName: Text;
        Json: Text;
        InStr: InStream;
    begin
        Json := SerializeCustomersToJson();
        if (Strlen(Json) = 0) then begin
            Message('No customers');
            exit;
        end;

        InStr := GetWordDocumentFromService(json);

        Message('Customer list created');
        FileName := 'CustomerList.docx';
        DownloadFromStream(inStr, 'Customer list document', '', 'All Files (*.*)|*.*', FileName);
    end;

    // Call the CreateCustomerDirectoryWordDoc Azure Function App 
    // using a HTTP request and get the return Word document in a stream
    local procedure GetWordDocumentFromService(Customers: Text) Result: InStream
    var
        Client: HttpClient;
        Response: HttpResponseMessage;
        Headers: HttpHeaders;
        Content: HttpContent;
        LocalURL: label 'http://localhost:7071/api/CreateCustomerDirectoryWordDoc', Locked = true;
        URL: label 'https://worddocmanagerazurefunc.azurewebsites.net/api/CreateCustomerDirectoryWordDoc', Locked = true;
    begin
        Content.Clear();
        Content.WriteFrom(Format(Customers));
        Content.GetHeaders(Headers);
        Headers.Remove('Content-Type');
        Headers.Add('Content-Type', 'application/json');

        Client.Post(URL, Content, Response);
        Response.Content().ReadAs(Result);
    end;

    // Serialize the customer list to JSON
    local procedure SerializeCustomersToJson() Json: Text
    var
        JsonCustomers: JsonArray;
        Customer: Record Customer;

    begin
        if not Customer.FindFirst() then begin
            Json := '';
            exit;
        end;

        repeat
            AddCustomer(JsonCustomers, Customer);
        until customer.Next() = 0;

        JsonCustomers.WriteTo(Json);
    end;

    local procedure AddCustomer(JsonCustomers: JsonArray; Customer: Record Customer)
    var
        JsonCustomer: JsonObject;
    begin
        JsonCustomer.Add('name', Customer.Name);
        JsonCustomer.Add('street', Customer.Address);
        JsonCustomer.Add('zipCode', Customer."Post Code");
        JsonCustomer.Add('city', Customer.City);
        JsonCustomers.Add(JsonCustomer);
    end;

}
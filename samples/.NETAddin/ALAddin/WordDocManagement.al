// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------
dotnet
{
    // Reference to the .net service-side Addin
    assembly(WordDocManagerAddin)
    {
        type(WordDocManagerAddin.WordDocManager; WordDocManager) { }
    }

    assembly(mscorlib)
    {
        type("System.Collections.Generic.List`1"; "GenericList1")
        {
        }
    }
}

codeunit 50110 WordDocManagement
{
    // Create the customer list word document by calling
    // the .net server-side addin
    procedure CreateCustomerDirectoryWordDocument()
    var
        CustomerList: DotNet GenericList1;
        CustomerDetails: DotNet GenericList1;
        WordDocManager: DotNet WordDocManager;
        InStr: InStream;
        FileName: Text;
        Customer: Record Customer;

    begin
        WordDocManager := WordDocManager.WordDocManager();
        CustomerList := customerList.List;

        if not Customer.FindFirst() then begin
            Message('No customers');
            exit;
        end;

        repeat
            CustomerDetails := customerDetails.List;
            CustomerDetails.Add(customer.Name);
            CustomerDetails.Add(customer.Address);
            CustomerDetails.Add(customer."Post Code");
            CustomerDetails.Add(customer.City);
            CustomerList.Add(customerDetails);
        until customer.Next() = 0;

        InStr := WordDocManager.CreateCustomerDirectoryDocument(customerList);

        Message('Customer list created');
        FileName := 'CustomerList.docx';
        DownloadFromStream(InStr, 'Customer list document', '', 'All Files (*.*)|*.*', fileName);

    end;
}
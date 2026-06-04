// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Test.Setup;

using Microsoft.Sales.Document;
using Microsoft.Sales.History;
using System.IO;
using System.TestLibraries.Agents;
using System.TestTools.TestRunner;
using System.Utilities;

codeunit 53749 "SR Resource Provider" implements IAgentTestResourceProvider
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure GetResource(ResourcePath: Text; var ResourceInStream: InStream; var FileName: Text[250]; var MIMEType: Text[100])
    var
        FileManagement: Codeunit "File Management";
    begin
        FileName := CopyStr(FileManagement.GetFileName(ResourcePath), 1, MaxStrLen(FileName));
        NavApp.GetResource(ResourcePath, ResourceInStream);
        MIMEType := GetMIMEType(FileName);
    end;

    procedure GenerateResource(GeneratorName: Text; GeneratorData: Codeunit "Test Input Json"; var ResourceInStream: InStream; var FileName: Text[250]; var MIMEType: Text[100])
    begin
        case GeneratorName of
            SalesInvoiceTok:
                GenerateSalesInvoicePdf(GeneratorData, ResourceInStream, FileName, MIMEType);
            else
                Error(UnknownFileGeneratorErr, GeneratorName);
        end;
    end;

    local procedure GenerateSalesInvoicePdf(GeneratorData: Codeunit "Test Input Json"; var ResourceInStream: InStream; var FileName: Text[250]; var MIMEType: Text[100])
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
        SalesInvoiceHeader: Record "Sales Invoice Header";
        LinesData, LineElement, Element : Codeunit "Test Input Json";
        RecRef: RecordRef;
        OutStream: OutStream;
        CustomerNo: Code[20];
        ItemNo: Code[20];
        PostedInvoiceNo: Code[20];
        ElementExists: Boolean;
        I: Integer;
    begin
        CustomerNo := CopyStr(GeneratorData.Element(CustomerNoTok).ValueAsText(), 1, MaxStrLen(CustomerNo));

        LibrarySales.CreateSalesHeader(SalesHeader, SalesHeader."Document Type"::Invoice, CustomerNo);

        Element := GeneratorData.ElementExists(DateTok, ElementExists);
        if ElementExists then begin
            SalesHeader.Validate("Posting Date", Element.ValueAsDate());
            SalesHeader.Validate("Document Date", Element.ValueAsDate());
            SalesHeader.Modify(true);
        end;

        LinesData := GeneratorData.Element(LinesTok);
        for I := 0 to LinesData.GetElementCount() - 1 do begin
            LineElement := LinesData.ElementAt(I);
            ItemNo := CopyStr(LineElement.Element(ItemNoTok).ValueAsText(), 1, MaxStrLen(ItemNo));

            LibrarySales.CreateSalesLine(
                SalesLine, SalesHeader,
                SalesLine.Type::Item, ItemNo,
                LineElement.Element(QuantityTok).ValueAsInteger());

            Element := LineElement.ElementExists(DescriptionTok, ElementExists);
            if ElementExists then begin
                SalesLine.Validate(Description, CopyStr(Element.ValueAsText(), 1, MaxStrLen(SalesLine.Description)));
                SalesLine.Modify(true);
            end;
        end;

        PostedInvoiceNo := LibrarySales.PostSalesDocument(SalesHeader, true, true);
        SalesInvoiceHeader.Get(PostedInvoiceNo);
        SalesInvoiceHeader.SetRecFilter();
        RecRef.GetTable(SalesInvoiceHeader);
        TempBlob.CreateOutStream(OutStream);

        if not Report.SaveAs(Report::"Standard Sales - Invoice", '', ReportFormat::Pdf, OutStream, RecRef) then
            Error(FailedToGenerateFileErr, SalesInvoiceTok);

        TempBlob.CreateInStream(ResourceInStream);

        FileName := CopyStr(StrSubstNo(InvoiceFileNameLbl, PostedInvoiceNo), 1, MaxStrLen(FileName));
        MIMEType := PdfMimeTypeTok;
    end;

    local procedure GetMIMEType(FileNameValue: Text[250]): Text[100]
    begin
        if FileNameValue.EndsWith('.png') then
            exit('image/png');
        if FileNameValue.EndsWith('.jpg') or FileNameValue.EndsWith('.jpeg') then
            exit('image/jpeg');
        if FileNameValue.EndsWith('.pdf') then
            exit(PdfMimeTypeTok);
        exit('application/octet-stream');
    end;

    var
        TempBlob: Codeunit "Temp Blob";
        LibrarySales: Codeunit "Library - Sales";
        CustomerNoTok: Label 'customer_no', Locked = true;
        DateTok: Label 'date', Locked = true;
        LinesTok: Label 'lines', Locked = true;
        ItemNoTok: Label 'item_no', Locked = true;
        QuantityTok: Label 'quantity', Locked = true;
        DescriptionTok: Label 'description', Locked = true;
        SalesInvoiceTok: Label 'sales-invoice', Locked = true;
        PdfMimeTypeTok: Label 'application/pdf', Locked = true;
        InvoiceFileNameLbl: Label 'Invoice_%1.pdf', Locked = true, Comment = '%1 = document number';
        UnknownFileGeneratorErr: Label 'Unknown file generator: %1', Comment = '%1 = generator name';
        FailedToGenerateFileErr: Label 'Failed to generate file for generator: %1', Comment = '%1 = generator name';
}

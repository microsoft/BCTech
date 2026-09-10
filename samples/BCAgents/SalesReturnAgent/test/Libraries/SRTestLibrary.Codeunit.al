// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Test.Libraries;

using Microsoft.Inventory.Item;
using Microsoft.Sales.Customer;
using Microsoft.Sales.Document;
using Microsoft.Sales.History;
using System.TestTools.AITestToolkit;
using System.TestTools.TestRunner;

codeunit 53746 "SR Test Library"
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    /// <summary>
    /// Creates customers from the suite setup data.
    /// </summary>
    procedure CreateCustomers(TestSetup: Codeunit "Test Input Json")
    var
        ActionData: Codeunit "Test Input Json";
        I: Integer;
    begin
        if not TryGetActionData(TestSetup, CreateCustomersTok, ActionData) then
            exit;

        for I := 0 to ActionData.GetElementCount() - 1 do
            CreateCustomer(ActionData.ElementAt(I));
    end;

    /// <summary>
    /// Creates items from the suite setup data.
    /// </summary>
    procedure CreateItems(TestSetup: Codeunit "Test Input Json")
    var
        ActionData: Codeunit "Test Input Json";
        I: Integer;
    begin
        if not TryGetActionData(TestSetup, CreateItemsTok, ActionData) then
            exit;

        for I := 0 to ActionData.GetElementCount() - 1 do
            CreateItem(ActionData.ElementAt(I));
    end;

    /// <summary>
    /// Creates credit memo test data for the current turn.
    /// </summary>
    procedure CreateCreditMemoTestData(TestSetup: Codeunit "Test Input Json")
    var
        ActionData: Codeunit "Test Input Json";
    begin
        if not TryGetActionData(TestSetup, CreatePostedShipmentTok, ActionData) then
            exit;

        // Posted shipment data is set up so the agent has context for the return.
        // The actual credit memo creation is done by the agent during the test.
    end;

    /// <summary>
    /// Creates posted sales invoices from the suite setup data.
    /// </summary>
    procedure CreatePostedSalesInvoices(TestSetup: Codeunit "Test Input Json")
    var
        ActionData: Codeunit "Test Input Json";
        I: Integer;
    begin
        if not TryGetActionData(TestSetup, CreatePostedSalesInvoicesTok, ActionData) then
            exit;

        for I := 0 to ActionData.GetElementCount() - 1 do
            CreatePostedSalesInvoice(ActionData.ElementAt(I));
    end;

    /// <summary>
    /// Validates that a credit memo was created with the expected data.
    /// </summary>
    procedure ValidateCreditMemoCreated(var ErrorReason: Text): Boolean
    var
        ExpectedData: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        ElementExists: Boolean;
        CreditMemoCreated: Boolean;
        CreditMemoPosted: Boolean;
    begin
        ExpectedData := AITTestContext.GetExpectedData();

        Element := ExpectedData.ElementExists(CreditMemoCreatedTok, ElementExists);
        if ElementExists then
            CreditMemoCreated := Element.ValueAsBoolean()
        else
            exit(true); // No validation needed

        Element := ExpectedData.ElementExists(CreditMemoPostedTok, ElementExists);
        if ElementExists then
            CreditMemoPosted := Element.ValueAsBoolean();

        if CreditMemoPosted then
            exit(ValidatePostedCreditMemo(ExpectedData, CreditMemoCreated, ErrorReason));

        exit(ValidateUnpostedCreditMemo(ExpectedData, CreditMemoCreated, ErrorReason));
    end;

    local procedure ValidateUnpostedCreditMemo(ExpectedData: Codeunit "Test Input Json"; CreditMemoCreated: Boolean; var ErrorReason: Text): Boolean
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
        LinesData: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        ExpectedCustomer: Text;
        ExpectedItemNo: Text;
        ExpectedUOM: Text;
        ExpectedQuantity: Integer;
        ElementExists: Boolean;
        LinesExist: Boolean;
        HasExpectedUOM: Boolean;
        I: Integer;
    begin
        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::"Credit Memo");
        if not CreditMemoCreated then begin
            if not SalesHeader.IsEmpty() then begin
                ErrorReason := CreditMemoShouldNotExistLbl;
                exit(false);
            end;
            exit(true);
        end;

        if SalesHeader.IsEmpty() then begin
            ErrorReason := CreditMemoNotCreatedLbl;
            exit(false);
        end;

        // Validate customer
        Element := ExpectedData.ElementExists(CreditMemoCustomerTok, ElementExists);
        if ElementExists then begin
            ExpectedCustomer := Element.ValueAsText();
            SalesHeader.SetRange("Sell-to Customer No.", CopyStr(ExpectedCustomer, 1, 20));
            if SalesHeader.IsEmpty() then begin
                ErrorReason := StrSubstNo(WrongCustomerLbl, ExpectedCustomer);
                exit(false);
            end;
        end;

        SalesHeader.FindFirst();

        // Resolve expected UOM (applies to all lines)
        Element := ExpectedData.ElementExists(ExpectedUomTok, HasExpectedUOM);
        if HasExpectedUOM then
            ExpectedUOM := Element.ValueAsText();

        // Validate lines
        LinesData := ExpectedData.ElementExists(CreditMemoLinesTok, LinesExist);
        if LinesExist then begin
            SalesLine.SetRange("Document Type", SalesLine."Document Type"::"Credit Memo");
            SalesLine.SetRange("Document No.", SalesHeader."No.");
            SalesLine.SetRange(Type, SalesLine.Type::Item);

            for I := 0 to LinesData.GetElementCount() - 1 do begin
                Element := LinesData.ElementAt(I).Element(ItemNoTok);
                ExpectedItemNo := Element.ValueAsText();
                ExpectedQuantity := LinesData.ElementAt(I).Element(QuantityTok).ValueAsInteger();

                SalesLine.SetRange("No.", CopyStr(ExpectedItemNo, 1, 20));
                if SalesLine.IsEmpty() then begin
                    ErrorReason := StrSubstNo(ItemLineNotFoundLbl, ExpectedItemNo);
                    exit(false);
                end;

                SalesLine.FindFirst();
                if SalesLine.Quantity <> ExpectedQuantity then begin
                    ErrorReason := StrSubstNo(WrongQuantityLbl, ExpectedItemNo, ExpectedQuantity, SalesLine.Quantity);
                    exit(false);
                end;

                Element := LinesData.ElementAt(I).ElementExists(UnitPriceTok, ElementExists);
                if ElementExists then
                    if SalesLine."Unit Price" <> Element.ValueAsDecimal() then begin
                        ErrorReason := StrSubstNo(WrongUnitPriceLbl, ExpectedItemNo, Element.ValueAsDecimal(), SalesLine."Unit Price");
                        exit(false);
                    end;

                Element := LinesData.ElementAt(I).ElementExists(LineAmountTok, ElementExists);
                if ElementExists then
                    if SalesLine."Line Amount" <> Element.ValueAsDecimal() then begin
                        ErrorReason := StrSubstNo(WrongLineAmountLbl, ExpectedItemNo, Element.ValueAsDecimal(), SalesLine."Line Amount");
                        exit(false);
                    end;

                if HasExpectedUOM then
                    if SalesLine."Unit of Measure Code" <> CopyStr(ExpectedUOM, 1, 10) then begin
                        ErrorReason := StrSubstNo(WrongUomLbl, ExpectedUOM, SalesLine."Unit of Measure Code", ExpectedItemNo);
                        exit(false);
                    end;
            end;

            // Verify no extra lines beyond what is expected
            SalesLine.SetRange("No.");
            if SalesLine.Count() <> LinesData.GetElementCount() then begin
                ErrorReason := StrSubstNo(LineCountMismatchLbl, LinesData.GetElementCount(), SalesLine.Count());
                exit(false);
            end;
        end;

        // Validate credit memo total
        Element := ExpectedData.ElementExists(CreditMemoTotalTok, ElementExists);
        if ElementExists then begin
            SalesHeader.CalcFields(Amount);
            if SalesHeader.Amount <> Element.ValueAsDecimal() then begin
                ErrorReason := StrSubstNo(WrongTotalLbl, Element.ValueAsDecimal(), SalesHeader.Amount);
                exit(false);
            end;
        end;

        exit(true);
    end;

    local procedure ValidatePostedCreditMemo(ExpectedData: Codeunit "Test Input Json"; CreditMemoCreated: Boolean; var ErrorReason: Text): Boolean
    var
        SalesCrMemoHeader: Record "Sales Cr.Memo Header";
        SalesCrMemoLine: Record "Sales Cr.Memo Line";
        LinesData: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        ExpectedCustomer: Text;
        ExpectedItemNo: Text;
        ExpectedUOM: Text;
        ExpectedQuantity: Integer;
        ElementExists: Boolean;
        LinesExist: Boolean;
        HasExpectedUOM: Boolean;
        I: Integer;
    begin
        if not CreditMemoCreated then begin
            ErrorReason := PostedCreditMemoConflictLbl;
            exit(false);
        end;

        if SalesCrMemoHeader.IsEmpty() then begin
            ErrorReason := PostedCreditMemoNotFoundLbl;
            exit(false);
        end;

        // Validate customer
        Element := ExpectedData.ElementExists(CreditMemoCustomerTok, ElementExists);
        if ElementExists then begin
            ExpectedCustomer := Element.ValueAsText();
            SalesCrMemoHeader.SetRange("Sell-to Customer No.", CopyStr(ExpectedCustomer, 1, 20));
            if SalesCrMemoHeader.IsEmpty() then begin
                ErrorReason := StrSubstNo(WrongCustomerLbl, ExpectedCustomer);
                exit(false);
            end;
        end;

        SalesCrMemoHeader.FindLast();

        // Resolve expected UOM (applies to all lines)
        Element := ExpectedData.ElementExists(ExpectedUomTok, HasExpectedUOM);
        if HasExpectedUOM then
            ExpectedUOM := Element.ValueAsText();

        // Validate lines
        LinesData := ExpectedData.ElementExists(CreditMemoLinesTok, LinesExist);
        if LinesExist then begin
            SalesCrMemoLine.SetRange("Document No.", SalesCrMemoHeader."No.");
            SalesCrMemoLine.SetRange(Type, SalesCrMemoLine.Type::Item);

            for I := 0 to LinesData.GetElementCount() - 1 do begin
                Element := LinesData.ElementAt(I).Element(ItemNoTok);
                ExpectedItemNo := Element.ValueAsText();
                ExpectedQuantity := LinesData.ElementAt(I).Element(QuantityTok).ValueAsInteger();

                SalesCrMemoLine.SetRange("No.", CopyStr(ExpectedItemNo, 1, 20));
                if SalesCrMemoLine.IsEmpty() then begin
                    ErrorReason := StrSubstNo(ItemLineNotFoundLbl, ExpectedItemNo);
                    exit(false);
                end;

                SalesCrMemoLine.FindFirst();
                if SalesCrMemoLine.Quantity <> ExpectedQuantity then begin
                    ErrorReason := StrSubstNo(WrongQuantityLbl, ExpectedItemNo, ExpectedQuantity, SalesCrMemoLine.Quantity);
                    exit(false);
                end;

                Element := LinesData.ElementAt(I).ElementExists(UnitPriceTok, ElementExists);
                if ElementExists then
                    if SalesCrMemoLine."Unit Price" <> Element.ValueAsDecimal() then begin
                        ErrorReason := StrSubstNo(WrongUnitPriceLbl, ExpectedItemNo, Element.ValueAsDecimal(), SalesCrMemoLine."Unit Price");
                        exit(false);
                    end;

                Element := LinesData.ElementAt(I).ElementExists(LineAmountTok, ElementExists);
                if ElementExists then
                    if SalesCrMemoLine."Line Amount" <> Element.ValueAsDecimal() then begin
                        ErrorReason := StrSubstNo(WrongLineAmountLbl, ExpectedItemNo, Element.ValueAsDecimal(), SalesCrMemoLine."Line Amount");
                        exit(false);
                    end;

                if HasExpectedUOM then
                    if SalesCrMemoLine."Unit of Measure Code" <> CopyStr(ExpectedUOM, 1, 10) then begin
                        ErrorReason := StrSubstNo(WrongUomLbl, ExpectedUOM, SalesCrMemoLine."Unit of Measure Code", ExpectedItemNo);
                        exit(false);
                    end;
            end;

            // Verify no extra lines beyond what is expected
            SalesCrMemoLine.SetRange("No.");
            if SalesCrMemoLine.Count() <> LinesData.GetElementCount() then begin
                ErrorReason := StrSubstNo(LineCountMismatchLbl, LinesData.GetElementCount(), SalesCrMemoLine.Count());
                exit(false);
            end;
        end;

        // Validate credit memo total
        Element := ExpectedData.ElementExists(CreditMemoTotalTok, ElementExists);
        if ElementExists then begin
            SalesCrMemoHeader.CalcFields(Amount);
            if SalesCrMemoHeader.Amount <> Element.ValueAsDecimal() then begin
                ErrorReason := StrSubstNo(WrongTotalLbl, Element.ValueAsDecimal(), SalesCrMemoHeader.Amount);
                exit(false);
            end;
        end;

        exit(true);
    end;

    /// <summary>
    /// Validates that no credit memo was created (for error scenarios).
    /// </summary>
    procedure ValidateNoCreditMemoCreated(var ErrorReason: Text): Boolean
    var
        SalesHeader: Record "Sales Header";
    begin
        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::"Credit Memo");
        if not SalesHeader.IsEmpty() then begin
            ErrorReason := CreditMemoShouldNotExistLbl;
            exit(false);
        end;
        exit(true);
    end;

    /// <summary>
    /// Validates that the work description contains expected text.
    /// </summary>
    procedure ValidateWorkDescription(var ErrorReason: Text): Boolean
    var
        SalesHeader: Record "Sales Header";
        ExpectedData: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        WorkDescription: Text;
        ExpectedText: Text;
        ElementExists: Boolean;
    begin
        ExpectedData := AITTestContext.GetExpectedData();
        Element := ExpectedData.ElementExists(WorkDescContainsTok, ElementExists);
        if not ElementExists then
            exit(true);

        ExpectedText := Element.ValueAsText();

        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::"Credit Memo");
        if not SalesHeader.FindFirst() then begin
            ErrorReason := CreditMemoNotCreatedLbl;
            exit(false);
        end;

        WorkDescription := SalesHeader.GetWorkDescription();
        if StrPos(WorkDescription, ExpectedText) = 0 then begin
            ErrorReason := StrSubstNo(WorkDescMissingTextLbl, ExpectedText, WorkDescription);
            exit(false);
        end;

        exit(true);
    end;

    /// <summary>
    /// Deletes all sales credit memos and their lines.
    /// </summary>
    procedure DeleteAllSalesCreditMemos()
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
    begin
        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::"Credit Memo");
        SalesHeader.DeleteAll(false);

        SalesLine.SetRange("Document Type", SalesLine."Document Type"::"Credit Memo");
        SalesLine.DeleteAll(false);
    end;

    /// <summary>
    /// Deletes any unposted sales invoices as a general cleanup safety measure.
    /// </summary>
    procedure DeleteAllUnpostedSalesInvoices()
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
    begin
        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::Invoice);
        SalesHeader.DeleteAll(false);

        SalesLine.SetRange("Document Type", SalesLine."Document Type"::Invoice);
        SalesLine.DeleteAll(false);
    end;

    local procedure CreateCustomer(TestInputJson: Codeunit "Test Input Json")
    var
        Customer: Record Customer;
        Element: Codeunit "Test Input Json";
        ElementExists: Boolean;
        CustomerNo: Code[20];
    begin
        Element := TestInputJson.ElementExists('No.', ElementExists);

        if ElementExists then begin
            CustomerNo := CopyStr(Element.ValueAsText(), 1, MaxStrLen(CustomerNo));
            if not Customer.Get(CustomerNo) then begin
                LibrarySales.CreateCustomer(Customer);
                Customer.Rename(CustomerNo);
            end;
        end else
            LibrarySales.CreateCustomer(Customer);

        Element := TestInputJson.ElementExists('Name', ElementExists);
        if ElementExists then
            Customer.Validate(Name, CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer.Name)));

        Element := TestInputJson.ElementExists('Email', ElementExists);
        if ElementExists then
            Customer.Validate("E-Mail", Element.ValueAsText());

        Element := TestInputJson.ElementExists('Address', ElementExists);
        if ElementExists then
            Customer.Validate(Address, CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer.Address)));

        Element := TestInputJson.ElementExists('City', ElementExists);
        if ElementExists then
            Customer.Validate(City, CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer.City)));

        Element := TestInputJson.ElementExists('Post Code', ElementExists);
        if ElementExists then
            Customer.Validate("Post Code", CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer."Post Code")));

        Element := TestInputJson.ElementExists('Country/Region Code', ElementExists);
        if ElementExists then
            Customer.Validate("Country/Region Code", CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer."Country/Region Code")));

        Customer.Modify();
        Commit();
    end;

    local procedure CreateItem(TestInputJson: Codeunit "Test Input Json")
    var
        Item: Record Item;
        Element: Codeunit "Test Input Json";
        ElementExists: Boolean;
        ItemNo: Code[20];
    begin
        Element := TestInputJson.ElementExists('No.', ElementExists);

        if ElementExists then begin
            ItemNo := CopyStr(Element.ValueAsText(), 1, MaxStrLen(ItemNo));
            if not Item.Get(ItemNo) then begin
                LibraryInventory.CreateItem(Item);
                Item.Rename(ItemNo);
            end;
        end else
            LibraryInventory.CreateItem(Item);

        Element := TestInputJson.ElementExists('Description', ElementExists);
        if ElementExists then
            Item.Validate(Description, CopyStr(Element.ValueAsText(), 1, MaxStrLen(Item.Description)));

        Element := TestInputJson.ElementExists('Unit of Measure Code', ElementExists);
        if ElementExists then
            Item.Validate("Base Unit of Measure", CopyStr(Element.ValueAsText(), 1, 10));

        Element := TestInputJson.ElementExists('Unit Price', ElementExists);
        if ElementExists then
            Item.Validate("Unit Price", Element.ValueAsDecimal());

        Item.Modify();
        Commit();
    end;

    local procedure CreatePostedSalesInvoice(TestInputJson: Codeunit "Test Input Json")
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
        LinesData: Codeunit "Test Input Json";
        LineElement: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        ElementExists: Boolean;
        CustomerNo: Code[20];
        ItemNo: Code[20];
        I: Integer;
    begin
        Element := TestInputJson.ElementExists('customer_no', ElementExists);
        if not ElementExists then
            exit;

        CustomerNo := CopyStr(Element.ValueAsText(), 1, MaxStrLen(CustomerNo));
        LibrarySales.CreateSalesHeader(SalesHeader, SalesHeader."Document Type"::Invoice, CustomerNo);

        Element := TestInputJson.ElementExists('date', ElementExists);
        if ElementExists then begin
            SalesHeader.Validate("Posting Date", Element.ValueAsDate());
            SalesHeader.Modify(true);
        end;

        LinesData := TestInputJson.ElementExists('lines', ElementExists);
        if ElementExists then
            for I := 0 to LinesData.GetElementCount() - 1 do begin
                LineElement := LinesData.ElementAt(I);

                ItemNo := CopyStr(LineElement.Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));
                LibrarySales.CreateSalesLine(
                    SalesLine, SalesHeader,
                    Enum::"Sales Line Type"::Item, ItemNo,
                    LineElement.Element('quantity').ValueAsDecimal());
            end;

        LibrarySales.PostSalesDocument(SalesHeader, true, true);
        Commit();
    end;

    local procedure TryGetActionData(TestSetup: Codeunit "Test Input Json"; ActionType: Text; var ActionData: Codeunit "Test Input Json"): Boolean
    var
        ActionsArray: Codeunit "Test Input Json";
        ActionsExist: Boolean;
        I: Integer;
    begin
        ActionsArray := TestSetup.ElementExists(SetupActionsTok, ActionsExist);
        if not ActionsExist then
            exit(false);

        for I := 0 to ActionsArray.GetElementCount() - 1 do
            if ActionsArray.ElementAt(I).Element(ActionTypeTok).ValueAsText() = ActionType then begin
                ActionData := ActionsArray.ElementAt(I).Element(ActionDataTok);
                exit(true);
            end;

        exit(false);
    end;

    var
        AITTestContext: Codeunit "AIT Test Context";
        LibraryInventory: Codeunit "Library - Inventory";
        LibrarySales: Codeunit "Library - Sales";
        CreditMemoNotCreatedLbl: Label 'Expected a credit memo to be created but none was found.';
        CreditMemoShouldNotExistLbl: Label 'Expected no credit memo to be created but one was found.';
        PostedCreditMemoNotFoundLbl: Label 'Expected a posted credit memo but none was found.';
        PostedCreditMemoConflictLbl: Label 'credit_memo_posted is true but credit_memo_created is false — conflicting expected data.';
        WrongCustomerLbl: Label 'Expected credit memo for customer %1 but none was found.', Comment = '%1 = customer number';
        ItemLineNotFoundLbl: Label 'Expected credit memo line for item %1 but none was found.', Comment = '%1 = item number';
        WrongQuantityLbl: Label 'Expected quantity %2 for item %1 but found %3.', Comment = '%1 = item number, %2 = expected quantity, %3 = actual quantity';
        WrongUnitPriceLbl: Label 'Expected unit price %2 for item %1 but found %3.', Comment = '%1 = item number, %2 = expected unit price, %3 = actual unit price';
        WrongLineAmountLbl: Label 'Expected line amount %2 for item %1 but found %3.', Comment = '%1 = item number, %2 = expected line amount, %3 = actual line amount';
        WrongTotalLbl: Label 'Expected credit memo total %1 but found %2.', Comment = '%1 = expected total, %2 = actual total';
        WorkDescMissingTextLbl: Label 'Expected work description to contain "%1" but it did not. Found: "%2"', Comment = '%1 = expected text, %2 = actual text';
        WrongUomLbl: Label 'Expected unit of measure %1 but found %2 for item %3.', Comment = '%1 = expected UOM, %2 = actual UOM, %3 = item number';
        SetupActionsTok: Label 'setup_actions', Locked = true;
        ActionTypeTok: Label 'action_type', Locked = true;
        ActionDataTok: Label 'action_data', Locked = true;
        CreateCustomersTok: Label 'create_customers', Locked = true;
        CreateItemsTok: Label 'create_items', Locked = true;
        CreatePostedShipmentTok: Label 'create_posted_shipment', Locked = true;
        CreatePostedSalesInvoicesTok: Label 'create_posted_sales_invoices', Locked = true;
        CreditMemoCreatedTok: Label 'credit_memo_created', Locked = true;
        CreditMemoCustomerTok: Label 'credit_memo_customer', Locked = true;
        CreditMemoPostedTok: Label 'credit_memo_posted', Locked = true;
        CreditMemoLinesTok: Label 'credit_memo_lines', Locked = true;
        CreditMemoTotalTok: Label 'credit_memo_total', Locked = true;
        LineCountMismatchLbl: Label 'Expected %1 credit memo line(s) but found %2.', Comment = '%1 = expected count, %2 = actual count';
        ItemNoTok: Label 'item_no', Locked = true;
        QuantityTok: Label 'quantity', Locked = true;
        UnitPriceTok: Label 'unit_price', Locked = true;
        LineAmountTok: Label 'line_amount', Locked = true;
        WorkDescContainsTok: Label 'work_description_contains', Locked = true;
        ExpectedUomTok: Label 'expected_uom', Locked = true;
}

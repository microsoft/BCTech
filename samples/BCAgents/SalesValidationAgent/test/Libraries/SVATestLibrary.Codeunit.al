// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace SalesValidationAgent.Test.Libraries;

using Microsoft.Inventory.Item;
using Microsoft.Inventory.Journal;
using Microsoft.Inventory.Location;
using Microsoft.Inventory.Tracking;
using Microsoft.Sales.Customer;
using Microsoft.Sales.Document;
using System.TestLibraries.Utilities;
using System.TestTools.AITestToolkit;
using System.TestTools.TestRunner;

codeunit 53741 "SVA Test Library"
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    /// <summary>
    /// Creates a location from the suite setup data.
    /// Reads the 'create_location' action from the provided test setup.
    /// Returns the location code for use in sales order creation.
    /// </summary>
    /// <param name="TestSetup">The current setup used by the test</param>
    /// <returns>The location code created for use in sales order creation</returns>
    procedure CreateLocation(TestSetup: Codeunit "Test Input Json"): Code[10]
    var
        Location: Record Location;
        ActionData: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        ElementExists: Boolean;
        LocationCode: Code[10];
    begin
        if not TryGetActionData(TestSetup, CreateLocationTok, ActionData) then
            exit(LibraryWarehouse.CreateLocationWithInventoryPostingSetup(Location));

        Element := ActionData.ElementAt(0).ElementExists('Name', ElementExists);
        if ElementExists then begin
            LocationCode := CopyStr(Element.ValueAsText(), 1, MaxStrLen(Location.Code));
            if Location.Get(LocationCode) then
                exit(LocationCode);
        end;

        LibraryWarehouse.CreateLocationWithInventoryPostingSetup(Location);
        Location.Rename(LocationCode);

        exit(Location.Code);
    end;

    /// <summary>
    /// Creates customers from the test setup data.
    /// Reads the 'create_customers' action from the provided test setup.
    /// </summary>
    /// <param name="TestSetup">The current setup used by the test</param>
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
    /// Validates that the expected sales orders were released and non-released for the current turn.
    /// Reads the current turn's expected_data.agent_released_orders as an integer count: if absent or 0, every order must remain Open.
    /// Otherwise, ExpectedReleasedOrders must be Released and ExpectedNonReleasedOrders must be Open.
    /// </summary>
    /// <param name="ExpectedReleasedOrders">Orders that should be released</param>
    /// <param name="ExpectedNonReleasedOrders">Orders that should not be released</param>
    /// <param name="ErrorReason">Text describing why the validation failed, if any</param>
    /// <returns>True if all orders are in the expected release state, false otherwise</returns>
    procedure ValidateSalesOrdersReleased(ExpectedReleasedOrders: List of [Code[20]]; ExpectedNonReleasedOrders: List of [Code[20]]; var ErrorReason: Text): Boolean
    var
        SalesHeader: Record "Sales Header";
        ExpectedData: Codeunit "Test Input Json";
        ReleaseInput: Codeunit "Test Input Json";
        FailedOrders: List of [Code[20]];
        OrderNo: Code[20];
        ErrorTextBuilder: TextBuilder;
        ReleaseExists: Boolean;
        Idx: Integer;
        ExpectedReleaseCount: Integer;
    begin
        ExpectedData := AITTestContext.GetExpectedData();
        ReleaseInput := ExpectedData.ElementExists(AgentReleasedOrdersTok, ReleaseExists);
        if ReleaseExists then
            ExpectedReleaseCount := ReleaseInput.ValueAsInteger();

        if ExpectedReleaseCount > 0 then begin
            for Idx := 1 to ExpectedReleasedOrders.Count() do begin
                OrderNo := ExpectedReleasedOrders.Get(Idx);
                SalesHeader.Get(SalesHeader."Document Type"::Order, OrderNo);
                if SalesHeader.Status <> SalesHeader.Status::Released then
                    FailedOrders.Add(OrderNo);
            end;

            if FailedOrders.Count() > 0 then begin
                ErrorTextBuilder.Append(OrdersExpectedReleasedButNotLbl);
                foreach OrderNo in FailedOrders do
                    ErrorTextBuilder.Append(OrderNo + CommaSeparatorLbl);
                ErrorReason := ErrorTextBuilder.ToText().TrimEnd(CommaSeparatorLbl);
            end;
            Clear(FailedOrders);
        end;

        for Idx := 1 to ExpectedNonReleasedOrders.Count() do begin
            OrderNo := ExpectedNonReleasedOrders.Get(Idx);
            SalesHeader.Get(SalesHeader."Document Type"::Order, OrderNo);
            if SalesHeader.Status <> SalesHeader.Status::Open then
                FailedOrders.Add(OrderNo);
        end;

        if FailedOrders.Count() > 0 then begin
            ErrorTextBuilder.Append(OrdersExpectedOpenButReleasedLbl);
            foreach OrderNo in FailedOrders do
                ErrorTextBuilder.Append(OrderNo + CommaSeparatorLbl);
            ErrorReason := ErrorTextBuilder.ToText().TrimEnd(CommaSeparatorLbl);
        end;

        exit(ErrorReason = '');
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
            if (not Customer.Get(CustomerNo)) then begin
                LibrarySales.CreateCustomer(Customer);
                Customer.Rename(CustomerNo);
            end;
        end else
            LibrarySales.CreateCustomer(Customer);

        Element := TestInputJson.ElementExists('Name', ElementExists);
        if ElementExists then
            Customer.Validate(Name, CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer.Name)));

        Element := TestInputJson.ElementExists('Address', ElementExists);
        if ElementExists then
            Customer.Validate(Address, CopyStr(Element.ValueAsText(), 1, MaxStrLen(Customer.Address)));

        Element := TestInputJson.ElementExists('PhoneNo', ElementExists);
        if ElementExists then
            Customer.Validate("Phone No.", Element.ValueAsText());

        Element := TestInputJson.ElementExists('Email', ElementExists);
        if ElementExists then
            Customer.Validate("E-Mail", Element.ValueAsText());

        Customer.Modify();
        Commit();
    end;

    /// <summary>
    /// Deletes all sales orders and their lines.
    /// </summary>
    procedure DeleteAllSalesOrders()
    var
        SalesHeader: Record "Sales Header";
        SalesLine: Record "Sales Line";
        ReservationEntry: Record "Reservation Entry";
    begin
        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::Order);
        SalesHeader.DeleteAll(false);

        SalesLine.SetRange("Document Type", SalesLine."Document Type"::Order);
        SalesLine.DeleteAll(false);

        ReservationEntry.DeleteAll(false);
    end;

    /// <summary>
    /// Creates sales orders for testing based on the test setup data.
    /// Reads the 'create_sales_order' action and classifies orders by line config (Shipping Advice + Reservation).
    /// Per-turn release expectations are evaluated by ValidateSalesOrdersReleased against expected_data.release.complete.
    /// </summary>
    /// <param name="ExpectedReleasedOrders">Orders that should be released</param>
    /// <param name="ExpectedNonReleasedOrders">Orders that should not be released</param>
    procedure CreateSalesOrderTestData(TestSetup: Codeunit "Test Input Json"; var ExpectedReleasedOrders: List of [Code[20]]; var ExpectedNonReleasedOrders: List of [Code[20]])
    var
        ActionData: Codeunit "Test Input Json";
        SalesOrderCode: Code[20];
        ExpectedReleaseOrder: Boolean;
        I: Integer;
    begin
        Clear(ExpectedReleasedOrders);
        Clear(ExpectedNonReleasedOrders);

        if not TryGetActionData(TestSetup, CreateSalesOrderTok, ActionData) then
            exit;

        for I := 0 to ActionData.GetElementCount() - 1 do begin
            ExpectedReleaseOrder := true;
            SalesOrderCode := CreateSalesOrder(ActionData.ElementAt(I), ExpectedReleaseOrder);
            if ExpectedReleaseOrder then
                ExpectedReleasedOrders.Add(SalesOrderCode)
            else
                ExpectedNonReleasedOrders.Add(SalesOrderCode);
        end;
    end;

    local procedure CreateSalesOrder(SalesOrderInput: Codeunit "Test Input Json"; var ExpectedReleaseOrder: Boolean): Code[20]
    var
        SalesHeader: Record "Sales Header";
        LinesInput: Codeunit "Test Input Json";
        Element: Codeunit "Test Input Json";
        RecRef: RecordRef;
        NoOfLines: Integer;
        IdxLine: Integer;
        ElementExists: Boolean;
    begin
#pragma warning disable AA0139
        LibrarySales.CreateSalesHeader(SalesHeader, SalesHeader."Document Type"::Order, SalesOrderInput.Element(SellToCustomerNoTok).ValueAsText());
#pragma warning restore AA0139
        RecRef.GetTable(SalesHeader);
        SetFieldsFromJson(RecRef, SalesOrderInput);
        RecRef.SetTable(SalesHeader);
        SalesHeader.Modify();

        SalesHeader.Validate("Shipment Date", GetShipmentDate(SalesOrderInput));
        SalesHeader.Modify(true);

        LinesInput := SalesOrderInput.Element(LinesTok);
        NoOfLines := LinesInput.GetElementCount();
        if NoOfLines = 0 then begin
            ExpectedReleaseOrder := false;
            exit(SalesHeader."No.");
        end;

        for IdxLine := 0 to NoOfLines - 1 do
            CreateSalesOrderLine(SalesHeader, LinesInput.ElementAt(IdxLine), ExpectedReleaseOrder);

        Element := SalesOrderInput.ElementExists(ShipmentDateOutOfRangeTok, ElementExists);
        if ElementExists then
            if (Element.ValueAsBoolean() = true) then
                ExpectedReleaseOrder := false;

        exit(SalesHeader."No.");
    end;

    local procedure CreateSalesOrderLine(var SalesHeader: Record "Sales Header"; LineInput: Codeunit "Test Input Json"; var ExpectedReleaseOrder: Boolean)
    var
        Item: Record Item;
        ItemJournalLine: Record "Item Journal Line";
        SalesLine: Record "Sales Line";
        Element: Codeunit "Test Input Json";
        LocationCode: Code[10];
        Quantity: Integer;
        QuantityInInventory: Integer;
        Reserve: Boolean;
        ElementExists: Boolean;
    begin
        LibraryInventory.CreateItem(Item);

        Quantity := LineInput.Element(SalesLine.FieldName(Quantity)).ValueAsInteger();

        Element := LineInput.ElementExists(SalesLine.FieldName("Location Code"), ElementExists);
        if ElementExists then
            LocationCode := CopyStr(Element.ValueAsText(), 1, MaxStrLen(LocationCode));

        Element := LineInput.ElementExists(QuantityInInventoryTok, ElementExists);
        if ElementExists then begin
            QuantityInInventory := Element.ValueAsInteger();

            if QuantityInInventory > 0 then begin
                LibraryInventory.CreateItemJournalLineInItemTemplate(ItemJournalLine, Item."No.", LocationCode, '', QuantityInInventory);
                LibraryInventory.PostItemJournalLine(ItemJournalLine."Journal Template Name", ItemJournalLine."Journal Batch Name");
            end;
        end;

        Element := LineInput.ElementExists(ReserveTok, ElementExists);
        if ElementExists then
            Reserve := Element.ValueAsBoolean();

        LibrarySales.CreateSalesLineWithShipmentDate(SalesLine, SalesHeader, Enum::"Sales Line Type"::Item, Item."No.", SalesHeader."Shipment Date", Quantity);
        SalesLine.Validate("Location Code", LocationCode);
        SalesLine.Modify(true);

        if Reserve then begin
            SalesLine.AutoReserve(false);
#pragma warning disable AA0181
            SalesLine.SetAutoCalcFields("Reserved Quantity");
            SalesLine.Find();
#pragma warning restore AA0181
            Assert.IsTrue(SalesLine."Reserved Quantity" > 0, StrSubstNo(FailedToReserveMsg, Item."No.", SalesHeader."No."));
            case SalesHeader."Shipping Advice" of
                SalesHeader."Shipping Advice"::Complete:
                    ExpectedReleaseOrder := ExpectedReleaseOrder and (SalesLine."Reserved Quantity" = Quantity);
                SalesHeader."Shipping Advice"::Partial:
                    ExpectedReleaseOrder := ExpectedReleaseOrder and (SalesLine."Reserved Quantity" > 0);
                else
                    ExpectedReleaseOrder := false;
            end;
        end else
            ExpectedReleaseOrder := false;
    end;

    local procedure GetShipmentDate(SalesOrderInput: Codeunit "Test Input Json"): Date
    var
        SalesHeader: Record "Sales Header";
        Element: Codeunit "Test Input Json";
        ElementExists: Boolean;
    begin
        Element := SalesOrderInput.ElementExists(SalesHeader.FieldName("Shipment Date"), ElementExists);
        if ElementExists then
            exit(Element.ValueAsDate());
        exit(WorkDate());
    end;

    local procedure SetFieldsFromJson(var RecRef: RecordRef; JsonInput: Codeunit "Test Input Json")
    var
        TestInputDataTools: Codeunit "Test Input Data Tools";
        FldRef: FieldRef;
        JsonObj: JsonObject;
        JsonTok: JsonToken;
        PropertyName: Text;
    begin
        JsonObj := JsonInput.ValueAsJsonObject();
        foreach PropertyName in JsonObj.Keys() do begin
            JsonObj.Get(PropertyName, JsonTok);
            if JsonTok.IsValue() then
                if TryGetFieldByName(RecRef, PropertyName, FldRef) then
                    case FldRef.Type of
                        FieldType::Date:
                            if Format(FldRef.Value()) <> JsonTok.AsValue().AsText() then
                                FldRef.Validate(TestInputDataTools.ResolveAsDate(JsonTok.AsValue().AsText()));
                        FieldType::DateTime:
                            FldRef.Validate(TestInputDataTools.ResolveAsDateTime(JsonTok.AsValue().AsText()));
                        FieldType::Option:
                            Evaluate(FldRef, JsonTok.AsValue().AsText())
                        else
                            FldRef.Validate(TestInputDataTools.ResolveText(JsonTok.AsValue().AsText()));
                    end;
        end;
    end;

    local procedure TryGetFieldByName(RecRef: RecordRef; FieldName: Text; var FldRef: FieldRef): Boolean
    var
        I: Integer;
    begin
        for I := 1 to RecRef.FieldCount() do begin
            FldRef := RecRef.FieldIndex(I);
            if FldRef.Name = FieldName then
                exit(true);
        end;
        exit(false);
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
        Assert: Codeunit "Library Assert";
        LibraryInventory: Codeunit "Library - Inventory";
        LibrarySales: Codeunit "Library - Sales";
        LibraryWarehouse: Codeunit "Library - Warehouse";
        OrdersExpectedOpenButReleasedLbl: Label 'The following sales orders were expected to remain open but were released: ';
        OrdersExpectedReleasedButNotLbl: Label 'The following sales orders were expected to be released but were not: ';
        FailedToReserveMsg: Label 'Failed to reserve the expected quantity for item %1 on sales order %2', Comment = '%1 = item number, %2 = sales order number';
        SetupActionsTok: Label 'setup_actions', Locked = true;
        ActionTypeTok: Label 'action_type', Locked = true;
        ActionDataTok: Label 'action_data', Locked = true;
        CreateSalesOrderTok: Label 'create_sales_order', Locked = true;
        CreateLocationTok: Label 'create_location', Locked = true;
        CreateCustomersTok: Label 'create_customers', Locked = true;
        LinesTok: Label 'lines', Locked = true;
        AgentReleasedOrdersTok: Label 'agent_released_orders', Locked = true;
        QuantityInInventoryTok: Label 'quantity_in_inventory', Locked = true;
        ReserveTok: Label 'reserve', Locked = true;
        SellToCustomerNoTok: Label 'Sell-to Customer No.', Locked = true;
        CommaSeparatorLbl: Label ', ', Locked = true;
        ShipmentDateOutOfRangeTok: Label 'shipment_date_outofrange', Locked = true;
}

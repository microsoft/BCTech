// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

codeunit 51100 "IoT Workflow Setup"
{
    Access = Internal;

    var
        IoTCategory: Label 'IoT', locked = true;
        IoTCategoryDescription: Label 'Azure IoT Central', locked = true;
        AzureIoTCentralMeasurementEventDescription: Label 'An Azure IoT Central measurement is received';
        CreatePurchaseOrderFromIoTWorkflowSetupDescription: Label 'Create Purchase Order from IoT Workflow Setup';

    local procedure RunWorkflowOnAfterInsertAzureIoTCentralMeasurementEventCode(): Text[128]
    begin
        Exit(UpperCase('OnAfterInsertAzureIoTCentralMeasurement'))
    end;

    local procedure CreatePurchaseOrderFromIoTWorkflowSetupResponseCode(): Text[128]
    begin
        Exit(UpperCase('CreatePurchaseOrderFromIoTWorkflowSetup'))
    end;

    local procedure CreatePurchaseOrderFromIoTGroupCode(): Code[20]
    begin
        exit(UpperCase('IoT_CreatePO'))
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Workflow Event Handling", 'OnAddWorkflowEventsToLibrary', '', true, true)]
    local procedure AddIoTEventsOnAddWorkflowEventsToLibrary()
    var
        WorkflowEvent: Record "Workflow Event";
        WorkflowEventHandling: Codeunit "Workflow Event Handling";
    begin
        If WorkflowEvent.Get(RunWorkflowOnAfterInsertAzureIoTCentralMeasurementEventCode()) then begin
            if WorkflowEvent.Description = AzureIoTCentralMeasurementEventDescription then
                exit;
            WorkflowEvent.Description := AzureIoTCentralMeasurementEventDescription;
            WorkflowEvent.Modify();
            exit;
        end;
        WorkflowEvent.SetRange(Description, AzureIoTCentralMeasurementEventDescription);
        If WorkflowEvent.FindSet() then
            WorkflowEvent.DeleteAll();

        WorkflowEventHandling.AddEventToLibrary(RunWorkflowOnAfterInsertAzureIoTCentralMeasurementEventCode(), Database::"Az. IoT Central Measurement", AzureIoTCentralMeasurementEventDescription, 0, false);
    end;


    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Workflow Response Handling", 'OnAddWorkflowResponsesToLibrary', '', true, true)]
    local procedure AddCreatePOResponseOnAddWorkflowResponsesToLibrary()
    var
        WorkflowResponse: Record "Workflow Response";
        WorkflowResponseHandling: Codeunit "Workflow Response Handling";
    begin
        If WorkflowResponse.Get(CreatePurchaseOrderFromIoTWorkflowSetupResponseCode()) then begin
            if WorkflowResponse.Description = CreatePurchaseOrderFromIoTWorkflowSetupDescription then
                exit;
            WorkflowResponse.Description := CreatePurchaseOrderFromIoTWorkflowSetupDescription;
            WorkflowResponse.Modify();
            exit;
        end;
        WorkflowResponse.SetRange(Description, CreatePurchaseOrderFromIoTWorkflowSetupDescription);
        If WorkflowResponse.FindSet() then
            WorkflowResponse.DeleteAll();

        WorkflowResponseHandling.AddResponseToLibrary(CreatePurchaseOrderFromIoTWorkflowSetupResponseCode(), Database::"Az. IoT Central Measurement", CreatePurchaseOrderFromIoTWorkflowSetupDescription, CreatePurchaseOrderFromIoTGroupCode());
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Workflow Response Handling", 'OnAddWorkflowResponsePredecessorsToLibrary', '', true, true)]
    local procedure AddCreatePOFromIoTPredecessorOnAddWorkflowResponsePredecessorsToLibrary()
    var
        WorkflowResponseHandling: Codeunit "Workflow Response Handling";
    begin
        WorkflowResponseHandling.AddResponsePredecessor(CreatePurchaseOrderFromIoTWorkflowSetupResponseCode(), RunWorkflowOnAfterInsertAzureIoTCentralMeasurementEventCode());
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Workflow Setup", 'OnAddWorkflowCategoriesToLibrary', '', true, true)]
    local procedure InsertIoTCategoryOnAddAddWorkflowCategoriesToLibrary()
    var
        WorkflowSetup: Codeunit "Workflow Setup";
    begin
        WorkflowSetup.InsertWorkflowCategory(IoTCategory, IoTCategoryDescription);
    end;

    // TODO create a workflow template

    [EventSubscriber(ObjectType::Table, Database::"Az. IoT Central Measurement", 'OnAfterInsertEvent', '', true, true)]
    local procedure RunWorkflowOnAfterInsertAzureIoTCentralMeasurement(var Rec: Record "Az. IoT Central Measurement");
    var
        WorkflowManagement: Codeunit "Workflow Management";
    Begin
        WorkflowManagement.HandleEvent(RunWorkflowOnAfterInsertAzureIoTCentralMeasurementEventCode(), Rec);
    End;


    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Workflow Response Handling", 'OnExecuteWorkflowResponse', '', true, true)]
    local procedure CreatePOFromIoTOnExecuteWorkflowResponse(ResponseWorkflowStepInstance: Record "Workflow Step Instance"; var ResponseExecuted: Boolean; var Variant: Variant)
    var
        AzIoTCentralMeasurement: Record "Az. IoT Central Measurement";
        IoTDeviceWorkflowSetup: Record "IoT Device Workflow Setup";
        AzureIoTCentralDevice: Record "Azure IoT Central Device";
    begin
        if ResponseExecuted then
            exit;

        if ResponseWorkflowStepInstance."Function Name" <> CreatePurchaseOrderFromIoTWorkflowSetupResponseCode() then
            exit;

        ResponseExecuted := true;

        AzIoTCentralMeasurement := Variant;

        // a little bit of jit setup, not needed but handy
        if not AzureIoTCentralDevice.Get(AzIoTCentralMeasurement."Device Connection ID") then begin
            AzureIoTCentralDevice."Device Id" := AzIoTCentralMeasurement."Device Connection ID";
            AzureIoTCentralDevice."Device Name" := AzIoTCentralMeasurement."Device Name";
            AzureIoTCentralDevice.Insert;
        end else begin
            if AzureIoTCentralDevice."Device Name" = '' then begin
                AzureIoTCentralDevice."Device Name" := AzIoTCentralMeasurement."Device Name";
                AzureIoTCentralDevice.modify;
            end;
        end;

        if not IoTDeviceWorkflowSetup.Get(AzIoTCentralMeasurement."Device ID", AzIoTCentralMeasurement."Rule ID") then begin
            IoTDeviceWorkflowSetup.SetRange("Device Connection ID", AzIoTCentralMeasurement."Device Connection ID");
            if not IoTDeviceWorkflowSetup.FindFirst() then begin
                // add setup record but do not create order

                IoTDeviceWorkflowSetup."Device ID" := AzIoTCentralMeasurement."Device ID";
                IoTDeviceWorkflowSetup."Device Connection ID" := AzIoTCentralMeasurement."Device Connection ID";
                IoTDeviceWorkflowSetup."Rule ID" := AzIoTCentralMeasurement."Rule ID";
                IoTDeviceWorkflowSetup."Rule Name" := AzIoTCentralMeasurement."Rule Name";
                IoTDeviceWorkflowSetup.Insert;
            end;

            exit;
        end;

        if IoTDeviceWorkflowSetup."Item No." = '' then
            exit;
        if IoTDeviceWorkflowSetup."Vendor No." = '' then
            exit;

        CreatePO(IoTDeviceWorkflowSetup."Vendor No.", IoTDeviceWorkflowSetup."Item No.");

        // clean up AzIoTCentralMeasurement record, it is no longer needed (we don't keep a log)
        AzIoTCentralMeasurement.Delete();
    end;

    local procedure CreatePO(VendorNo: Code[20]; ItemNo: Code[20])
    var
        PurchaseHeader: Record "Purchase Header";
        PurchaseLine: Record "Purchase Line";
        Vendor: Record Vendor;
        Item: Record Item;
        ReleasePurchaseDocument: Codeunit "Release Purchase Document";
        ApprovalsMgmt: Codeunit "Approvals Mgmt.";
    begin
        if not Vendor.Get(VendorNo) then
            exit;
        if not Item.Get(ItemNo) then
            exit;

        // create header
        PurchaseHeader.validate("Document Type", PurchaseHeader."Document Type"::Order);
        PurchaseHeader.Insert(true);
        PurchaseHeader.Validate("Buy-from Vendor No.", Vendor."No.");
        PurchaseHeader.Modify(true);

        // add line
        PurchaseLine.Validate("Document Type", PurchaseHeader."Document Type");
        PurchaseLine.Validate("Document No.", PurchaseHeader."No.");
        PurchaseLine."Line No." := 10000;
        PurchaseLine.Validate(Type, PurchaseLine.Type::Item);
        PurchaseLine.Validate("No.", Item."No.");
        PurchaseLine.Validate(Quantity, Item."Reorder Quantity");
        if PurchaseLine.Quantity = 0 then
            PurchaseLine.Validate(Quantity, 1);
        PurchaseLine.Insert(true);

        // release
        ReleasePurchaseDocument.ReleasePurchaseHeader(PurchaseHeader, false);

        // send for approval:
        if ApprovalsMgmt.CheckPurchaseApprovalPossible(PurchaseHeader) then
            ApprovalsMgmt.OnSendPurchaseDocForApproval(PurchaseHeader);
    end;
}
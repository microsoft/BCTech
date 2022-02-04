codeunit 50100 "Integration Subscribers"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Integration Table Synch.", 'OnLoadCRMOption', '', true, true)]
    local procedure HandleOnLoadCRMOption(var TempCRMRecordRef: RecordRef; IntegrationTableMapping: Record "Integration Table Mapping")
    var
        DataverseIndustry: Record "Dataverse Industry";
    begin
        if IntegrationTableMapping."Table ID" = Database::"Industry Group" then begin
            DataverseIndustry.Load();
            TempCRMRecordRef.GetTable(DataverseIndustry);
        end;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Int. Option Synch. Invoke", 'OnPrepareNewDestination', '', true, true)]
    local procedure HandleOnPrepareNewDestination(IntegrationTableMapping: Record "Integration Table Mapping"; var CoupledRecordRef: RecordRef)
    begin
        if IntegrationTableMapping."Table ID" = Database::"Industry Group" then
            CoupledRecordRef.Open(Database::"Dataverse Industry");
    end;

    [EventSubscriber(ObjectType::Table, Database::"CRM Option Mapping", 'OnGetMetadataInfo', '', true, true)]
    local procedure HandleOnGetMetadataInfo(CRMRecordRef: RecordRef; var EntityName: Text; var FieldName: Text)
    var
        TableMetadata: Record "Table Metadata";
    begin
        if CRMRecordRef.Number = Database::"Dataverse Industry" then begin
            if TableMetadata.Get(Database::"CRM Account") then
                EntityName := TableMetadata.ExternalName
            else
                exit;

            FieldName := 'industrycode';
        end;
    end;

    [EventSubscriber(ObjectType::Table, Database::"CRM Option Mapping", 'OnIsCRMRecordRefMapped', '', true, true)]
    local procedure HandledOnIsCRMRecordRefMapped(CRMRecordRef: RecordRef; var CRMOptionMapping: Record "CRM Option Mapping"; var Handled: Boolean)
    var
        CRMAccount: Record "CRM Account";
        DataverseIndustry: Record "Dataverse Industry";
    begin
        if CRMRecordRef.Number = Database::"Dataverse Industry" then begin
            CRMOptionMapping.SetRange("Integration Table ID", Database::"CRM Account");
            CRMOptionMapping.SetRange("Integration Field ID", CRMAccount.FieldNo(IndustryCode));
            CRMOptionMapping.SetRange("Option Value", CRMRecordRef.Field(DataverseIndustry.FieldNo("Option Id")).Value());
            Handled := true;
        end;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Integration Management", 'OnIsCRMTable', '', true, true)]
    local procedure HandleOnIsCRMTable(TableID: Integer; var CRMTable: Boolean; var Handled: Boolean)
    begin
        if TableID = Database::"Dataverse Industry" then begin
            CRMTable := true;
            Handled := true;
        end;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Integration Management", 'OnGetTableIdFromCRMOption', '', true, true)]
    local procedure HandleOnGetTableIdFromCRMOption(RecRef: RecordRef; var TableId: Integer)
    begin
        if RecRef.Number = Database::"Dataverse Industry" then
            TableId := Database::"Industry Group";
    end;

    [EventSubscriber(ObjectType::Table, Database::"Coupling Record Buffer", 'OnFindCRMOptionByName', '', true, true)]
    local procedure HandleOnFindCRMOptionByName(CRMTableID: Integer; var EntityName: Text; var FieldName: Text; var Handled: Boolean)
    var
        TableMetadata: Record "Table Metadata";
    begin
        if CRMTableID = Database::"Dataverse Industry" then begin
            if TableMetadata.Get(Database::"CRM Account") then
                EntityName := TableMetadata.ExternalName
            else
                exit;

            FieldName := 'industrycode';
            Handled := true;
        end;

    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Setup Defaults", 'OnGetCDSTableNo', '', false, false)]
    local procedure HandleOnGetCDSTableNo(BCTableNo: Integer; var CDSTableNo: Integer; var handled: Boolean)
    begin
        if BCTableNo = Database::"Industry Group" then begin
            CDSTableNo := Database::"Dataverse Industry";
            handled := true;
        end;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Lookup CRM Tables", 'OnLookupCRMOption', '', true, true)]
    local procedure HandleOnLookupCRMTables(CRMTableID: Integer; NAVTableId: Integer; SavedCRMOptionId: Integer; var CRMOptionCode: Text[250]; var CRMOptionId: Integer; IntTableFilter: Text; var Handled: Boolean)
    begin
        if CRMTableID = Database::"Dataverse Industry" then
            Handled := LookupDataveseIndustry(SavedCRMOptionId, CRMOptionId, CRMOptionCode, IntTableFilter);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CDS Setup Defaults", 'OnAfterResetConfiguration', '', true, true)]
    local procedure HandleOnAfterResetConfiguration(CDSConnectionSetup: Record "CDS Connection Setup")
    var
        IntegrationTableMapping: Record "Integration Table Mapping";
        IntegrationFieldMapping: Record "Integration Field Mapping";
        CRMAccount: Record "CRM Account";
        IndustryGroup: Record "Industry Group";
    begin
        InsertIntegrationTableMapping(
          IntegrationTableMapping, 'INDUSTRY',
          Database::"Industry Group", Database::"CRM Account",
          CRMAccount.FieldNo(IndustryCode), 0,
          '', '', true);

        InsertIntegrationFieldMapping('INDUSTRY', IndustryGroup.FieldNo(Code), CRMAccount.FieldNo(IndustryCode), IntegrationFieldMapping.Direction::FromIntegrationTable, '', true, false);
    end;

    local procedure LookupDataveseIndustry(SavedCRMId: Integer; var CRMOptionId: Integer; var CRMOptionCode: Text[250]; IntTableFilter: Text): Boolean
    var
        DataverseIndustry: Record "Dataverse Industry";
        OriginalDataverseIndustry: Record "Dataverse Industry";
        DataverseIndustryList: Page "Dataverse Industry List";
    begin
        if CRMOptionId <> 0 then begin
            DataverseIndustryList.LoadRecords();
            DataverseIndustry := DataverseIndustryList.GetRec(CRMOptionId);
            if DataverseIndustry."Option Id" <> 0 then
                DataverseIndustryList.SetRecord(DataverseIndustry);
            if SavedCRMId <> 0 then begin
                OriginalDataverseIndustry := DataverseIndustryList.GetRec(SavedCRMId);
                if OriginalDataverseIndustry."Option Id" <> 0 then
                    DataverseIndustryList.SetCurrentlyMappedDataverseIndustryOptionId(SavedCRMId);
            end;
        end;
        DataverseIndustry.SetView(IntTableFilter);
        DataverseIndustryList.SetTableView(DataverseIndustry);
        DataverseIndustryList.LookupMode(true);
        Commit();
        if DataverseIndustryList.RunModal() = ACTION::LookupOK then begin
            DataverseIndustryList.GetRecord(DataverseIndustry);
            CRMOptionId := DataverseIndustry."Option Id";
            CRMOptionCode := DataverseIndustry."Code";
            exit(true);
        end;
        exit(false);
    end;

    local procedure InsertIntegrationTableMapping(var IntegrationTableMapping: Record "Integration Table Mapping"; MappingName: Code[20]; TableNo: Integer; IntegrationTableNo: Integer; IntegrationTableUIDFieldNo: Integer; IntegrationTableModifiedFieldNo: Integer; TableConfigTemplateCode: Code[10]; IntegrationTableConfigTemplateCode: Code[10]; SynchOnlyCoupledRecords: Boolean)
    begin
        IntegrationTableMapping.CreateRecord(MappingName, TableNo, IntegrationTableNo, IntegrationTableUIDFieldNo, IntegrationTableModifiedFieldNo, TableConfigTemplateCode, IntegrationTableConfigTemplateCode, SynchOnlyCoupledRecords, IntegrationTableMapping.Direction::ToIntegrationTable, 'CDS');
    end;

    local procedure InsertIntegrationFieldMapping(IntegrationTableMappingName: Code[20]; TableFieldNo: Integer; IntegrationTableFieldNo: Integer; SynchDirection: Option; ConstValue: Text; ValidateField: Boolean; ValidateIntegrationTableField: Boolean)
    var
        IntegrationFieldMapping: Record "Integration Field Mapping";
    begin
        IntegrationFieldMapping.CreateRecord(IntegrationTableMappingName, TableFieldNo, IntegrationTableFieldNo, SynchDirection,
            ConstValue, ValidateField, ValidateIntegrationTableField);
    end;
}
codeunit 50127 "Rbld. Dataverse Cpl. Tbl."
{
    Subtype = Install;

    trigger OnInstallAppPerCompany()
    var
        CRMIntegrationRecord: Record "CRM Integration Record";
        IntegrationRecord: Record "Integration Record";
        RecRef: RecordRef;
        CRMIntegrationRecordCorrectionDictionary: Dictionary of [Guid, Guid];
        SysIdAfterMigration: Guid;
        CRMIntegrationRecordSysId: Guid;
        CommitCounter: Integer;
    begin
        // collect all CRM Integration Record records that need to be corrected
        if CRMIntegrationRecord.FindSet() then
            repeat
                if IntegrationRecord.Get(CRMIntegrationRecord."Integration ID") then
                    if RecRef.Get(IntegrationRecord."Record ID") then begin
                        SysIdAfterMigration := RecRef.Field(RecRef.SystemIdNo()).Value();
                        if CRMIntegrationRecord."Integration ID" <> SysIdAfterMigration then
                            CRMIntegrationRecordCorrectionDictionary.Add(CRMIntegrationRecord.SystemId, SysIdAfterMigration);
                    end;
            until CRMIntegrationRecord.Next() = 0;

        // loop through the correction dictionary and rename the CRM Integration Record records with new values
        foreach CRMIntegrationRecordSysId in CRMIntegrationRecordCorrectionDictionary.Keys() do begin
            CommitCounter += 1;
            CRMIntegrationRecordCorrectionDictionary.Get(CRMIntegrationRecordSysId, SysIdAfterMigration);
            CRMIntegrationRecord.GetBySystemId(CRMIntegrationRecordSysId);
            CRMIntegrationRecord.Rename(CRMIntegrationRecord."CRM ID", SysIdAfterMigration);
            if CRMIntegrationRecord."Table ID" = 0 then
                GetTableID(CRMIntegrationRecord);
            if CommitCounter = 1000 then begin
                Commit();
                CommitCounter := 0;
            end;
        end;
    end;

    local procedure GetTableID(var CRMIntegrationRecord: Record "CRM Integration Record"): Integer
    begin
        if CRMIntegrationRecord."Table ID" <> 0 then
            exit(CRMIntegrationRecord."Table ID");

        if IsNullGuid(CRMIntegrationRecord."Integration ID") then
            exit(0);

        if RepairTableIdByLocalRecord(CRMIntegrationRecord) then
            exit(CRMIntegrationRecord."Table ID");

        exit(0);
    end;

    local procedure RepairTableIdByLocalRecord(var CRMIntegrationRecord: Record "CRM Integration Record"): Boolean
    var
        IntegrationTableMapping: Record "Integration Table Mapping";
    begin
        if CRMIntegrationRecord."Table ID" <> 0 then
            exit(true);

        if IsNullGuid(CRMIntegrationRecord."Integration ID") then
            exit(true);

        if FindMappingByLocalRecordId(CRMIntegrationRecord, IntegrationTableMapping) then begin
            CRMIntegrationRecord."Table ID" := IntegrationTableMapping."Table ID";
            CRMIntegrationRecord.Modify();
            exit(true);
        end;

        exit(false);
    end;

    local procedure FindMappingByLocalRecordId(var CRMIntegrationRecord: Record "CRM Integration Record"; var IntegrationTableMapping: Record "Integration Table Mapping"): Boolean
    var
        LocalRecordRef: RecordRef;
    begin
        IntegrationTableMapping.SetRange("Delete After Synchronization", false);
        IntegrationTableMapping.SetFilter("Table ID", '<>0');
        if IntegrationTableMapping.FindSet() then
            repeat
                LocalRecordRef.Close();
                LocalRecordRef.Open(IntegrationTableMapping."Table ID");
                if LocalRecordRef.GetBySystemId(CRMIntegrationRecord."Integration ID") then
                    exit(true);
            until IntegrationTableMapping.Next() = 0;
        exit(false);
    end;

}
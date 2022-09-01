codeunit 50101 "Dataverse Int. Management"
{
    var
        IntegrationTablePrefixTok: Label 'Dataverse', Comment = 'Product name', Locked = true;
        JobQueueEntryNameTok: Label ' %1 - %2 synchronization job.', Comment = '%1 = The Integration Table Name to synchronized (ex. CUSTOMER), %2 = CRM product name';

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Setup Defaults", 'OnGetCDSTableNo', '', false, false)]
    local procedure HandleOnGetCDSTableNo(BCTableNo: Integer; var CDSTableNo: Integer; var handled: Boolean)
    begin
        if BCTableNo = Database::Employee then begin
            CDSTableNo := Database::"Dataverse cdm_worker";
            handled := true;
        end;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Lookup CRM Tables", 'OnLookupCRMTables', '', false, false)]
    local procedure HandleOnLookupCRMTables(CRMTableID: Integer; NAVTableId: Integer; SavedCRMId: Guid; var CRMId: Guid; IntTableFilter: Text; var Handled: Boolean)
    begin
        if CRMTableID = Database::"Dataverse cdm_worker" then
            Handled := LookupDataverseWorker(SavedCRMId, CRMId, IntTableFilter);
    end;

    local procedure LookupDataverseWorker(SavedCRMId: Guid; var CRMId: Guid; IntTableFilter: Text): Boolean
    var
        DataverseWorker: Record "Dataverse cdm_worker";
        OriginalDataverseWorker: Record "Dataverse cdm_worker";
        DataverseWorkerList: Page "Dataverse Worker List";
    begin
        if not IsNullGuid(CRMId) then begin
            if DataverseWorker.Get(CRMId) then
                DataverseWorkerList.SetRecord(DataverseWorker);
            if not IsNullGuid(SavedCRMId) then
                if OriginalDataverseWorker.Get(SavedCRMId) then
                    DataverseWorkerList.SetCurrentlyCoupledDataverseWorker(OriginalDataverseWorker);
        end;

        DataverseWorker.SetView(IntTableFilter);
        DataverseWorkerList.SetTableView(DataverseWorker);
        DataverseWorkerList.LookupMode(true);
        if DataverseWorkerList.RunModal = Action::LookupOK then begin
            DataverseWorkerList.GetRecord(DataverseWorker);
            CRMId := DataverseWorker.cdm_workerId;
            exit(true);
        end;
        exit(false);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Setup Defaults", 'OnAddEntityTableMapping', '', false, false)]
    local procedure HandleOnAddEntityTableMapping(var TempNameValueBuffer: Record "Name/Value Buffer" temporary);
    var
        CRMSetupDefaults: Codeunit "CRM Setup Defaults";
    begin
        CRMSetupDefaults.AddEntityTableMapping('cdm_worker', Database::Employee, TempNameValueBuffer);
        CRMSetupDefaults.AddEntityTableMapping('cdm_worker', Database::"Dataverse cdm_worker", TempNameValueBuffer);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CDS Setup Defaults", 'OnAfterResetConfiguration', '', false, false)]
    local procedure HandleOnAfterResetConfiguration(CDSConnectionSetup: Record "CDS Connection Setup")
    begin
        AddEmployeeWorkerMapping('EMPLOYEE-WORKER', true);
    end;

    local procedure AddEmployeeWorkerMapping(IntegrationTableMappingName: Code[20]; ShouldRecreateJobQueueEntry: Boolean)
    var
        IntegrationTableMapping: Record "Integration Table Mapping";
        IntegrationFieldMapping: Record "Integration Field Mapping";
        DataverseWorker: Record "Dataverse cdm_worker";
        Employee: Record Employee;
    begin
        InsertIntegrationTableMapping(
          IntegrationTableMapping, IntegrationTableMappingName,
          Database::Employee, Database::"Dataverse cdm_worker",
          DataverseWorker.FieldNo(cdm_workerId), DataverseWorker.FieldNo(ModifiedOn),
          '', '', true);

        DataverseWorker.Reset();
        DataverseWorker.SetRange(cdm_Type, DataverseWorker.cdm_Type::Employee);
        IntegrationTableMapping.SetIntegrationTableFilter(
          GetTableFilterFromView(Database::"Dataverse cdm_worker", DataverseWorker.TableCaption(), DataverseWorker.GetView()));
        IntegrationTableMapping.Modify();

        // Birthday
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Birth Date"), DataverseWorker.FieldNo(cdm_Birthdate), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Email
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("E-Mail"), DataverseWorker.FieldNo(cdm_PrimaryEmailAddress), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // First name
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("First Name"), DataverseWorker.FieldNo(cdm_FirstName), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Middle name
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Middle Name"), DataverseWorker.FieldNo(cdm_MiddleName), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Last name
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Last Name"), DataverseWorker.FieldNo(cdm_LastName), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Gender
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo(Gender), DataverseWorker.FieldNo(cdm_Gender), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Mobile phone
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Mobile Phone No."), DataverseWorker.FieldNo(cdm_MobilePhone), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Primary phone
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Phone No."), DataverseWorker.FieldNo(cdm_PrimaryTelephone), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Job title - profession
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Job Title"), DataverseWorker.FieldNo(cdm_Profession), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);

        RecreateJobQueueEntryFromIntTableMapping(IntegrationTableMapping, 30, ShouldRecreateJobQueueEntry, 720);
    end;

    local procedure GetTableFilterFromView(TableID: Integer; Caption: Text; View: Text): Text
    var
        FilterBuilder: FilterPageBuilder;
    begin
        FilterBuilder.AddTable(Caption, TableID);
        FilterBuilder.SetView(Caption, View);
        exit(FilterBuilder.GetView(Caption, true));
    end;

    local procedure InsertIntegrationFieldMapping(IntegrationTableMappingName: Code[20]; TableFieldNo: Integer; IntegrationTableFieldNo: Integer; SynchDirection: Option; ConstValue: Text; ValidateField: Boolean; ValidateIntegrationTableField: Boolean)
    var
        IntegrationFieldMapping: Record "Integration Field Mapping";
    begin
        IntegrationFieldMapping.CreateRecord(IntegrationTableMappingName, TableFieldNo, IntegrationTableFieldNo, SynchDirection,
          ConstValue, ValidateField, ValidateIntegrationTableField);
    end;

    local procedure InsertIntegrationTableMapping(var IntegrationTableMapping: Record "Integration Table Mapping"; MappingName: Code[20]; TableNo: Integer; IntegrationTableNo: Integer; IntegrationTableUIDFieldNo: Integer; IntegrationTableModifiedFieldNo: Integer; TableConfigTemplateCode: Code[10]; IntegrationTableConfigTemplateCode: Code[10]; SynchOnlyCoupledRecords: Boolean)
    begin
        IntegrationTableMapping.CreateRecord(MappingName, TableNo, IntegrationTableNo, IntegrationTableUIDFieldNo,
          IntegrationTableModifiedFieldNo, TableConfigTemplateCode, IntegrationTableConfigTemplateCode,
          SynchOnlyCoupledRecords, IntegrationTableMapping.Direction::Bidirectional, IntegrationTablePrefixTok);
    end;

    local procedure RecreateJobQueueEntryFromIntTableMapping(IntegrationTableMapping: Record "Integration Table Mapping"; IntervalInMinutes: Integer; ShouldRecreateJobQueueEntry: Boolean; InactivityTimeoutPeriod: Integer)
    var
        JobQueueEntry: Record "Job Queue Entry";
    begin
        JobQueueEntry.SetRange("Object Type to Run", JobQueueEntry."Object Type to Run"::Codeunit);
        JobQueueEntry.SetRange("Object ID to Run", Codeunit::"Integration Synch. Job Runner");
        JobQueueEntry.SetRange("Record ID to Process", IntegrationTableMapping.RecordId);
        JobQueueEntry.DeleteTasks();

        JobQueueEntry.InitRecurringJob(IntervalInMinutes);
        JobQueueEntry."Object Type to Run" := JobQueueEntry."Object Type to Run"::Codeunit;
        JobQueueEntry."Object ID to Run" := Codeunit::"Integration Synch. Job Runner";
        JobQueueEntry."Record ID to Process" := IntegrationTableMapping.RecordId;
        JobQueueEntry."Run in User Session" := false;
        JobQueueEntry.Description :=
          CopyStr(StrSubstNo(JobQueueEntryNameTok, IntegrationTableMapping.Name, 'Dataverse'), 1, MaxStrLen(JobQueueEntry.Description));
        JobQueueEntry."Maximum No. of Attempts to Run" := 10;
        JobQueueEntry.Status := JobQueueEntry.Status::Ready;
        JobQueueEntry."Rerun Delay (sec.)" := 30;
        JobQueueEntry."Inactivity Timeout Period" := InactivityTimeoutPeriod;
        if ShouldRecreateJobQueueEntry then
            Codeunit.Run(Codeunit::"Job Queue - Enqueue", JobQueueEntry)
        else
            JobQueueEntry.Insert(true);
    end;
}
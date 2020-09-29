codeunit 50101 "CDS Integration Management"
{
    var
        IntegrationTablePrefixTok: Label 'Dynamics CRM', Comment = 'Product name', Locked = true;
        JobQueueEntryNameTok: Label ' %1 - %2 synchronization job.', Comment = '%1 = The Integration Table Name to synchronized (ex. CUSTOMER), %2 = CRM product name';

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Setup Defaults", 'OnGetCDSTableNo', '', false, false)]
    local procedure HandleOnGetCDSTableNo(BCTableNo: Integer; var CDSTableNo: Integer; var handled: Boolean)
    begin
        if BCTableNo = Database::Employee then begin
            CDSTableNo := Database::"CDS cdm_worker";
            handled := true;
        end;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Integration Management", 'OnIsIntegrationRecord', '', true, true)]
    local procedure HandleOnIsIntegrationRecord(TableID: Integer; var isIntegrationRecord: Boolean)
    begin
        if TableID = Database::Employee then
            isIntegrationRecord := true;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Integration Management", 'OnAfterAddToIntegrationPageList', '', true, true)]
    local procedure HandleOnAfterAddToIntegrationPageList(var TempNameValueBuffer: Record "Name/Value Buffer"; var NextId: Integer)
    begin
        TempNameValueBuffer.Init();
        TempNameValueBuffer.ID := NextId;
        NextId := NextId + 1;
        TempNameValueBuffer.Name := Format(Page::"Employee Card");
        TempNameValueBuffer.Value := Format(Database::"Employee");
        TempNameValueBuffer.Insert();
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Lookup CRM Tables", 'OnLookupCRMTables', '', true, true)]
    local procedure HandleOnLookupCRMTables(CRMTableID: Integer; NAVTableId: Integer; SavedCRMId: Guid; var CRMId: Guid; IntTableFilter: Text; var Handled: Boolean)
    begin
        if CRMTableID = Database::"CDS cdm_worker" then
            Handled := LookupCDSWorker(SavedCRMId, CRMId, IntTableFilter);
    end;

    local procedure LookupCDSWorker(SavedCRMId: Guid; var CRMId: Guid; IntTableFilter: Text): Boolean
    var
        CDSWorker: Record "CDS cdm_worker";
        OriginalCDSWorker: Record "CDS cdm_worker";
        CDSWorkerList: Page "CDS Worker List";
    begin
        if not IsNullGuid(CRMId) then begin
            if CDSWorker.Get(CRMId) then
                CDSWorkerList.SetRecord(CDSWorker);
            if not IsNullGuid(SavedCRMId) then
                if OriginalCDSWorker.Get(SavedCRMId) then
                    CDSWorkerList.SetCurrentlyCoupledCDSWorker(OriginalCDSWorker);
        end;

        CDSWorker.SetView(IntTableFilter);
        CDSWorkerList.SetTableView(CDSWorker);
        CDSWorkerList.LookupMode(true);
        if CDSWorkerList.RunModal = Action::LookupOK then begin
            CDSWorkerList.GetRecord(CDSWorker);
            CRMId := CDSWorker.cdm_workerId;
            exit(true);
        end;
        exit(false);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Integration Rec. Synch. Invoke", 'OnBeforeInsertRecord', '', false, false)]
    local procedure HandleOnBeforeInsertRecord(SourceRecordRef: RecordRef; DestinationRecordRef: RecordRef)
    var
        CDSIntegrationMgt: Codeunit "CDS Integration Mgt.";
    begin
        if DestinationRecordRef.Number() = Database::"CDS cdm_worker" then
            CDSIntegrationMgt.SetCompanyId(DestinationRecordRef);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"CRM Setup Defaults", 'OnAfterResetConfiguration', '', true, true)]
    local procedure HandleOnAfterResetConfiguration(CRMConnectionSetup: Record "CRM Connection Setup")
    begin
        AddEmployeeWorkerMapping('EMPLOYEE-WORKER', true);
    end;

    local procedure AddEmployeeWorkerMapping(IntegrationTableMappingName: Code[20]; ShouldRecreateJobQueueEntry: Boolean)
    var
        IntegrationTableMapping: Record "Integration Table Mapping";
        IntegrationFieldMapping: Record "Integration Field Mapping";
        CDSWorker: Record "CDS cdm_worker";
        Employee: Record Employee;
    begin
        InsertIntegrationTableMapping(
          IntegrationTableMapping, IntegrationTableMappingName,
          Database::Employee, Database::"CDS cdm_worker",
          CDSWorker.FieldNo(cdm_workerId), CDSWorker.FieldNo(ModifiedOn),
          '', '', true);

        CDSWorker.Reset();
        CDSWorker.SetRange(cdm_Type, CDSWorker.cdm_Type::Employee);
        IntegrationTableMapping.SetIntegrationTableFilter(
          GetTableFilterFromView(Database::"CDS cdm_worker", CDSWorker.TableCaption(), CDSWorker.GetView()));
        IntegrationTableMapping.Modify();

        // Birthday
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Birth Date"), CDSWorker.FieldNo(cdm_Birthdate), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Email
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("E-Mail"), CDSWorker.FieldNo(cdm_PrimaryEmailAddress), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // First name
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("First Name"), CDSWorker.FieldNo(cdm_FirstName), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Middle name
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Middle Name"), CDSWorker.FieldNo(cdm_MiddleName), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Last name
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Last Name"), CDSWorker.FieldNo(cdm_LastName), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Gender
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo(Gender), CDSWorker.FieldNo(cdm_Gender), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Mobile phone
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Mobile Phone No."), CDSWorker.FieldNo(cdm_MobilePhone), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Primary phone
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Phone No."), CDSWorker.FieldNo(cdm_PrimaryTelephone), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);
        // Job title - profession
        InsertIntegrationFieldMapping(IntegrationTableMappingName, Employee.FieldNo("Job Title"), CDSWorker.FieldNo(cdm_Profession), IntegrationFieldMapping.Direction::Bidirectional, '', true, false);

        RecreateJobQueueEntryFromIntTableMapping(IntegrationTableMapping, 2, ShouldRecreateJobQueueEntry, 720);
    end;

    procedure GetTableFilterFromView(TableID: Integer; Caption: Text; View: Text): Text
    var
        FilterBuilder: FilterPageBuilder;
    begin
        FilterBuilder.AddTable(Caption, TableID);
        FilterBuilder.SetView(Caption, View);
        exit(FilterBuilder.GetView(Caption, true));
    end;

    procedure InsertIntegrationFieldMapping(IntegrationTableMappingName: Code[20]; TableFieldNo: Integer; IntegrationTableFieldNo: Integer; SynchDirection: Option; ConstValue: Text; ValidateField: Boolean; ValidateIntegrationTableField: Boolean)
    var
        IntegrationFieldMapping: Record "Integration Field Mapping";
    begin
        IntegrationFieldMapping.CreateRecord(IntegrationTableMappingName, TableFieldNo, IntegrationTableFieldNo, SynchDirection,
          ConstValue, ValidateField, ValidateIntegrationTableField);
    end;

    procedure InsertIntegrationTableMapping(var IntegrationTableMapping: Record "Integration Table Mapping"; MappingName: Code[20]; TableNo: Integer; IntegrationTableNo: Integer; IntegrationTableUIDFieldNo: Integer; IntegrationTableModifiedFieldNo: Integer; TableConfigTemplateCode: Code[10]; IntegrationTableConfigTemplateCode: Code[10]; SynchOnlyCoupledRecords: Boolean)
    begin
        IntegrationTableMapping.CreateRecord(MappingName, TableNo, IntegrationTableNo, IntegrationTableUIDFieldNo,
          IntegrationTableModifiedFieldNo, TableConfigTemplateCode, IntegrationTableConfigTemplateCode,
          SynchOnlyCoupledRecords, IntegrationTableMapping.Direction::Bidirectional, IntegrationTablePrefixTok);
    end;

    procedure RecreateJobQueueEntryFromIntTableMapping(IntegrationTableMapping: Record "Integration Table Mapping"; IntervalInMinutes: Integer; ShouldRecreateJobQueueEntry: Boolean; InactivityTimeoutPeriod: Integer)
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
          CopyStr(StrSubstNo(JobQueueEntryNameTok, IntegrationTableMapping.Name, 'Common Data Service'), 1, MaxStrLen(JobQueueEntry.Description));
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
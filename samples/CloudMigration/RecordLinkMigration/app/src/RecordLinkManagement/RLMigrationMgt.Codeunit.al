// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using Microsoft.DataMigration;
using System.Environment.Configuration;
using System.Security.AccessControl;

codeunit 57501 "RL Migration Mgt."
{
    InherentEntitlements = X;
    InherentPermissions = X;

    var
        RecordLinkMigrationTelemetryCategoryTok: Label 'RecordLinkMigration', Locked = true;
        MigrationStartedLbl: Label 'Record link migration started for company %1.', Locked = true;
        MigrationCompletedLbl: Label 'Record link migration completed for company %1. Migrated: %2, Errors: %3.', Locked = true;
        MigrationProgressMsg: Label 'Migrating record links for #1...\#2 of #3 records processed.';

    trigger OnRun()
    begin
    end;

    procedure TransferBufferToRecordLinks()
    var
        HybridCompany: Record "Hybrid Company";
    begin
        HybridCompany.SetRange(Replicate, true);
        if HybridCompany.FindSet() then
            repeat
                TransferForCompany(HybridCompany.Name);
            until HybridCompany.Next() = 0;

        // Process records with blank company (per-database links)
        TransferForCompany('');
    end;

    local procedure TransferForCompany(CompanyName: Text[30])
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationMapping: Record "RL Migration Mapping";
        RLMigrationProgress: Record "RL Migration Progress";
        RecordLink: Record "Record Link";
        ProgressDialog: Dialog;
        LinkId: Integer;
        MigratedCount: Integer;
        ErrorCount: Integer;
        TotalCount: Integer;
        BatchCounter: Integer;
    begin
        InitializeProgress(CompanyName, RLMigrationProgress);

        RLMigrationBuffer.SetRange(Company, CompanyName);
        if RLMigrationProgress."Last Processed Link ID" > 0 then
            RLMigrationBuffer.SetFilter("Link ID", '>%1', RLMigrationProgress."Last Processed Link ID");

        TotalCount := RLMigrationBuffer.Count();
        if TotalCount = 0 then
            exit;

        Session.LogMessage('0000RL2', StrSubstNo(MigrationStartedLbl, CompanyName), Verbosity::Normal, DataClassification::SystemMetadata, TelemetryScope::ExtensionPublisher, 'Category', RecordLinkMigrationTelemetryCategoryTok);

        RLMigrationProgress.Status := RLMigrationProgress.Status::"In Progress";
        RLMigrationProgress."Total Records" := TotalCount + RLMigrationProgress."Migrated Records";
        RLMigrationProgress.Modify();

        // Get next available Link ID
        RecordLink.SetLoadFields("Link ID");
        if RecordLink.FindLast() then
            LinkId := RecordLink."Link ID" + 1
        else
            LinkId := 1;

        ProgressDialog.Open(MigrationProgressMsg, CompanyName, MigratedCount, TotalCount);
        BatchCounter := 0;

        RLMigrationBuffer.FindSet();
        repeat
            // Check if already migrated via mapping table
            RLMigrationMapping.SetRange("Source Link ID", RLMigrationBuffer."Link ID");
            RLMigrationMapping.SetRange(Company, RLMigrationBuffer.Company);
            if not RLMigrationMapping.FindFirst() then begin
                if CreateRecordLink(RLMigrationBuffer, LinkId) then begin
                    InsertMapping(RLMigrationBuffer, LinkId - 1);
                    MigratedCount += 1;
                end else
                    ErrorCount += 1;
            end else
                MigratedCount += 1;

            BatchCounter += 1;
            if BatchCounter >= GetBatchSize() then begin
                UpdateProgress(RLMigrationProgress, MigratedCount, ErrorCount, RLMigrationBuffer."Link ID");
                Commit();
                BatchCounter := 0;
                ProgressDialog.Update();
            end;
        until RLMigrationBuffer.Next() = 0;

        // Final commit
        UpdateProgress(RLMigrationProgress, MigratedCount, ErrorCount, RLMigrationBuffer."Link ID");
        RLMigrationProgress.Status := RLMigrationProgress.Status::Completed;
        RLMigrationProgress."Last Migration DateTime" := CurrentDateTime();
        RLMigrationProgress.Modify();
        Commit();

        ProgressDialog.Close();

        Session.LogMessage('0000RL3', StrSubstNo(MigrationCompletedLbl, CompanyName, MigratedCount, ErrorCount), Verbosity::Normal, DataClassification::SystemMetadata, TelemetryScope::ExtensionPublisher, 'Category', RecordLinkMigrationTelemetryCategoryTok);
    end;

    local procedure CreateRecordLink(var RLMigrationBuffer: Record "RL Migration Buffer"; var LinkId: Integer): Boolean
    var
        RecordLink: Record "Record Link";
    begin
        RLMigrationBuffer.CalcFields(Note);

        RecordLink."Link ID" := LinkId;
        RecordLink."Record ID" := RLMigrationBuffer."Record ID";
        RecordLink.URL1 := RLMigrationBuffer.URL1;
        RecordLink.Description := RLMigrationBuffer.Description;
        RecordLink.Type := RLMigrationBuffer.Type;
        RecordLink.Note := RLMigrationBuffer.Note;
        RecordLink.Created := RLMigrationBuffer.Created;
        RecordLink."User ID" := RLMigrationBuffer."User ID";
        RecordLink.Company := RLMigrationBuffer.Company;
        RecordLink.Notify := RLMigrationBuffer.Notify;
        RecordLink."To User ID" := RLMigrationBuffer."To User ID";

        if RecordLink.Insert() then begin
            LinkId += 1;
            exit(true);
        end;

        exit(false);
    end;

    local procedure InsertMapping(var RLMigrationBuffer: Record "RL Migration Buffer"; TargetLinkId: Integer)
    var
        RLMigrationMapping: Record "RL Migration Mapping";
    begin
        RLMigrationMapping."Source Link ID" := RLMigrationBuffer."Link ID";
        RLMigrationMapping."Target Link ID" := TargetLinkId;
        RLMigrationMapping.Company := RLMigrationBuffer.Company;
        RLMigrationMapping."Record ID" := RLMigrationBuffer."Record ID";
        RLMigrationMapping.Type := RLMigrationBuffer.Type;
        RLMigrationMapping.Description := RLMigrationBuffer.Description;
        RLMigrationMapping."Note Prefix" := GetNotePrefix(RLMigrationBuffer);
        RLMigrationMapping.Insert();
    end;

    local procedure GetNotePrefix(var RLMigrationBuffer: Record "RL Migration Buffer"): Text[2048]
    var
        NoteInStream: InStream;
        NoteText: Text;
    begin
        if RLMigrationBuffer.Type <> RLMigrationBuffer.Type::Note then
            exit('');

        RLMigrationBuffer.CalcFields(Note);
        if not RLMigrationBuffer.Note.HasValue() then
            exit('');

        RLMigrationBuffer.Note.CreateInStream(NoteInStream);
        NoteInStream.ReadText(NoteText);
        exit(CopyStr(NoteText, 1, 2048));
    end;

    local procedure InitializeProgress(CompanyName: Text[30]; var RLMigrationProgress: Record "RL Migration Progress")
    begin
        if not RLMigrationProgress.Get(CompanyName) then begin
            RLMigrationProgress.Init();
            RLMigrationProgress."Company Name" := CompanyName;
            RLMigrationProgress.Status := RLMigrationProgress.Status::"Not Started";
            RLMigrationProgress.Insert();
        end;
    end;

    local procedure UpdateProgress(var RLMigrationProgress: Record "RL Migration Progress"; MigratedCount: Integer; ErrorCount: Integer; LastLinkId: Integer)
    begin
        RLMigrationProgress."Migrated Records" := MigratedCount;
        RLMigrationProgress."Error Count" := ErrorCount;
        RLMigrationProgress."Last Processed Link ID" := LastLinkId;
        RLMigrationProgress.Modify();
    end;

    local procedure GetBatchSize(): Integer
    begin
        exit(1000);
    end;

    procedure ApplyUserMapping()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        DestUserText: Text[132];
    begin
        RLMigrationUserMapping.SetRange("Is Mapped", true);
        if not RLMigrationUserMapping.FindSet() then
            exit;

        repeat
            DestUserText := RLMigrationUserMapping."Dest User Name";

            // Map User ID
            RLMigrationBuffer.Reset();
            RLMigrationBuffer.SetFilter("User ID", '=%1', RLMigrationUserMapping."Source User Name");
            if RLMigrationBuffer.FindSet() then
                repeat
                    RLMigrationBuffer."User ID" := DestUserText;
                    RLMigrationBuffer.Modify();
                until RLMigrationBuffer.Next() = 0;

            // Map To User ID
            RLMigrationBuffer.Reset();
            RLMigrationBuffer.SetFilter("To User ID", '=%1', RLMigrationUserMapping."Source User Name");
            if RLMigrationBuffer.FindSet() then
                repeat
                    RLMigrationBuffer."To User ID" := DestUserText;
                    RLMigrationBuffer.Modify();
                until RLMigrationBuffer.Next() = 0;
        until RLMigrationUserMapping.Next() = 0;
    end;

    procedure PopulateUserMappings(ConfirmOverwrite: Boolean)
    var
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationBuffer: Record "RL Migration Buffer";
        UserMappingSource: Record "User Mapping Source";
        User: Record User;
        DistinctUsers: List of [Text];
    begin
        if not RLMigrationUserMapping.IsEmpty() then
            if ConfirmOverwrite then begin
                if not Confirm(UserMappingExistsQst) then
                    exit;
            end else
                exit;

        // Copy from existing cloud migration user mapping
        if UserMappingSource.FindSet() then
            repeat
                if not RLMigrationUserMapping.Get(UserMappingSource."User ID") then begin
                    RLMigrationUserMapping.Init();
                    RLMigrationUserMapping."Source User Name" := UserMappingSource."User ID";
                    if TryAutoMatchUser(UserMappingSource."Authentication Email", RLMigrationUserMapping."Dest User Name") then
                        RLMigrationUserMapping."Is Mapped" := true;
                    RLMigrationUserMapping.Insert();
                end;
            until UserMappingSource.Next() = 0;

        // Then, populate distinct User IDs from buffer that are not yet mapped
        RLMigrationBuffer.SetFilter("User ID", '<>%1', '');
        if RLMigrationBuffer.FindSet() then
            repeat
                if not DistinctUsers.Contains(RLMigrationBuffer."User ID") then begin
                    DistinctUsers.Add(RLMigrationBuffer."User ID");
                    if not RLMigrationUserMapping.Get(RLMigrationBuffer."User ID") then begin
                        RLMigrationUserMapping.Init();
                        RLMigrationUserMapping."Source User Name" := CopyStr(RLMigrationBuffer."User ID", 1, MaxStrLen(RLMigrationUserMapping."Source User Name"));
                        RLMigrationUserMapping.Insert();
                    end;
                end;
            until RLMigrationBuffer.Next() = 0;

        // Also populate distinct To User IDs
        RLMigrationBuffer.Reset();
        RLMigrationBuffer.SetFilter("To User ID", '<>%1', '');
        if RLMigrationBuffer.FindSet() then
            repeat
                if not DistinctUsers.Contains(RLMigrationBuffer."To User ID") then begin
                    DistinctUsers.Add(RLMigrationBuffer."To User ID");
                    if not RLMigrationUserMapping.Get(RLMigrationBuffer."To User ID") then begin
                        RLMigrationUserMapping.Init();
                        RLMigrationUserMapping."Source User Name" := CopyStr(RLMigrationBuffer."To User ID", 1, MaxStrLen(RLMigrationUserMapping."Source User Name"));
                        RLMigrationUserMapping.Insert();
                    end;
                end;
            until RLMigrationBuffer.Next() = 0;
    end;

    local procedure TryAutoMatchUser(AuthEmail: Text[50]; var DestUserId: Code[50]): Boolean
    var
        User: Record User;
    begin
        if AuthEmail <> '' then begin
            User.SetRange("Authentication Email", AuthEmail);
            if User.FindFirst() then begin
                DestUserId := User."User Name";
                exit(true);
            end;
        end;

        exit(false);
    end;

    procedure DeleteBuffer()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
    begin
        RLMigrationBuffer.DeleteAll();
    end;

    procedure RenameUsersInRecordLinks(): Integer
    var
        RLMigrationUserMapping: Record "RL Migration User Mapping";
        RLMigrationMapping: Record "RL Migration Mapping";
        RecordLink: Record "Record Link";
        RenamedCount: Integer;
    begin
        RLMigrationUserMapping.SetRange("Is Mapped", true);
        if not RLMigrationUserMapping.FindSet() then
            exit(0);

        repeat
            // Rename User ID
            RecordLink.Reset();
            RecordLink.SetRange("User ID", RLMigrationUserMapping."Source User Name");
            if RecordLink.FindSet() then
                repeat
                    RecordLink."User ID" := RLMigrationUserMapping."Dest User Name";
                    RecordLink.Modify();
                    RenamedCount += 1;
                until RecordLink.Next() = 0;

            // Rename To User ID
            RecordLink.Reset();
            RecordLink.SetRange("To User ID", RLMigrationUserMapping."Source User Name");
            if RecordLink.FindSet() then
                repeat
                    RecordLink."To User ID" := RLMigrationUserMapping."Dest User Name";
                    RecordLink.Modify();
                    RenamedCount += 1;
                until RecordLink.Next() = 0;
        until RLMigrationUserMapping.Next() = 0;

        exit(RenamedCount);
    end;

    procedure ResetProgress()
    var
        RLMigrationProgress: Record "RL Migration Progress";
    begin
        RLMigrationProgress.DeleteAll();
    end;

    procedure ClearUserMappings()
    var
        RLMigrationUserMapping: Record "RL Migration User Mapping";
    begin
        RLMigrationUserMapping.DeleteAll();
    end;

    var
        UserMappingExistsQst: Label 'User mapping data already exists. Do you want to add new entries? Existing mappings will be preserved.';
}

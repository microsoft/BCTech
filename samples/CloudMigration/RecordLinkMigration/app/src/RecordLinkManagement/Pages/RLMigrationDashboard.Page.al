// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

page 57500 "RL Migration Dashboard"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "RL Migration Progress";
    Caption = 'Record Link Migration';
    Editable = false;
    InsertAllowed = false;
    DeleteAllowed = false;

    layout
    {
        area(Content)
        {
            group(BufferInfo)
            {
                Caption = 'Buffer Summary';
                field(TotalBufferRecords; TotalBufferRecords)
                {
                    ApplicationArea = All;
                    Caption = 'Total Records in Buffer';
                    ToolTip = 'Specifies the total number of record links and notes in the migration buffer.';
                    Editable = false;
                }
            }
            repeater(Companies)
            {
                field("Company Name"; Rec."Company Name")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the company name.';
                }
                field(Status; Rec.Status)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the migration status for this company.';
                }
                field("Total Records"; Rec."Total Records")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the total number of record links to migrate for this company.';
                }
                field("Migrated Records"; Rec."Migrated Records")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the number of record links already migrated.';
                }
                field("Error Count"; Rec."Error Count")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the number of errors encountered during migration.';
                }
                field("Last Migration DateTime"; Rec."Last Migration DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies when the last migration was performed for this company.';
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(TransferToRecordLink)
            {
                ApplicationArea = All;
                Caption = 'Transfer to Record Link';
                ToolTip = 'Transfers buffered record links and notes to the system Record Link table for all replicated companies.';
                Image = TransferOrder;

                trigger OnAction()
                var
                    RLMigrationMgt: Codeunit "RL Migration Mgt.";
                begin
                    RLMigrationMgt.TransferBufferToRecordLinks();
                    CurrPage.Update(false);
                    Message(TransferCompletedMsg);
                end;
            }
            action(DefineUserMappings)
            {
                ApplicationArea = All;
                Caption = 'Define User Mappings';
                ToolTip = 'Define how on-premises users map to SaaS users for the User ID and To User ID fields.';
                Image = UserSetup;
                RunObject = page "RL Migration User Mapping";
            }
            action(IdentifyDuplicates)
            {
                ApplicationArea = All;
                Caption = 'Identify Duplicates';
                ToolTip = 'Scans the buffer for record links that already exist in the target system.';
                Image = Find;

                trigger OnAction()
                var
                    RLMigrationDuplicateFinder: Codeunit "RL Migration Duplicate Finder";
                begin
                    RLMigrationDuplicateFinder.IdentifyDuplicates();
                    Page.Run(Page::"RL Migration Duplicates");
                end;
            }
            action(RenameUsersInRecordLink)
            {
                ApplicationArea = All;
                Caption = 'Rename Users in Record Link';
                ToolTip = 'Renames User ID and To User ID in already-migrated system Record Links using the defined user mapping.';
                Image = UserSetup;

                trigger OnAction()
                var
                    RLMigrationMgt: Codeunit "RL Migration Mgt.";
                    RenamedCount: Integer;
                begin
                    if not Confirm(RenameUsersConfirmMsg) then
                        exit;

                    RenamedCount := RLMigrationMgt.RenameUsersInRecordLinks();
                    Message(RenameUsersCompletedMsg, RenamedCount);
                end;
            }
            action(DeleteBuffer)
            {
                ApplicationArea = All;
                Caption = 'Delete Buffer';
                ToolTip = 'Deletes all records from the migration buffer. Warning: this may cause duplicates on future migration runs.';
                Image = Delete;

                trigger OnAction()
                var
                    RLMigrationMgt: Codeunit "RL Migration Mgt.";
                begin
                    if not Confirm(DeleteBufferWarningMsg) then
                        exit;

                    RLMigrationMgt.DeleteBuffer();
                    CurrPage.Update(false);
                    Message(BufferDeletedMsg);
                end;
            }
        }
        area(Promoted)
        {
            group(Category_Process)
            {
                actionref(TransferToRecordLink_Promoted; TransferToRecordLink) { }
                actionref(DefineUserMappings_Promoted; DefineUserMappings) { }
                actionref(IdentifyDuplicates_Promoted; IdentifyDuplicates) { }
            }
        }
    }

    trigger OnOpenPage()
    begin
        CalculateBufferStats();
        PopulateProgressFromBuffer();
    end;

    local procedure CalculateBufferStats()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
    begin
        TotalBufferRecords := RLMigrationBuffer.Count();
    end;

    local procedure PopulateProgressFromBuffer()
    var
        RLMigrationBuffer: Record "RL Migration Buffer";
        RLMigrationProgress: Record "RL Migration Progress";
        CompanyList: List of [Text];
        CompanyName: Text;
        BufferCount: Integer;
    begin
        // Collect distinct companies from buffer and ensure progress records exist
        if not RLMigrationBuffer.FindSet() then
            exit;

        repeat
            if not CompanyList.Contains(RLMigrationBuffer.Company) then
                CompanyList.Add(RLMigrationBuffer.Company);
        until RLMigrationBuffer.Next() = 0;

        foreach CompanyName in CompanyList do begin
            if not RLMigrationProgress.Get(CopyStr(CompanyName, 1, 30)) then begin
                RLMigrationProgress.Init();
                RLMigrationProgress."Company Name" := CopyStr(CompanyName, 1, 30);
                RLMigrationProgress.Status := RLMigrationProgress.Status::"Not Started";
                RLMigrationBuffer.Reset();
                RLMigrationBuffer.SetRange(Company, CopyStr(CompanyName, 1, 30));
                RLMigrationProgress."Total Records" := RLMigrationBuffer.Count();
                RLMigrationProgress.Insert();
            end;
        end;
    end;

    var
        TotalBufferRecords: Integer;
        TransferCompletedMsg: Label 'Transfer to Record Link completed. Check the progress details below.';
        RenameUsersConfirmMsg: Label 'This will rename User ID and To User ID in all migrated system Record Links using the defined user mapping. Continue?';
        RenameUsersCompletedMsg: Label 'User rename completed. %1 record(s) updated.', Comment = '%1 = Number of renamed records';
        DeleteBufferWarningMsg: Label 'Deleting the buffer will remove the ability to detect duplicates on future migration runs. Are you sure you want to continue?';
        BufferDeletedMsg: Label 'The migration buffer has been deleted.';
}

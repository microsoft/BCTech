// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

page 57502 "RL Migration Duplicates"
{
    PageType = List;
    ApplicationArea = All;
    SourceTable = "RL Migration Mapping";
    SourceTableView = where("Is Duplicate" = const(true));
    Caption = 'Record Link Migration - Duplicates';
    InsertAllowed = false;
    DeleteAllowed = false;

    layout
    {
        area(Content)
        {
            group(Instructions)
            {
                ShowCaption = false;
                InstructionalText = 'These buffer records match existing record links in the system. Choose an action for each: Skip (do not migrate) or Overwrite (update the existing record link).';
            }
            repeater(Duplicates)
            {
                field("Source Link ID"; Rec."Source Link ID")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the link ID from the source (buffer).';
                }
                field(Company; Rec.Company)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the company this record link belongs to.';
                }
                field("Record ID"; Rec."Record ID")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the record that this link points to.';
                }
                field(Type; Rec.Type)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies whether this is a Link or a Note.';
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the description of the record link.';
                }
                field("Target Link ID"; Rec."Target Link ID")
                {
                    ApplicationArea = All;
                    Caption = 'Existing Link ID';
                    ToolTip = 'Specifies the ID of the existing record link that matches this buffer record.';
                }
                field("Duplicate Action"; Rec."Duplicate Action")
                {
                    ApplicationArea = All;
                    ToolTip = 'Choose Skip to not migrate this record, or Overwrite to update the existing record link.';
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(SkipAll)
            {
                ApplicationArea = All;
                Caption = 'Skip All';
                ToolTip = 'Mark all duplicates to be skipped during migration.';
                Image = Cancel;

                trigger OnAction()
                var
                    RLMigrationMapping: Record "RL Migration Mapping";
                begin
                    RLMigrationMapping.SetRange("Is Duplicate", true);
                    RLMigrationMapping.ModifyAll("Duplicate Action", RLMigrationMapping."Duplicate Action"::Skip);
                    CurrPage.Update(false);
                end;
            }
            action(OverwriteAll)
            {
                ApplicationArea = All;
                Caption = 'Overwrite All';
                ToolTip = 'Mark all duplicates to overwrite the existing record links during migration.';
                Image = Apply;

                trigger OnAction()
                var
                    RLMigrationMapping: Record "RL Migration Mapping";
                begin
                    RLMigrationMapping.SetRange("Is Duplicate", true);
                    RLMigrationMapping.ModifyAll("Duplicate Action", RLMigrationMapping."Duplicate Action"::Overwrite);
                    CurrPage.Update(false);
                end;
            }
        }
        area(Promoted)
        {
            group(Category_Process)
            {
                actionref(SkipAll_Promoted; SkipAll) { }
                actionref(OverwriteAll_Promoted; OverwriteAll) { }
            }
        }
    }
}

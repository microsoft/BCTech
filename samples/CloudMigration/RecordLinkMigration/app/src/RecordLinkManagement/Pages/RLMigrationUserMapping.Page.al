// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace MSFT.DataMigration;

using System.Security.AccessControl;
using System.Security.User;

page 57501 "RL Migration User Mapping"
{
    PageType = List;
    ApplicationArea = All;
    SourceTable = "RL Migration User Mapping";
    Caption = 'Record Link Migration - User Mapping';
    InsertAllowed = true;
    DeleteAllowed = true;
    ModifyAllowed = true;
    MultipleNewLines = false;

    layout
    {
        area(Content)
        {
            group(Instructions)
            {
                ShowCaption = false;
                InstructionalText = 'Map on-premises usernames to their corresponding SaaS usernames. Click "Populate Users" to auto-detect users from the buffer, then "Apply Mapping" to update the buffer with the new usernames.';
            }
            repeater(UserMappings)
            {
                field("Source User ID"; Rec."Source User Name")
                {
                    ApplicationArea = All;
                    Caption = 'On-Premises User';
                    ToolTip = 'Specifies the on-premises username (e.g., DOMAIN\user).';
                }
                field("Dest User ID"; Rec."Dest User Name")
                {
                    ApplicationArea = All;
                    Caption = 'SaaS User';
                    ToolTip = 'Specifies the SaaS username to map to (e.g., user@contoso.com).';

                    trigger OnLookup(var Text: Text): Boolean
                    var
                        User: Record User;
                    begin
                        if Page.RunModal(Page::Users, User) = Action::LookupOK then begin
                            Rec."Dest User Name" := User."User Name";
                            Rec."Is Mapped" := true;
                            Rec.Modify();
                            Text := User."User Name";
                            exit(true);
                        end;
                        exit(false);
                    end;

                    trigger OnValidate()
                    begin
                        Rec."Is Mapped" := Rec."Dest User Name" <> '';
                        Rec.Modify();
                    end;
                }
                field("Is Mapped"; Rec."Is Mapped")
                {
                    ApplicationArea = All;
                    ToolTip = 'Indicates whether this user has been mapped to a SaaS user.';
                    Editable = false;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(PopulateUsers)
            {
                ApplicationArea = All;
                Caption = 'Populate Users';
                ToolTip = 'Auto-detects distinct users from the migration buffer and pre-populates the mapping list. Also copies from existing cloud migration user mappings.';
                Image = UserSetup;

                trigger OnAction()
                var
                    RLMigrationMgt: Codeunit "RL Migration Mgt.";
                begin
                    RLMigrationMgt.PopulateUserMappings(true);
                    CurrPage.Update(false);
                    Message(UsersPopulatedMsg);
                end;
            }
            action(ApplyMapping)
            {
                ApplicationArea = All;
                Caption = 'Apply Mapping';
                ToolTip = 'Applies the user mapping to the buffer table, replacing on-premises usernames with the SaaS usernames in User ID and To User ID fields.';
                Image = Apply;

                trigger OnAction()
                var
                    RLMigrationMgt: Codeunit "RL Migration Mgt.";
                begin
                    if not Confirm(ApplyMappingConfirmMsg) then
                        exit;

                    RLMigrationMgt.ApplyUserMapping();
                    Message(MappingAppliedMsg);
                end;
            }
            action(ClearMappings)
            {
                ApplicationArea = All;
                Caption = 'Clear All Mappings';
                ToolTip = 'Removes all user mapping entries so you can start fresh.';
                Image = Delete;

                trigger OnAction()
                var
                    RLMigrationMgt: Codeunit "RL Migration Mgt.";
                begin
                    if not Confirm(ClearMappingsConfirmMsg) then
                        exit;

                    RLMigrationMgt.ClearUserMappings();
                    CurrPage.Update(false);
                end;
            }
        }
        area(Promoted)
        {
            group(Category_Process)
            {
                actionref(PopulateUsers_Promoted; PopulateUsers) { }
                actionref(ApplyMapping_Promoted; ApplyMapping) { }
                actionref(ClearMappings_Promoted; ClearMappings) { }
            }
        }
    }

    var
        UsersPopulatedMsg: Label 'User list has been populated from the buffer and existing mappings.';
        ApplyMappingConfirmMsg: Label 'This will update all User ID and To User ID values in the migration buffer to use the mapped SaaS usernames. Continue?';
        MappingAppliedMsg: Label 'User mapping has been applied to the migration buffer.';
        ClearMappingsConfirmMsg: Label 'This will remove all user mapping entries. Are you sure?';
}

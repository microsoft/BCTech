page 73926 "APIV2 - Aut. Security Groups"
{
    APIPublisher = 'escaperoom';
    APIGroup = 'automation';
    APIVersion = 'v1.0';
    EntityCaption = 'Security Group';
    EntitySetCaption = 'Security Groups';
    DelayedInsert = true;
    EntityName = 'securityGroup';
    EntitySetName = 'securityGroups';
    PageType = API;
    SourceTable = "Security Group Buffer";
    Extensible = false;
    SourceTableTemporary = true;
    ODataKeyFields = "Group ID";
    ModifyAllowed = false;

    layout
    {
        area(content)
        {
            repeater(Group)
            {
                field(id; Rec."Group ID")
                {
                    Caption = 'Id';
                }
                field("code"; Rec.Code)
                {
                    Caption = 'Code';
                }
                field(groupName; Rec."Group Name")
                {
                    Caption = 'Group Name';
                }
            }
        }
    }

    trigger OnFindRecord(Which: Text): Boolean
    begin
        if not AreRecordsLoaded then begin
            LoadRecords();
            AreRecordsLoaded := true;
            if Rec.IsEmpty() then
                exit(false);
        end;

        exit(true);
    end;

    trigger OnOpenPage()
    begin
        BindSubscription(AutomationAPIManagement);
    end;

    trigger OnInsertRecord(BelowxRec: Boolean): Boolean
    var
        EmptyGuid: Guid;
    begin
        SecurityGroup.Create(Rec.Code, Rec."Group ID");
        SecurityGroup.AddPermissionSet(Rec.Code, 'SUPER', '', 0, EmptyGuid);
    end;

    trigger OnDeleteRecord(): Boolean
    begin
        SecurityGroup.Delete(Rec.Code);
    end;

    local procedure LoadRecords()
    begin
        SecurityGroup.GetGroups(Rec);
    end;

    var
        SecurityGroup: Codeunit "Security Group";
        AutomationAPIManagement: Codeunit "Automation - API Management";
        AreRecordsLoaded: Boolean;
}
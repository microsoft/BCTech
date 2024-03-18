
page 50115 "IsEntitled Tester Card"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Entitlement List";
    SourceTableTemporary = true;

    layout
    {
        area(Content)
        {
            repeater(List)
            {
                field("Name/Id"; rec."Name/Id") { }
                field("Owning AppId"; rec."Owning AppId") { }
                field("Is Entitled"; rec."Is Entitled") { Editable = false; Style = Favorable; StyleExpr = Rec."Is Entitled"; }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ActionName)
            {
                ApplicationArea = All;

                Caption = 'Test entitlements';

                trigger OnAction()
                begin
                    if (rec.FindSet()) then
                        repeat
                            rec."Is Entitled" := NavApp.IsEntitled(rec."Name/Id", rec."Owning AppId");
                            rec.Modify();
                        until (rec.Next() = 0);
                end;
            }
        }
    }


    trigger OnOpenPage()
    var
        SystemAppIdTok: Label '63ca2fa4-4f03-4f2b-a480-172fef340d3f', Locked = true;
    begin
        clear(Rec);
        rec.DeleteAll();

        AddRow('Dynamics 365 Business Central for IWs', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Team Member - Embedded', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Team Member', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Premium Partner Sandbox', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Premium - Embedded', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Premium', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central External Accountant', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Essentials', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Essential - Embedded', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Essential - Attach', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Device - Embedded', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Device', SystemAppIdTok);
        AddRow('Dynamics 365 Business Central Basic Financials', SystemAppIdTok);
        AddRow('Dynamics 365 Administrator', SystemAppIdTok);
        AddRow('Dynamics 365 Admin - Partner', SystemAppIdTok);
        AddRow('Delegated Helpdesk agent - Partner', SystemAppIdTok);
        AddRow('Delegated BC Admin agent - Partner', SystemAppIdTok);
        AddRow('Delegated Admin agent - Partner', SystemAppIdTok);
        AddRow('Azure AD Application Automation', SystemAppIdTok);
        AddRow('Azure AD Application Api', SystemAppIdTok);
        AddRow('Internal Administrator', SystemAppIdTok);
        AddRow('Microsoft 365', SystemAppIdTok);
    end;

    local procedure AddRow(NameId: Text[200]; OwningAppId: Guid)
    begin
        rec.Init();
        rec."Name/Id" := NameId;
        rec."Owning AppId" := OwningAppId;
        rec."Is Entitled" := false;
        rec.Insert();
    end;
}
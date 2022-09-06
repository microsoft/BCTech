page 50100 "Dataverse Worker List"
{
    ApplicationArea = All;
    Caption = 'Dataverse Worker';
    Editable = false;
    PageType = List;
    RefreshOnActivate = true;
    SourceTable = "Dataverse cdm_worker";
    UsageCategory = Lists;

    layout
    {
        area(content)
        {
            repeater(Control1)
            {
                field("First Name"; Rec.cdm_FirstName)
                {
                    ApplicationArea = All;
                    Caption = 'First Name';
                }
                field("Last Name"; Rec.cdm_LastName)
                {
                    ApplicationArea = All;
                    Caption = 'Last Name';
                }
                field("Birth date"; Rec.cdm_Birthdate)
                {
                    ApplicationArea = All;
                    Caption = 'Birth Date';
                }
                field("Phone"; Rec.cdm_PrimaryTelephone)
                {
                    ApplicationArea = All;
                    Caption = 'Phone';
                }
                field("E-mail"; Rec.cdm_PrimaryEmailAddress)
                {
                    ApplicationArea = All;
                    Caption = 'E-mail';
                }
                field("Mobile Phone"; Rec.cdm_MobilePhone)
                {
                    ApplicationArea = All;
                    Caption = 'Mobile Phone';
                }
                field("Profession"; Rec.cdm_Profession)
                {
                    ApplicationArea = All;
                    Caption = 'Profession';
                }
                field("Type"; Rec.cdm_Type)
                {
                    ApplicationArea = All;
                    Caption = 'Type';
                }
            }
        }
    }

    actions
    {
        area(processing)
        {
            action(CreateFromDataverse)
            {
                ApplicationArea = All;
                Caption = 'Create in Business Central';
                Promoted = true;
                PromotedCategory = Process;
                ToolTip = 'Generate the entity from the coupled Dataverse worker.';

                trigger OnAction()
                var
                    DataverseWorker: Record "Dataverse cdm_worker";
                    CRMIntegrationManagement: Codeunit "CRM Integration Management";
                begin
                    CurrPage.SetSelectionFilter(DataverseWorker);
                    CRMIntegrationManagement.CreateNewRecordsFromCRM(DataverseWorker);
                end;
            }
        }
    }

    var
        CurrentlyCoupledDataverseWorker: Record "Dataverse cdm_worker";

    trigger OnInit()
    begin
        Codeunit.Run(Codeunit::"CRM Integration Management");
    end;

    procedure SetCurrentlyCoupledDataverseWorker(DataverseWorker: Record "Dataverse cdm_worker")
    begin
        CurrentlyCoupledDataverseWorker := DataverseWorker;
    end;
}
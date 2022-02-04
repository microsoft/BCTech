page 50100 "Dataverse Industry List"
{
    ApplicationArea = Suite;
    Caption = 'Industry - Dataverse';
    AdditionalSearchTerms = 'Industry CDS, Industry Common Data Service';
    Editable = false;
    PageType = List;
    SourceTable = "Dataverse Industry";
    SourceTableView = sorting("Code");
    SourceTableTemporary = true;
    UsageCategory = Lists;

    layout
    {
        area(content)
        {
            repeater(Control2)
            {
                ShowCaption = false;
                field("Code"; Rec."Code")
                {
                    ApplicationArea = Suite;
                    Caption = 'Code';
                    StyleExpr = FirstColumnStyle;
                    ToolTip = 'Specifies data from a corresponding field in a Dataverse entity. For more information about Dataverse, see Dataverse Help Center.';
                }
            }
        }
    }

    actions
    {
        area(processing)
        {
            action(CreateFromCRM)
            {
                ApplicationArea = Suite;
                Caption = 'Create in Business Central';
                Image = NewCustomer;
                Promoted = true;
                PromotedCategory = Process;
                ToolTip = 'Generate the entity from the coupled Dataverse Industry.';
                Visible = OptionMappingEnabled;

                trigger OnAction()
                var
                    DataverseIndustry: Record "Dataverse Industry";
                    CRMIntegrationManagement: Codeunit "CRM Integration Management";
                begin
                    DataverseIndustry.Copy(Rec, true);
                    CurrPage.SetSelectionFilter(DataverseIndustry);
                    CRMIntegrationManagement.CreateNewRecordsFromSelectedCRMOptions(DataverseIndustry);
                end;
            }
        }
    }

    trigger OnInit()
    begin
        Codeunit.Run(Codeunit::"CRM Integration Management");
    end;

    trigger OnOpenPage()
    var
        CRMIntegrationManagmeent: Codeunit "CRM Integration Management";
    begin
        OptionMappingEnabled := CRMIntegrationManagmeent.IsOptionMappingEnabled();
        LoadRecords();
    end;

    var
        CurrentlyMappedDataverseIndustryOptionId: Integer;
        Coupled: Text;
        FirstColumnStyle: Text;
        LinesLoaded: Boolean;
        OptionMappingEnabled: Boolean;

    procedure SetCurrentlyMappedDataverseIndustryOptionId(OptionId: Integer)
    begin
        CurrentlyMappedDataverseIndustryOptionId := OptionId;
    end;

    procedure GetRec(OptionId: Integer): Record "Dataverse Industry"
    begin
        if Rec.Get(OptionId) then
            exit(Rec);
    end;

    procedure LoadRecords()
    begin
        if LinesLoaded then
            exit;

        LinesLoaded := Rec.Load();
    end;
}
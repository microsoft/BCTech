page 54390 "Suggest Resource Subpart"
{
    PageType = ListPart;
    Extensible = false;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Resource Proposal";
    SourceTableTemporary = true;

    layout
    {
        area(Content)
        {
            repeater(ResourceDetails)
            {
                ShowCaption = false;
                field("No."; Rec."No.")
                {
                    ApplicationArea = All;
                }
                field(Name; Rec.Name)
                {
                    ApplicationArea = All;
                }
                field("Job Title"; Rec."Job Title")
                {
                    ApplicationArea = All;
                }
                field(Explanation; Rec.Explanation)
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    procedure Load(var TempCopilotResourceProposal: Record "Resource Proposal" temporary)
    begin
        Rec.Reset();
        Rec.DeleteAll();

        TempCopilotResourceProposal.Reset();
        if TempCopilotResourceProposal.FindSet() then
            repeat
                Rec.Copy(TempCopilotResourceProposal, false);
                Rec.Insert();
            until TempCopilotResourceProposal.Next() = 0;

        CurrPage.Update(false);
    end;
}
namespace CopilotToolkitDemo.DescribeJob;
using CopilotToolkitDemo.SuggestResource;

page 54328 "Copilot Res. Proposal Subpart"
{
    PageType = ListPart;
    Extensible = false;
    ApplicationArea = All;
    UsageCategory = Administration;
    Caption = 'Dynamics 365 Copilot Jobs';
    SourceTable = "Copilot Resource Proposal";
    SourceTableTemporary = true;

    layout
    {
        area(Content)
        {
            repeater(ResourceDetails)
            {
                Caption = ' ';
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

                    trigger OnAssistEdit()
                    var
                        InStr: InStream;
                        FullExplanation: Text;
                    begin
                        Rec.CalcFields("Full Explanation");
                        Rec."Full Explanation".CreateInStream(InStr);
                        InStr.ReadText(FullExplanation);
                        Message(FullExplanation);
                        CurrPage.Update(false);
                    end;
                }
            }
        }
    }

    procedure GetNo(): Code[20] // TODO
    begin
        exit(Rec."No.");
    end;

    procedure Load(var TempCopilotResourceProposal: Record "Copilot Resource Proposal" temporary)
    begin
        Rec.Reset();
        Rec.DeleteAll();

        TempCopilotResourceProposal.Reset();
        if TempCopilotResourceProposal.FindSet() then
            repeat
                TempCopilotResourceProposal.CalcFields("Full Explanation");
                Rec.Copy(TempCopilotResourceProposal, false);
                Rec."Full Explanation" := TempCopilotResourceProposal."Full Explanation";
                Rec.Insert();
            until TempCopilotResourceProposal.Next() = 0;

        CurrPage.Update(false);
    end;
}
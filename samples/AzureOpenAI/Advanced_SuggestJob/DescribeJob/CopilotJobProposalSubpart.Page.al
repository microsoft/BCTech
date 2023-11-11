namespace CopilotToolkitDemo.DescribeJob;

page 54321 "Copilot Job Proposal Subpart"
{
    PageType = ListPart;
    Extensible = false;
    ApplicationArea = All;
    UsageCategory = Administration;
    Caption = 'Dynamics 365 Copilot Jobs';
    SourceTable = "Copilot Job Proposal";
    SourceTableTemporary = true;

    layout
    {
        area(Content)
        {
            repeater(Tasks)
            {
                Caption = ' ';
                ShowCaption = false;
                IndentationColumn = DescriptionIndent;
                IndentationControls = "Task Description";

                field("Job Task No."; Rec."Job Task No.")
                {
                    ApplicationArea = All;
                    Style = Strong;
                    StyleExpr = StyleIsStrong;
                }
                field("Task Description"; Rec."Task Description")
                {
                    ApplicationArea = All;
                    Style = Strong;
                    StyleExpr = StyleIsStrong;
                }
                field("Action Description Preview"; Rec."Action Description Preview")
                {
                    ApplicationArea = All;
                    Caption = 'Action Description';

                    trigger OnAssistEdit()
                    var
                        InStr: InStream;
                        ActionDescription: Text;
                    begin
                        Rec.CalcFields("Action Description");
                        Rec."Action Description".CreateInStream(InStr);
                        InStr.ReadText(ActionDescription);
                        Message(ActionDescription);
                    end;
                }
                field("Resource Role"; Rec."Resource Role Description")
                {
                    ApplicationArea = All;
                }
                field("Item Category"; Rec."Item Category")
                {
                    ApplicationArea = All;
                }
                field("Start Date"; Rec."Start Date")
                {
                    ApplicationArea = All;
                }
                field("End Date"; Rec."End Date")
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    internal procedure Load(var TmpProjectJobAIProposal: Record "Copilot Job Proposal" temporary)
    begin
        Rec.Reset();
        Rec.DeleteAll();

        TmpProjectJobAIProposal.Reset();
        if TmpProjectJobAIProposal.FindSet() then
            repeat
                TmpProjectJobAIProposal.CalcFields("Action Description");
                Rec.Copy(TmpProjectJobAIProposal, false);
                Rec."Action Description" := TmpProjectJobAIProposal."Action Description";
                Rec.Insert();
            until TmpProjectJobAIProposal.Next() = 0;

        CurrPage.Update(false);
    end;

    trigger OnAfterGetRecord()
    begin
        DescriptionIndent := Rec.Indentation;
        StyleIsStrong := Rec."Job Task Type" <> Rec."Job Task Type"::Posting;
    end;

    var
        DescriptionIndent: Integer;
        StyleIsStrong: Boolean;
}
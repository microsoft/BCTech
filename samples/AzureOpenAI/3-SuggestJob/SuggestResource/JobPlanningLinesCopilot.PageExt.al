namespace CopilotToolkitDemo.SuggestResource;

using Microsoft.Projects.Project.Planning;

pageextension 54301 "Job Planning Lines Copilot" extends "Job Planning Lines"
{
    layout
    {
        addafter(Description)
        {
            field(WorkTypeCode; Rec."Work Type Code")
            {
                ApplicationArea = All;
            }
        }
    }

    actions
    {
        addlast(Prompting)
        {
            action(SuggestResourceCopilotAction)
            {
                Caption = 'Suggest resource';
                ToolTip = 'Asks Copilot which resource can be assigned to the job planning line. You will have to confirm the suggestion from Copilot.';
                Visible = Rec.Type = Rec.Type::Resource;
                ApplicationArea = All;

                trigger OnAction()
                begin
                    SuggestResourceWithAI(Rec);
                end;
            }
        }
        addlast(Prompting)
        {
            action(SuggestItemCopilotAction)
            {
                Caption = 'Suggest item';
                ToolTip = 'Asks Copilot which item can be assigned to the job planning line. You will have to confirm the suggestion from Copilot.';
                Visible = Rec.Type = Rec.Type::Item;
                ApplicationArea = All;

                trigger OnAction()
                begin
                    Message('not implemented');
                end;
            }
        }
    }

    procedure SuggestResourceWithAI(var JobPlanningLine: Record "Job Planning Line");
    var
        ResourceAIProposal: Page "Copilot Resource Proposal";
    begin
        JobPlanningLine.TestField(Type, JobPlanningLine.Type::Resource);
        ResourceAIProposal.SetJobPlanningLine(JobPlanningLine);
        ResourceAIProposal.RunModal();
        CurrPage.Update(false); //TODO
    end;

}
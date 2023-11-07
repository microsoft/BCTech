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
        addbefore("Create &Sales Invoice")
        {
            group(Copilot)
            {
                action(SuggestResource)
                {
                    Caption = 'Suggest resource';
                    ToolTip = 'Asks Copilot which resource can be assigned to the job planning line. You will have to confirm the suggestion from Copilot.';
                    Image = Sparkle;
                    Visible = Rec.Type = Rec.Type::Resource;
                    ApplicationArea = All;

                    trigger OnAction()
                    begin
                        SuggestResourceWithAI(Rec);
                    end;
                }
                action(SuggestItem)
                {
                    Caption = 'Suggest item';
                    ToolTip = 'Asks Copilot which item can be assigned to the job planning line. You will have to confirm the suggestion from Copilot.';
                    Image = Sparkle;
                    Visible = Rec.Type = Rec.Type::Item;
                    ApplicationArea = All;

                    trigger OnAction()
                    begin
                        Message('not implemented');
                    end;
                }

            }
        }
        addbefore("Create &Sales Invoice_Promoted")
        {
            actionref(SuggestResource_Promoted; SuggestResource) { }
            actionref(SuggestItem_Promoted; SuggestItem) { }
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
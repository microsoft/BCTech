pageextension 54398 "Job Planning Ext Copilot" extends "Job Planning Lines"
{
    actions
    {
        addfirst(Prompting)
        {
            action(SuggestReourceCopilotAction)
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
    }

    procedure SuggestResourceWithAI(var JobPlanningLine: Record "Job Planning Line");
    var
        ResourceAIProposal: Page "Suggest Resource Proposal";
    begin
        JobPlanningLine.TestField(Type, JobPlanningLine.Type::Resource);
        ResourceAIProposal.SetJobPlanningLine(JobPlanningLine);
        ResourceAIProposal.RunModal();
        CurrPage.Update(false); //TODO
    end;
}
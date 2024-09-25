namespace CopilotToolkitDemo.SuggestJobBasic;

using Microsoft.Projects.Project.Job;

pageextension 54394 "SuggestJob - Job List Ext" extends "Job List"
{
    actions
    {
        addfirst(Category_New)
        {
            actionref(GenerateCopilotPromoted; GenerateCopilotAction)
            {
            }
        }

        addlast(Prompting)
        {
            action(GenerateCopilotAction)
            {
                Caption = 'Draft with Copilot';
                Ellipsis = true;
                ApplicationArea = All;
                ToolTip = 'Lets Copilot generate a draft project based on your description.';

                trigger OnAction()
                begin
                    Page.RunModal(Page::"SuggestJob - Proposal");
                end;
            }
        }
    }
}
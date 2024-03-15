namespace CopilotToolkitDemo.DescribeJob;

using Microsoft.Projects.Project.Job;

pageextension 54300 "Job List Extension" extends "Job List"
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
                    Page.RunModal(Page::"Copilot Job Proposal");
                end;
            }
        }
    }
}
namespace CopilotToolkitDemo.DescribeJob;

using Microsoft.Projects.Project.Job;

pageextension 54300 "Job List Extension" extends "Job List"
{
    actions
    {
        addafter("Create Job &Sales Invoice")
        {
            action(GenerateCopilot)
            {
                Caption = 'Suggest with Copilot';
                Image = Sparkle;
                ApplicationArea = All;
                ToolTip = 'Lets Copilot generate a draft job based on your description.';

                trigger OnAction()
                begin
                    Page.RunModal(Page::"Copilot Job Proposal");
                end;
            }
        }
        addfirst(Category_New)
        {
            actionref(GenerateCopilotPromoted; GenerateCopilot)
            {
            }
        }
    }
}
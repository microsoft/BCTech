namespace DefaultPublisher.DraftProject;
using Microsoft.Projects.Project.Job;

pageextension 50100 ProjectListExtension extends "Job List"
{
    actions
    {
        addfirst(Prompting)
        {
            action("DraftWithCopilot")
            {
                ApplicationArea = All;
                Caption = 'Draft with Copilot';

                RunObject = Page "Draft Project";
            }
        }
    }
}
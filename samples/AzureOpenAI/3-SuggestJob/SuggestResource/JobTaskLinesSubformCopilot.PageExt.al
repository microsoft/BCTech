namespace CopilotToolkitDemo.SuggestResource;

using CopilotToolkitDemo.Common;
using Microsoft.Projects.Project.Job;

pageextension 54302 "Job Task Lines Sub. Copilot" extends "Job Task Lines Subform"
{
    actions
    {
        addbefore("Split &Planning Lines")
        {
            action(SplitIntoMoreDetails)
            {
                Caption = 'Breakdown with AI';
                Image = LinesFromTimesheet;
                ApplicationArea = All;

                trigger OnAction()
                var
                    AIJobsChat: Codeunit "Simplified Copilot Chat";
                    Request, Result : Text;
                    Window: Dialog;
                begin
                    /*
                    Request := StrSubstNo('Giving following task %1 under the %2 project.', Rec.Description ;

                    Request += StrSubstNo(' and given project step %1', JobPlanningLine.Description + ' ' + JobPlanningLine."Description 2");
                    Request += StrSubstNo('. Find the best suitable item for description %1. Return the best matches.', JobPlanningLine."Suggester Item");
                    Window.Open('Communicating with the Matrix...');
                    Result := AIJobsChat.Chat(Request);
                    Window.Close();
                    Message(Result);
                    */
                end;
            }
        }
    }
}
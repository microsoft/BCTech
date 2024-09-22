namespace CopilotToolkitDemo.SuggestJobBasic;
using Microsoft.Projects.Project.Job;

page 54397 "SuggestJob - Proposal Subpart"
{
    PageType = ListPart;
    Extensible = false;
    ApplicationArea = All;
    UsageCategory = Administration;
    Caption = 'Dynamics 365 Copilot Jobs';
    SourceTable = "Job Task";
    SourceTableTemporary = true;

    layout
    {
        area(Content)
        {
            repeater(Tasks)
            {
                Caption = ' ';
                ShowCaption = false;

                field("Job Task No."; Rec."Job Task No.")
                {
                    ApplicationArea = All;
                }
                field("Task Description"; Rec.Description)
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    internal procedure Load(var TempJobTask: Record "Job Task" temporary)
    begin
        Rec.Reset();
        Rec.DeleteAll();
        Rec.Copy(TempJobTask, true);

        CurrPage.Update(false);
    end;
}
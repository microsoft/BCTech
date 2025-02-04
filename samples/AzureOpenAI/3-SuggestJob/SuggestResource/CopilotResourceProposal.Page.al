namespace CopilotToolkitDemo.SuggestResource;

using Microsoft.Projects.Project.Planning;
using CopilotToolkitDemo.DescribeJob;

page 54322 "Copilot Resource Proposal"
{
    PageType = PromptDialog;
    Extensible = false;
    IsPreview = true;
    Caption = 'Allocate resource with Copilot';
    DataCaptionExpression = JobPlanningLine.Description + ',' + JobPlanningLine."Description 2" + ',' + ChatRequest;

    layout
    {
        area(Prompt)
        {
            field(TaskDescription; JobPlanningLine.Description)
            {
                Caption = 'Task description';
                Editable = false;
                ApplicationArea = All;
            }
            field(RoleDescription; JobPlanningLine."Description 2")
            {
                Caption = 'Role description';
                Editable = false;
                ApplicationArea = All;
            }
            field(ChatRequest; ChatRequest)
            {
                Caption = 'Additional notes';
                MultiLine = true;
                ApplicationArea = All;
                InstructionalText = 'If needed, provide additional notes for the resource suggestion.';

                trigger OnValidate()
                begin
                    CurrPage.Update();
                end;
            }
        }
        area(Content)
        {
            part(ProposalDetails; "Copilot Res. Proposal Subpart")
            {
                Caption = 'Suggested Resources';
                ShowFilter = false;
                ApplicationArea = All;
                Editable = false;
            }
        }
    }
    actions
    {
        area(SystemActions)
        {
            systemaction(Generate)
            {
                Caption = 'Generate';
                ToolTip = 'Generate Resource proposal with Dynamics 365 Copilot.';

                trigger OnAction()
                begin
                    RunGeneration();
                end;
            }
            systemaction(OK)
            {
                Caption = 'Confirm';
                ToolTip = 'Add selected Resource to the Project Plannig Line.';
            }
            systemaction(Cancel)
            {
                Caption = 'Discard';
                ToolTip = 'Discard Resource proposed by Dynamics 365 Copilot.';
            }
            systemaction(Regenerate)
            {
                Caption = 'Regenerate';
                ToolTip = 'Regenerate Resource proposal with Dynamics 365 Copilot.';

                trigger OnAction()
                begin
                    RunGeneration();
                end;
            }
        }
    }

    trigger OnQueryClosePage(CloseAction: Action): Boolean
    var
        TempCopilotResourceProposal: Record "Copilot Resource Proposal" temporary;
    begin
        if CloseAction = CloseAction::OK then begin
            CurrPage.ProposalDetails.Page.GetRecord(TempCopilotResourceProposal);
            JobPlanningLine.Validate("No.", TempCopilotResourceProposal."No.");
            JobPlanningLine.Modify(true);
        end;
    end;

    local procedure RunGeneration()
    var
        TempCopilotResourceProposal: Record "Copilot Resource Proposal" temporary;
        GenResourceProposal: Codeunit "Generate Resource Proposal";
        InStr: InStream;
        Attempts: Integer;
    begin
        GenResourceProposal.SetUserPrompt(ChatRequest);
        GenResourceProposal.SetTask(JobPlanningLine.Description, JobPlanningLine."Description 2");

        TempCopilotResourceProposal.Reset();
        TempCopilotResourceProposal.DeleteAll();

        for Attempts := 0 to 5 do
            if GenResourceProposal.Run() then begin
                GenResourceProposal.GetResult(TempCopilotResourceProposal);

                if not TempCopilotResourceProposal.IsEmpty() then begin
                    CurrPage.ProposalDetails.Page.Load(TempCopilotResourceProposal);

                    exit;
                end;
            end;

        if GetLastErrorText() = '' then
            Error(SomethingWentWrongErr)
        else
            Error(SomethingWentWrongWithLatestErr, GetLastErrorText());
    end;


    procedure SetJobPlanningLine(JobPlanningLine2: Record "Job Planning Line")
    begin
        JobPlanningLine := JobPlanningLine2;
    end;

    var
        SomethingWentWrongErr: Label 'Something went wrong. Please try again.';
        SomethingWentWrongWithLatestErr: Label 'Something went wrong. Please try again. The latest error is: %1';
        JobPlanningLine: Record "Job Planning Line";
        ChatRequest: Text;
}
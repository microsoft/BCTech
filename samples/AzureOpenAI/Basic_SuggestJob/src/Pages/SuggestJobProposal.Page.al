namespace CopilotToolkitDemo.SuggestJobBasic;

using Microsoft.Sales.Customer;
using Microsoft.Projects.Project.Job;

page 54395 "SuggestJob - Proposal"
{
    PageType = PromptDialog;
    Extensible = false;
    Caption = 'Draft new project with Copilot';
    DataCaptionExpression = InputProjectDescription;
    IsPreview = true;

    layout
    {
        area(Prompt) // <--- this is the input section
        {
            field(ProjectDescriptionField; InputProjectDescription)
            {
                ApplicationArea = All;
                ShowCaption = false;
                MultiLine = true;
                InstructionalText = 'Describe the project you want to create with Copilot';
            }
        }
        area(Content) // <--- this is the output section
        {
            field("Job Short Description"; TempJob.Description)
            {
                ApplicationArea = All;
                Caption = 'Project Short Description';
            }
            field(CustomerNameField; TempJob."Bill-to Name")
            {
                ApplicationArea = All;
                Caption = 'Customer Name';
                ShowMandatory = true;

                trigger OnAssistEdit()
                var
                    Customer: Record Customer;
                begin
                    if not Customer.SelectCustomer(Customer) then
                        Clear(Customer);

                    TempJob."Bill-to Name" := Customer.Name;
                    TempJob."Bill-to Customer No." := Customer."No.";
                end;

                trigger OnValidate()
                begin
                    FindCustomerNameAndNumber(TempJob."Bill-to Name", false);
                end;
            }
            part(ProposalDetails; "SuggestJob - Proposal Subpart")
            {
                Caption = 'Job structure';
                ShowFilter = false;
                ApplicationArea = All;
                Editable = true;
                Enabled = true;
            }
        }
    }
    actions
    {
        area(PromptGuide)
        {
            action(OrganizeCampaign)
            {
                ApplicationArea = All;
                Caption = 'Create a campaign';

                trigger OnAction()
                begin
                    InputProjectDescription := 'Campaign on [social media] for [Customer] to [promote education].';
                end;
            }
            group(Furnishing)
            {
                action(FurnishOffice)
                {
                    ApplicationArea = All;
                    Caption = 'Furnish an office';

                    trigger OnAction()
                    begin
                        InputProjectDescription := '[Customer] needs to furnish [office] for [4 people].';
                    end;
                }

                action(SetUpConferenceRooms)
                {
                    ApplicationArea = All;
                    Caption = 'Set up work areas';

                    trigger OnAction()
                    begin
                        InputProjectDescription := 'Design and set up [work areas] for [Customer].';
                    end;
                }
            }

            action(OrganizeWorkshop)
            {
                ApplicationArea = All;
                Caption = 'Organize a workshop';

                trigger OnAction()
                begin
                    InputProjectDescription := 'Organize a [workshop] for [Customer] about [sustainability].';
                end;
            }
        }
        area(SystemActions)
        {
            systemaction(Generate)
            {
                Caption = 'Generate';
                ToolTip = 'Generate a project structure with Dynamics 365 Copilot.';

                trigger OnAction()
                begin
                    RunGeneration();
                end;
            }
            systemaction(OK)
            {
                Caption = 'Keep it';
                ToolTip = 'Save the Project proposed by Dynamics 365 Copilot.';
            }
            systemaction(Cancel)
            {
                Caption = 'Discard it';
                ToolTip = 'Discard the Project proposed by Dynamics 365 Copilot.';
            }
            systemaction(Regenerate)
            {
                Caption = 'Regenerate';
                ToolTip = 'Regenerate the Project proposed by Dynamics 365 Copilot.';

                trigger OnAction()
                begin
                    RunGeneration();
                end;
            }
        }
    }

    trigger OnQueryClosePage(CloseAction: Action): Boolean
    var
        SuggestJobGenerateProposal: Codeunit "SuggestJob - Generate Proposal";
    begin
        if CloseAction = CloseAction::OK then
            Save();
    end;

    local procedure RunGeneration()
    var
        SuggestJobGenerateProposal: Codeunit "SuggestJob - Generate Proposal";
        ProgressDialog: Dialog;
        Attempts: Integer;
        LocalCustomerName: Text;
    begin
        if InputProjectDescription = '' then
            Error(ProjectDescriptionEmptyErr);

        if StrLen(InputProjectDescription) < 20 then
            Message(DescriptionTooShortMsg);

        ProgressDialog.Open(GeneratingTextDialogTxt);
        SuggestJobGenerateProposal.SetUserPrompt(InputProjectDescription);

        TempJobTask.Reset();
        TempJobTask.DeleteAll();

        for Attempts := 0 to 3 do
            if SuggestJobGenerateProposal.Run() then
                if SuggestJobGenerateProposal.GetResult(TempJobTask, TempJob.Description, LocalCustomerName) then begin
                    FindCustomerNameAndNumber(LocalCustomerName, true);
                    CurrPage.ProposalDetails.Page.Load(TempJobTask);
                    exit;
                end;

        if GetLastErrorText() = '' then
            Error(SomethingWentWrongErr)
        else
            Error(SomethingWentWrongWithLatestErr, GetLastErrorText());
    end;

    local procedure FindCustomerNameAndNumber(InputCustomerName: Text; Silent: Boolean)
    var
        Customer: Record Customer;
    begin
        if InputCustomerName <> '' then begin
            Customer.SetFilter("Search Name", '%1', '*' + UpperCase(InputCustomerName) + '*');

            if not Customer.FindFirst() then
                if not Silent then
                    Error(CustomerDoesNotExistErr);
        end;

        TempJob."Bill-to Name" := Customer.Name;
        TempJob."Bill-to Customer No." := Customer."No.";
    end;

    procedure Save()
    var
        JobTask: Record "Job Task";
        Job: Record Job;
    begin
        Job.Init();
        Job.Description := TempJob.Description;
        Job.Validate("Bill-to Customer No.", TempJob."Bill-to Customer No.");
        Job.Insert(true);

        if TempJobTask.FindSet() then
            repeat
                JobTask.Init();
                JobTask.TransferFields(TempJobTask);
                JobTask.Validate("Job No.", Job."No.");
                JobTask.Insert(true);
            until TempJobTask.Next() = 0;
    end;

    var
        TempJob: Record Job temporary;
        TempJobTask: Record "Job Task" temporary;
        InputProjectDescription: Text;
        GeneratingTextDialogTxt: Label 'Generating with Copilot...';
        SomethingWentWrongErr: Label 'Something went wrong. Please try again.';
        SomethingWentWrongWithLatestErr: Label 'Something went wrong. Please try again. The latest error is: %1';
        ProjectDescriptionEmptyErr: Label 'Please describe a project that Copilot can draft for you.';
        CustomerDoesNotExistErr: Label 'Customer does not exist';
        DescriptionTooShortMsg: Label 'The description of the project is too short, and this might impact the result quality.';
}
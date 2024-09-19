namespace CopilotToolkitDemo.SuggestJobBasic;

using Microsoft.Projects.Project.Job;
using System.AI;

codeunit 54390 "SuggestJob - Generate Proposal"
{
    trigger OnRun()
    begin
        GenerateJobProposal();
    end;

    procedure SetUserPrompt(InputUserPrompt: Text)
    begin
        UserPrompt := InputUserPrompt;
    end;

    procedure GetResult(var LocalTempJobTask: Record "Job Task" temporary; var JobDescription: Text; var CustomerName: Text)
    begin
        LocalTempJobTask.Copy(TempJobTask, true);
        JobDescription := TempJob.Description;
        CustomerName := TempJob."Bill-to Name";
    end;

    local procedure GenerateJobProposal()
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIDeployments: Codeunit "AOAI Deployments";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIChatCompletionParams: Codeunit "AOAI Chat Completion Params";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        SuggestJobCreateJob: Codeunit "SuggestJob - Create Job";
        SuggestJobCreateJobTask: Codeunit "SuggestJob - Create Job Task";
        AoaiKey: SecretText;
    begin
        AoaiKey := Format(CreateGuid());
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions", 'notused', 'notused', AoaiKey, AOAIDeployments.GetGPT4oLatest());

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Suggest Project");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(GetSystemPrompt());
        AOAIChatMessages.AddUserMessage(UserPrompt);
        AOAIChatMessages.SetToolInvokePreference("AOAI Tool Invoke Preference"::Automatic);
        AOAIChatMessages.SetToolChoice('auto');
        AOAIChatMessages.AddTool(SuggestJobCreateJob);
        AOAIChatMessages.AddTool(SuggestJobCreateJobTask);

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if not AOAIOperationResponse.IsSuccess() then
            Error(AOAIOperationResponse.GetError());

        SuggestJobCreateJob.GetJob(TempJob);
        SuggestJobCreateJobTask.GetJobTasks(TempJobTask);
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    begin
        SystemPrompt := StrSubstNo(
            'The user will describe a project. Your task is to prepare the project plan for this project to be used in Microsoft Dynamics 365 Business Central.' +
            'You must call the function "create_job" to create the job, and the function "create_job_task" to create at least 6 tasks for the job.' +
            'Use %1 as a start date for the project. Use the yyyy-mm-dd date format for dates.',
            Format(CurrentDateTime(), 0, 9));
    end;

    var
        TempJobTask: Record "Job Task" temporary;
        TempJob: Record "Job" temporary;
        UserPrompt: Text;
}
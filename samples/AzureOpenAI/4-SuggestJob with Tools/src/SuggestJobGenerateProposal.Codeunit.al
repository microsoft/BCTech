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

    procedure GetResult(var LocalTempJobTask: Record "Job Task" temporary; var JobDescription: Text; var CustomerName: Text): Boolean
    begin
        if TempJobTask.IsEmpty() then
            exit(false);

        LocalTempJobTask.Copy(TempJobTask, true);
        JobDescription := TempJob.Description;
        CustomerName := TempJob."Bill-to Name";

        exit(true);
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
    begin
        // If you are using managed resources, call this function:
        // NOTE: endpoint, deployment, and key are only used to verify that you have a valid Azure OpenAI subscription; we don't use them to generate the result
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions",
            GetEndpoint(), GetDeployment(), GetApiKey(), AOAIDeployments.GetGPT4oLatest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", GetEndpoint(), GetDeployment(), GetApiKey());

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Suggest Project");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(GetSystemPrompt());
        AOAIChatMessages.AddUserMessage(UserPrompt);

        AOAIChatMessages.AddTool(SuggestJobCreateJob);
        AOAIChatMessages.AddTool(SuggestJobCreateJobTask);
        AOAIChatMessages.SetToolInvokePreference("AOAI Tool Invoke Preference"::Automatic);
        AOAIChatMessages.SetToolChoice('auto');

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if not AOAIOperationResponse.IsSuccess() then
            Error(AOAIOperationResponse.GetError());

        SuggestJobCreateJob.GetJob(TempJob);
        SuggestJobCreateJobTask.GetJobTasks(TempJobTask);
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    begin
        SystemPrompt :=
            'The user will describe a project. Your task is to prepare the project plan for this project to be used in Microsoft Dynamics 365 Business Central.' +
            'You must call the function "create_job" to create the job, and the function "create_job_task" to create at least 6 tasks for the job.';
    end;

    local procedure GetApiKey(): SecretText
    begin
        // Use your Azure Open AI secret key.
        // NOTE: Do not add the key in plain text. Instead, use Isolated Storage or other more secure ways.
        exit(Format(CreateGuid()));
    end;

    local procedure GetDeployment(): Text
    begin
        // Use your deployment name from Azure Open AI here
        exit('gpt-' + CreateGuid());
    end;

    local procedure GetEndpoint(): Text
    begin
        // Use your endpoint name from Azure Open AI here
        exit('https://my-deployment.azure.com/');
    end;

    var
        TempJobTask: Record "Job Task" temporary;
        TempJob: Record "Job" temporary;
        UserPrompt: Text;
}
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
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions", AOAIDeployments.GetGPT41Latest());
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

        if AOAIOperationResponse.IsSuccess() then begin
            SuggestJobCreateJob.GetJob(TempJob);
            SuggestJobCreateJobTask.GetJobTasks(TempJobTask);
            exit;
        end;

        case AOAIOperationResponse.GetStatusCode() of
            402: // Payment Required
                Error('Your Entra Tenant ran out of AI quota. '
                    + 'Make sure your Business Central environment is linked to a Power Platform environment, and billing is set up correctly. '
                    + 'Consult the Business Central documentation for more information.');
            429: // Too many requests
                Error('You have been using Copilot very fast! '
                    + 'So fast that we suspect you might have some automation or scheduled task that calls Copilot a lot. '
                    + 'Have a look at your Job Queues, scheduled tasks, and automations, and make sure that everything looks fine. '
                    + 'And don''t worry, you''ll be able to use Copilot again in less than a minute!');
            503: // Service Unavailable (very very rare)
                Error('It seems like our services are under heavy load right now. '
                    + 'This happens very rarely, and our engineers are notified whenever this happens. '
                    + 'We are probably already working on it as soon as you are done reading this message!');
            else // Others
                Error('A generic error occurred: ' + AOAIOperationResponse.GetError());
        end;
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

    local procedure GetAccountName(): Text
    begin
        // Use your Azure OpenAI account name (the first part of your own Azure OpenAI url, for example for "MyPartner.openai.azure.com" use "MyPartner"
        exit('MyPartner');
    end;

    var
        TempJobTask: Record "Job Task" temporary;
        TempJob: Record "Job" temporary;
        UserPrompt: Text;
}
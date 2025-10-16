namespace CopilotToolkitDemo.Common;

using System.AI;

codeunit 54334 "Simplified Copilot Chat"
{
    procedure Chat(SystemPrompt: Text; UserPrompt: Text): Text
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIDeployments: Codeunit "AOAI Deployments";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIChatCompletionParams: Codeunit "AOAI Chat Completion Params";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        Result: Text;
    begin
        // If you are using managed resources, call this function:
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions", AOAIDeployments.GetGPT41Latest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", GetEndpoint(), GetDeployment(), GetSecretKey());

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Describe Project");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(SystemPrompt);
        AOAIChatMessages.AddUserMessage(UserPrompt);

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then begin
            Result := AOAIChatMessages.GetLastMessage();

            // Sometimes AI model returns special characters against instructions. This is a workaround to fix that for this demo, not recommended for production use.
            Result := Result.Replace('&', '&amp;');
            if Result.StartsWith('```xml') then
                Result := CopyStr(Result.TrimEnd('`'), 1 + 6);

            exit(Result);
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
                exit('A generic error occurred: ' + AOAIOperationResponse.GetError());
        end;
    end;
}
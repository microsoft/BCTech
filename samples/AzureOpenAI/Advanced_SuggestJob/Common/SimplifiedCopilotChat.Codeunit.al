namespace CopilotToolkitDemo.Common;

using System.AI;

codeunit 54334 "Simplified Copilot Chat"
{
    procedure Chat(SystemPrompt: Text; UserPrompt: Text): Text
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIChatCompletionParams: Codeunit "AOAI Chat Completion Params";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        AOAIDeployments: Codeunit "AOAI Deployments";
        Result: Text;
        EntityTextModuleInfo: ModuleInfo;
    begin
        // This way of retrieving the deployment version will be available in an upcoming minor version
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", AOAIDeployments.GetGPT35TurboLatest());
        AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", 'gpt-35-turbo-latest');

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Describe Project");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(SystemPrompt);
        AOAIChatMessages.AddUserMessage(UserPrompt);

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then
            Result := AOAIChatMessages.GetLastMessage()
        else
            Error(AOAIOperationResponse.GetError());

        // Sometimes AI model returns special characters against instructions. This is a workaround to fix that for this demo, not recommended for production use.
        Result := Result.Replace('&', '&amp;');

        exit(Result);
    end;

    procedure GenerateTextCompletion(UserPrompt: Text): Text
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAITextCompletionParams: Codeunit "AOAI Text Completion Params";
        AOAIDeployments: Codeunit "AOAI Deployments";
    begin
        AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Text Completions", AOAIDeployments.GetGPT35TurboLatest());
        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Describe Project");

        AOAITextCompletionParams.SetMaxTokens(2500);
        AOAITextCompletionParams.SetTemperature(0);

        AzureOpenAI.GenerateTextCompletion(UserPrompt, AOAITextCompletionParams, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then
            Error(AOAIOperationResponse.GetError());

        exit(AOAIOperationResponse.GetResult());
    end;
}
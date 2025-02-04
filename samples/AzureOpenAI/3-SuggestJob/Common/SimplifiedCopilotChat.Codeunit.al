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
        IsolatedStorageWrapper: Codeunit "Isolated Storage Wrapper";
        Result: Text;
    begin
        // If you are using managed resources, call this function:
        // NOTE: endpoint, deployment, and key are only used to verify that you have a valid Azure OpenAI subscription; we don't use them to generate the result
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions",
            IsolatedStorageWrapper.GetEndpoint(), IsolatedStorageWrapper.GetDeployment(), IsolatedStorageWrapper.GetSecretKey(), AOAIDeployments.GetGPT4oLatest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", IsolatedStorageWrapper.GetEndpoint(), IsolatedStorageWrapper.GetDeployment(), IsolatedStorageWrapper.GetSecretKey());

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
        if Result.StartsWith('```xml') then
            Result := CopyStr(Result.TrimEnd('`'), 1 + 6);

        exit(Result);
    end;

    procedure GenerateTextCompletion(UserPrompt: Text): Text
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAITextCompletionParams: Codeunit "AOAI Text Completion Params";
        IsolatedStorageWrapper: Codeunit "Isolated Storage Wrapper";
    begin
        AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Text Completions", IsolatedStorageWrapper.GetEndpoint(),
            IsolatedStorageWrapper.GetDeployment(), IsolatedStorageWrapper.GetSecretKey());
        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Describe Project");

        AOAITextCompletionParams.SetMaxTokens(2500);
        AOAITextCompletionParams.SetTemperature(0);

        AzureOpenAI.GenerateTextCompletion(UserPrompt, AOAITextCompletionParams, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then
            Error(AOAIOperationResponse.GetError());

        exit(AOAIOperationResponse.GetResult());
    end;
}
namespace CopilotToolkitDemo.ItemSubstitution;

using System.IO;
using Microsoft.Inventory.Item;
using System.Environment;
using System.AI;
using System.Utilities;

codeunit 54323 "Generate Item Sub Proposal"
{
    trigger OnRun()
    begin
        GenerateItemProposal();
    end;

    procedure SetUserPrompt(InputUserPrompt: Text)
    begin
        UserPrompt := InputUserPrompt;
    end;

    procedure GetResult(var TmpItemSubstAIProposal2: Record "Copilot Item Sub Proposal" temporary)
    begin
        TmpItemSubstAIProposal2.Copy(TmpItemSubstAIProposal, true);
    end;

    procedure SetSuggestOnlyAvailableItems()
    begin
        SuggestOnlyAvailableItems := true;
    end;

    internal procedure GetCompletionResult(): Text
    begin
        exit(CompletionResult);
    end;

    local procedure GenerateItemProposal()
    var
        InStr: InStream;
        OutStr: OutStream;
        CurrInd, LineNo : Integer;
        JResTok: JsonToken;
        JResItemsTok: JsonToken;
        JsonItemsArray: JsonArray;
        JItemTok: JsonToken;
        JItem: JsonToken;
        NumberToken: JsonToken;
        DescToken: JsonToken;
        ExplToken: JsonToken;
        InvToken: JsonToken;
        DateVar: Date;
        TmpText: Text;
        i: Integer;
    begin
        CompletionResult := '';
        TmpText := Chat(GetSystemPrompt(), GetFinalUserPrompt(UserPrompt));

        JResTok.ReadFrom(TmpText);
        JResTok.AsObject().Get('items', JResItemsTok);
        JsonItemsArray := JResItemsTok.AsArray();

        if JsonItemsArray.Count() > 0 then begin
            LineNo := 10000;
            for i := 0 to JsonItemsArray.Count() - 1 do begin
                JsonItemsArray.Get(i, JItem);
                TmpItemSubstAIProposal.Init();

                if JItem.AsObject().Get('number', NumberToken) then
                    TmpItemSubstAIProposal."No." := UpperCase(CopyStr(NumberToken.AsValue().AsText(), 1, MaxStrLen(TmpItemSubstAIProposal."No.")));

                if JItem.AsObject().Get('description', DescToken) then
                    TmpItemSubstAIProposal.Description := CopyStr(DescToken.AsValue().AsText(), 1, MaxStrLen(TmpItemSubstAIProposal.Description));

                if JItem.AsObject().Get('explanation', ExplToken) then begin
                    TmpItemSubstAIProposal.Explanation := CopyStr(ExplToken.AsValue().AsText(), 1, MaxStrLen(TmpItemSubstAIProposal.Explanation));
                    TmpItemSubstAIProposal."Full Explanation".CreateOutStream(OutStr);
                    OutStr.WriteText(ExplToken.AsValue().AsText());
                end;
                if JItem.AsObject().Get('inventory', InvToken) then begin
                    TmpItemSubstAIProposal.Quantity := InvToken.AsValue().AsDecimal();
                end;

                TmpItemSubstAIProposal.Insert();
            end;
        end;
    end;

    procedure Chat(ChatSystemPrompt: Text; ChatUserPrompt: Text): Text
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        EnvironmentInformation: Codeunit "Environment Information";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIChatCompletionParams: Codeunit "AOAI Chat Completion Params";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        AOAIDeployments: Codeunit "AOAI Deployments";
        IsolatedStorageWrapper: Codeunit "Isolated Storage Wrapper";
        Result: Text;
        EntityTextModuleInfo: ModuleInfo;
    begin
        // If you are using managed resources, call this function:
        // NOTE: account name and key are only used to verify that you have a valid Azure OpenAI subscription; we don't use them to generate the result
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions",
            IsolatedStorageWrapper.GetAccountName(), IsolatedStorageWrapper.GetSecretKey(), AOAIDeployments.GetGPT4oLatest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", IsolatedStorageWrapper.GetEndpoint(), IsolatedStorageWrapper.GetDeployment(), IsolatedStorageWrapper.GetSecretKey());

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Find Item Substitutions");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);
        AOAIChatCompletionParams.SetJsonMode(true);

        AOAIChatMessages.AddSystemMessage(ChatSystemPrompt);
        AOAIChatMessages.AddUserMessage(ChatUserPrompt);

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then begin
            Result := AOAIChatMessages.GetLastMessage();
            CompletionResult := Result;
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

    local procedure GetFinalUserPrompt(InputUserPrompt: Text) FinalUserPrompt: Text
    var
        Item: Record Item;
        Newline: Char;
    begin
        Newline := 10;
        FinalUserPrompt := 'These are the items:' + Newline;
        if Item.FindSet() then
            repeat begin
                // Calculate inventory for the item
                Item.CalcFields(Inventory);
                FinalUserPrompt +=
                    'Number: ' + Item."No." + ', ' +
                    'Description:' + Item.Description + '. ' +
                    'Inventory:' + Item.Inventory.ToText() + Newline;
            end until Item.Next() = 0;

        FinalUserPrompt += Newline;
        FinalUserPrompt += StrSubstNo('The description of the item that needs to be substituted is: %1.', InputUserPrompt);
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    var
        Item: Record Item;
    begin
        SystemPrompt += 'The user will provide an item description, and a list of other items. Your task is to find items that can substitute that item.';
        SystemPrompt += ' Try to suggest several relevant items if possible.';
        if SuggestOnlyAvailableItems then
            SystemPrompt += ' Only suggest items that have inventory larger than 0.';
        SystemPrompt += ' The output should be in json with items array as a root node.';
        SystemPrompt += ' Each item should be a json object with the following fields:';
        SystemPrompt += ' number - item number, description - item description, inventory - item inventory, explanation - explanation why this item was suggested.';
        SystemPrompt += ' Do not use line breaks or other special characters in explanation.';
    end;

    var
        TmpItemSubstAIProposal: Record "Copilot Item Sub Proposal" temporary;
        UserPrompt: Text;
        SuggestOnlyAvailableItems: Boolean;
        CompletionResult: Text;
}
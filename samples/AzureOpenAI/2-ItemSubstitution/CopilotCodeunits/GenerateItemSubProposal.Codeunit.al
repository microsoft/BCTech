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

    local procedure GenerateItemProposal()
    var
        TmpXmlBuffer: Record "XML Buffer" temporary;
        TempBlob: Codeunit "Temp Blob";
        InStr: InStream;
        OutStr: OutStream;
        CurrInd, LineNo : Integer;
        DateVar: Date;
        TmpText: Text;
    begin
        TempBlob.CreateOutStream(OutStr);
        TmpText := Chat(GetSystemPrompt(), GetFinalUserPrompt(UserPrompt));
        OutStr.WriteText(TmpText);
        TempBlob.CreateInStream(InStr);

        TmpXmlBuffer.DeleteAll();
        TmpXmlBuffer.LoadFromStream(InStr);

        Clear(OutStr);
        LineNo := 10000;
        if TmpXmlBuffer.FindSet() then
            repeat
                case TmpXmlBuffer.Path of
                    '/items/item':
                        TmpItemSubstAIProposal.Init();
                    '/items/item/number':
                        begin
                            TmpItemSubstAIProposal."No." := UpperCase(CopyStr(TmpXmlBuffer.GetValue(), 1, MaxStrLen(TmpItemSubstAIProposal."No.")));
                            TmpItemSubstAIProposal.Insert();
                        end;
                    '/items/item/description':
                        begin
                            TmpItemSubstAIProposal.Description := CopyStr(TmpXmlBuffer.GetValue(), 1, MaxStrLen(TmpItemSubstAIProposal.Description));
                            TmpItemSubstAIProposal.Modify();
                        end;
                    '/items/item/explanation':
                        begin
                            TmpItemSubstAIProposal.Explanation := CopyStr(TmpXmlBuffer.GetValue(), 1, MaxStrLen(TmpItemSubstAIProposal.Explanation));
                            TmpItemSubstAIProposal."Full Explanation".CreateOutStream(OutStr);
                            OutStr.WriteText(TmpXmlBuffer.GetValue());
                            TmpItemSubstAIProposal.Modify();
                        end;
                end;
            until TmpXmlBuffer.Next() = 0;
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
        // NOTE: endpoint, deployment, and key are only used to verify that you have a valid Azure OpenAI subscription; we don't use them to generate the result
        AzureOpenAI.SetManagedResourceAuthorization(Enum::"AOAI Model Type"::"Chat Completions",
            IsolatedStorageWrapper.GetEndpoint(), IsolatedStorageWrapper.GetDeployment(), IsolatedStorageWrapper.GetSecretKey(), AOAIDeployments.GetGPT4oLatest());
        // If you are using your own Azure OpenAI subscription, call this function instead:
        // AzureOpenAI.SetAuthorization(Enum::"AOAI Model Type"::"Chat Completions", IsolatedStorageWrapper.GetEndpoint(), IsolatedStorageWrapper.GetDeployment(), IsolatedStorageWrapper.GetSecretKey());

        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Find Item Substitutions");

        AOAIChatCompletionParams.SetMaxTokens(2500);
        AOAIChatCompletionParams.SetTemperature(0);

        AOAIChatMessages.AddSystemMessage(ChatSystemPrompt);
        AOAIChatMessages.AddUserMessage(ChatUserPrompt);

        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIChatCompletionParams, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then
            Result := AOAIChatMessages.GetLastMessage()
        else
            Error(AOAIOperationResponse.GetError());

        Result := Result.Replace('&', '&amp;');
        Result := Result.Replace('"', '');
        Result := Result.Replace('''', '');

        exit(Result);
    end;

    local procedure GetFinalUserPrompt(InputUserPrompt: Text) FinalUserPrompt: Text
    var
        Item: Record Item;
        Newline: Char;
    begin
        Newline := 10;
        FinalUserPrompt := 'These are the available items:' + Newline;
        if Item.FindSet() then
            repeat
                FinalUserPrompt +=
                    'Number: ' + Item."No." + ', ' +
                    'Description:' + Item.Description + '.' + Newline;
            until Item.Next() = 0;

        FinalUserPrompt += Newline;
        FinalUserPrompt += StrSubstNo('The description of the item that needs to be substituted is: %1.', InputUserPrompt);
    end;

    local procedure GetSystemPrompt() SystemPrompt: Text
    var
        Item: Record Item;
    begin
        SystemPrompt += 'The user will provide an item description, and a list of other items. Your task is to find items that can substitute that item.';
        SystemPrompt += 'Try to suggest several relevant items if possible.';
        SystemPrompt += 'The output should be in xml, containing item number (use number tag), item description (use description tag), and explanation why this item was suggested (use explanation tag).';
        SystemPrompt += 'Use items as a root level tag, use item as item tag.';
        SystemPrompt += 'Do not use line breaks or other special characters in explanation.';
        SystemPrompt += 'Skip empty nodes.';
    end;

    var
        TmpItemSubstAIProposal: Record "Copilot Item Sub Proposal" temporary;
        UserPrompt: Text;
}
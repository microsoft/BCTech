// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 50100 "Demo1"
{
    ApplicationArea = Basic, Suite;
    Caption = 'Demo1 - Solar System';
    DelayedInsert = true;
    SourceTableTemporary = true;
    PageType = PromptDialog;
    Extensible = false;
    SourceTable = "Copilot Test Table";

    layout
    {
        area(Prompt)
        {
            field(Input; Rec.Input)
            {
                InstructionalText = 'Ask a question about the solar system';
                Enabled = true;
                Editable = true;
            }
        }

        area(Content)
        {
            field(Output; BigOutput)
            {
                Enabled = true;
                Editable = true;
                MultiLine = true;
                ExtendedDatatype = RichContent;
            }
        }
    }

    actions
    {
        area(SystemActions)
        {
            systemaction(Generate)
            {
                Tooltip = 'Generate a suggestion based on the input prompt';
                trigger OnAction()
                begin
                    GenerateText();
                end;
            }

            systemaction(Regenerate)
            {
                Tooltip = 'Generate a suggestion based on the input prompt';
                trigger OnAction()
                begin
                    GenerateText();
                end;
            }

            systemaction(Cancel)
            {
                ToolTip = 'Discards all suggestions and dismisses the dialog';
            }

            systemaction(Ok)
            {
                Caption = 'Keep it';
                ToolTip = 'Accepts the current suggestion and dismisses the dialog';
            }
        }
    }

    trigger OnInit()
    begin
        Rec.Init();
        Rec.Insert();
    end;

    trigger OnAfterGetCurrRecord()
    begin
        if Rec.IsEmpty() then
            exit;

        BigOutput := Rec.Output;
    end;

    var
        BigOutput: Text;

    local procedure GenerateText()
    var
        AzureOpenAi: Codeunit "Azure OpenAI";
        AoaiChatMessages: Codeunit "AOAI Chat Messages";
        AoaiDeployments: Codeunit "AOAI Deployments";
        AoaiOperationResponse: Codeunit "AOAI Operation Response";
        AoaiToken: Codeunit "AOAI Token";
        LastError: Text;
        ProgressDialog: Dialog;
    begin
        ProgressDialog.Open('Exploring the cosmos');

        AzureOpenAi.SetCopilotCapability(Enum::"Copilot Capability"::"DOK Nordic");
        AzureOpenAi.SetAuthorization(enum::"AOAI Model Type"::"Chat Completions", 'gpt-35-turbo-preview'); // specify endpoint + key if not using managed AI resources

        AoaiChatMessages.AddSystemMessage('You are an AI assistant that helps people find information about the solar system. Decline to answer if the question is not related to the solar system.');
        AoaiChatMessages.AddUserMessage(Rec.Input);

        // Example use of token counting
        if AoaiToken.GetGPT35TokenCount(Rec.Input) > 1000 then
            Error('Input too long');

        AzureOpenAi.GenerateChatCompletion(AoaiChatMessages, AoaiOperationResponse);
        HandleError(AoaiOperationResponse);

        BigOutput := AoaiChatMessages.GetLastMessage();

        Rec.AddEntry(Rec.Input, BigOutput);
        ProgressDialog.Close();
    end;

    local procedure HandleError(var AoaiOperationResponse: Codeunit "AOAI Operation Response")
    var
        LastError: Text;
    begin
        if not AoaiOperationResponse.IsSuccess() then begin
            LastError := AoaiOperationResponse.GetError();
            if LastError = '' then
                LastError := GetLastErrorText();

            Error(LastError);
        end;
    end;
}
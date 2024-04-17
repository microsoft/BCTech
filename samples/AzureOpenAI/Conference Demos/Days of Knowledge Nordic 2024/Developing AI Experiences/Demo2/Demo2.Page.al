// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 50102 "Demo2"
{
    ApplicationArea = Basic, Suite;
    Caption = 'Demo2 - Weather';
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
                InstructionalText = 'Enter the location you would like the weather for';
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
        Secret: Text;
        LastError: Text;
        ProgressDialog: Dialog;
    begin
        ProgressDialog.Open('Looking up the weather');

        AzureOpenAi.SetCopilotCapability(Enum::"Copilot Capability"::"DOK Nordic");
        AzureOpenAi.SetAuthorization(enum::"AOAI Model Type"::"Chat Completions", 'gpt-35-turbo-preview'); // specify endpoint + key if not using managed AI resources

        AoaiChatMessages.AddSystemMessage('You are an AI assistant that helps people find information about the current weather. Decline to answer if the question is not asking about the weather in a particular location.');
        AoaiChatMessages.AddUserMessage(Rec.Input);

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
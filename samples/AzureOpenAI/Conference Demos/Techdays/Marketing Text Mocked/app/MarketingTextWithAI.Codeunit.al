namespace Techdays.AITestToolkitDemo;
using Microsoft.Inventory.Item;
using System.AI;

codeunit 50100 "Marketing Text With AI"
{
    Access = Internal;

    procedure GenerateTagLine(ItemNo: Code[20]; MaxLength: Integer): Text
    var
        Item: Record Item;
        TagLine: Text;
    begin
        // Generate the tag line using AI
        Item.Get(ItemNo);

        MaxLength := 150;
        TagLine := GenerateCompletion('Generate *only* the tagline for the item ' + Item.Description
                            + ' with unit of measure ' + Item."Base Unit of Measure"
                            + '. *The maximum length of the tagline should be ' + Format(MaxLength) + ' characters*.',
                            GeneratedTextOption::Tagline);

        exit(TagLine);
    end;
































    procedure GenerateMarketingText(ItemNo: Code[20]; Style: Enum "Marketing Text Style"): Text
    var
        Item: Record Item;
    begin
        // Generate the marketing text using AI
        Item.Get(ItemNo);

        exit(GenerateCompletion('Generate the marketing text paragraph (within 200 words) for the item ' + Item.Description
                            + ' with unit of measure ' + Item."Base Unit of Measure"
                            + '. The style should be ' + Format(Style), GeneratedTextOption::Content));

    end;

    local procedure GenerateCompletion(UserPrompt: Text; TextOption: Option Tagline,Content): Text
    var
        AzureOpenAI: Codeunit "Azure OpenAI";
        AOAIChatMessages: Codeunit "AOAI Chat Messages";
        AOAIOperationResponse: Codeunit "AOAI Operation Response";
        AOAIFunctionResponse: Codeunit "AOAI Function Response";
        GenerateTextFunctionCalling: Codeunit "Generate Text Function Calling";
        SystemPromptLbl: Label 'You can generate marketing text and tagline for the marketing text. Generate them based on the user''s instructions.', Locked = true;
    begin
        // Setup Azure OpenAI
        AzureOpenAI.SetCopilotCapability(Enum::"Copilot Capability"::"Marketing Text Simple");
        SetAuthorization(AzureOpenAI);

        // Add functions
        GenerateTextFunctionCalling.SetOption(TextOption);
        AOAIChatMessages.AddTool(GenerateTextFunctionCalling);
        AOAIChatMessages.SetToolChoice('auto');

        AOAIChatMessages.SetPrimarySystemMessage(Format(SystemPromptLbl));
        AOAIChatMessages.AddUserMessage(UserPrompt);

        // Start Order Copilot
        AzureOpenAI.GenerateChatCompletion(AOAIChatMessages, AOAIOperationResponse);

        if AOAIOperationResponse.IsSuccess() then begin
            if AOAIOperationResponse.IsFunctionCall() then begin
                AOAIFunctionResponse := AOAIOperationResponse.GetFunctionResponse();
                if AOAIFunctionResponse.IsSuccess() then
                    exit(AOAIFunctionResponse.GetResult())
                else
                    Error(AOAIFunctionResponse.GetError());
            end;
        end else
            Error(AOAIOperationResponse.GetError());
    end;







































    var
        GeneratedTextOption: Option Tagline,Content;
}
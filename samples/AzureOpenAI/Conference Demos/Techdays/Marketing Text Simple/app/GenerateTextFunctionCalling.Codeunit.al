// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Techdays.AITestToolkitDemo;
using System.AI;

codeunit 50103 "Generate Text Function Calling" implements "AOAI Function"
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    var
        FunctionNameLbl: Label 'generate_product_marketing_ad', Locked = true;
        GeneratedTextOption: Option Tagline,Content;

    procedure GetPrompt(): JsonObject
    var
        Prompt: JsonObject;
        FunctionObject: JsonObject;
        Parameters: JsonObject;
        Properties: JsonObject;
        Parameter: JsonObject;
    begin
        Prompt.Add('type', 'function');

        FunctionObject.Add('name', FunctionNameLbl);
        FunctionObject.Add('description', 'This function gets the marketing text content or tagline based on the given item description and unit of measure for Business Central.');

        case
            GeneratedTextOption of
            GeneratedTextOption::Tagline:
                begin
                    Parameter.Add('type', 'string');
                    Parameter.Add('description', 'This contains the marketing tagline based on the user input and the requested maximum length');
                    Properties.Add('marketing_text_tagline', Parameter);
                end;
            GeneratedTextOption::Content:
                begin
                    Parameter.Add('type', 'string');
                    Parameter.Add('description', 'This contains the marketing text based on the user input');
                    Properties.Add('marketing_text_body', Parameter);
                end;
        end;

        Parameters.Add('type', 'object');
        Parameters.Add('properties', Properties);

        FunctionObject.Add('parameters', Parameters);
        Prompt.Add('function', FunctionObject);

        exit(Prompt);
    end;

    [NonDebuggable]
    procedure Execute(Arguments: JsonObject): Variant
    var
        TaglineToken: JsonToken;
        ParagraphToken: JsonToken;
        Result: Text;
    begin
        if Arguments.Get('marketing_text_tagline', TaglineToken) then
            Result := TaglineToken.AsValue().AsText()
        else
            if Arguments.Get('marketing_text_body', ParagraphToken) then
                Result += ParagraphToken.AsValue().AsText();
        exit(Result);
    end;

    procedure GetName(): Text
    begin
        exit(FunctionNameLbl);
    end;

    procedure SetOption(TextOption: Option Tagline,Content)
    begin
        GeneratedTextOption := TextOption;
    end;

}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
codeunit 50102 "Weather Function"
{

    /*
    {
        "type": "function",
        "function": {
            "name": "get_current_weather",
            "description": "Get the current weather",
            "parameters": {
            "type": "object",
            "properties": {
                "location": {
                    "type": "string",
                    "description": "The city and country, e.g. Paris, France"
                },
                "format": {
                    "type": "string",
                    "enum": ["celsius", "fahrenheit"],
                    "description": "The temperature unit to use. Infer this from the location."
                }
            },
            "required": ["location", "format"]
            }
        }
    }
    */
    procedure GetToolPrompt(): JsonObject
    var
        JsonObject: JsonObject;
    begin
        JsonObject.ReadFrom('{"type": "function","function": {"name": "get_current_weather","description": "Get the current weather","parameters": {"type": "object","properties": {"location": {"type": "string","description": "The city and country, e.g. Paris, France"},"format": {"type": "string","enum": ["celsius","fahrenheit"],"description": "The temperature unit to use. Infer this from the location."}},"required": ["location","format"]}}}');
        exit(JsonObject);
    end;
}
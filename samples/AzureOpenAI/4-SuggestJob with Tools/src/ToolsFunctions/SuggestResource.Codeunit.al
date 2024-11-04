codeunit 54398 "Suggest Resource" implements "AOAI Function"
{
    var
        FunctionNameLbl: Label 'suggest_resource', Locked = true;

    procedure GetPrompt(): JsonObject
    var
        ToolDefinition: JsonObject;
        FunctionDefinition: JsonObject;
        ParametersDefinition: JsonObject;
    begin
        ParametersDefinition.ReadFrom(
            '{"type": "object",' +
            '"properties": {' +
                '"resourceName": { "type": "string", "description": "The name of the resource"},' +
                '"resourceNumber": { "type": "string", "description": "The resource number"},' +
                '"resourceJobTitle": { "type": "string", "description": "Job title for the resource"},' +
                '"explanation": { "type": "string", "description": "A short explanation why this resource was suggested (max. 100 characters)."}' +
            '},"required": ["resourceName", "resourceNumber", "resourceJobTitle", "explanation"]}'
            );

        FunctionDefinition.Add('name', FunctionNameLbl);
        FunctionDefinition.Add('description', 'Call this function to suggest a resource for a task');
        FunctionDefinition.Add('parameters', ParametersDefinition);

        ToolDefinition.Add('type', 'function');
        ToolDefinition.Add('function', FunctionDefinition);

        exit(ToolDefinition);
    end;

    procedure Execute(Arguments: JsonObject): Variant
    var
        ResourceName: JsonToken;
        ResourceNumber: JsonToken;
        ResourceJobTitle: JsonToken;
        Explanation: JsonToken;
    begin
        Arguments.Get('resourceName', ResourceName);
        Arguments.Get('resourceNumber', ResourceNumber);
        Arguments.Get('resourceJobTitle', ResourceJobTitle);
        Arguments.Get('explanation', Explanation);

        TempResourceProposal.Init();
        TempResourceProposal.Name := ResourceName.AsValue().AsText();
        TempResourceProposal."No." := ResourceNumber.AsValue().AsText();
        TempResourceProposal."Job Title" := ResourceJobTitle.AsValue().AsText();
        TempResourceProposal.Explanation := Explanation.AsValue().AsText();
        TempResourceProposal.Insert();

        exit('Completed suggesting resources');
    end;

    procedure GetName(): Text
    begin
        exit(FunctionNameLbl);
    end;

    // NOTE: this is not part of the interface
    procedure GetResourceProposal(var LocalTempResourceProposal: Record "Resource Proposal" temporary)
    begin
        LocalTempResourceProposal.Copy(TempResourceProposal, true);
    end;

    var
        TempResourceProposal: Record "Resource Proposal" temporary;
}
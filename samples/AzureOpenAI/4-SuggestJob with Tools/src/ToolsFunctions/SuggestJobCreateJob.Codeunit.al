codeunit 54396 "SuggestJob - Create Job" implements "AOAI Function"
{
    var
        FunctionNameLbl: Label 'create_job', Locked = true;

    procedure GetPrompt(): JsonObject
    var
        ToolDefinition: JsonObject;
        FunctionDefinition: JsonObject;
        ParametersDefinition: JsonObject;
    begin
        ParametersDefinition.ReadFrom(
            '{"type": "object",' +
            '"properties": {' +
                '"desc": { "type": "string", "description": "A short description for this job (max 100 characters)."},' +
                '"customerName": { "type": "string", "description": "The name of the customer, if the user specified one, otherwise an empty string."}' +
            '},"required": ["desc", "customerName"]}'
            );

        FunctionDefinition.Add('name', FunctionNameLbl);
        FunctionDefinition.Add('description', 'Call this function to create a new job line (also called job task)');
        FunctionDefinition.Add('parameters', ParametersDefinition);

        ToolDefinition.Add('type', 'function');
        ToolDefinition.Add('function', FunctionDefinition);

        exit(ToolDefinition);
    end;

    procedure Execute(Arguments: JsonObject): Variant
    var
        Desc: JsonToken;
        CustomerName: JsonToken;
    begin
        Arguments.Get('desc', Desc);
        Arguments.Get('customerName', CustomerName);

        TempJob.DeleteAll();
        TempJob.Init();
        TempJob."Bill-to Name" := CustomerName.AsValue().AsText();
        TempJob.Description := Desc.AsValue().AsText();
        TempJob.Insert();
        exit('Completed creating job');
    end;

    procedure GetName(): Text
    begin
        exit(FunctionNameLbl);
    end;

    // NOTE: this is not part of the interface
    procedure GetJob(var LocalTempJob: Record Job temporary)
    begin
        LocalTempJob.Copy(TempJob, true);
    end;

    var
        TempJob: Record Job temporary;
}
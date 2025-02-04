codeunit 54395 "SuggestJob - Create Job Task" implements "AOAI Function"
{
    var
        FunctionNameLbl: Label 'create_job_task', Locked = true;

    procedure GetPrompt(): JsonObject
    var
        ToolDefinition: JsonObject;
        FunctionDefinition: JsonObject;
        ParametersDefinition: JsonObject;
    begin
        ParametersDefinition.ReadFrom(
            '{"type": "object",' +
            '"properties": {' +
                '"desc": { "type": "string", "description": "A short description for this task (max 100 characters)."},' +
                '"type": { "type": "string", "description": "resource if the task needs a resource, item if the task needs a physical item, both if it needs both"}' +
            '},"required": ["desc", "type"]}'
            );

        FunctionDefinition.Add('name', FunctionNameLbl);
        FunctionDefinition.Add('description', 'Call this function to create a new job (also called project)');
        FunctionDefinition.Add('parameters', ParametersDefinition);

        ToolDefinition.Add('type', 'function');
        ToolDefinition.Add('function', FunctionDefinition);

        exit(ToolDefinition);
    end;

    procedure Execute(Arguments: JsonObject): Variant
    var
        Desc: JsonToken;
        Type: JsonToken;
    begin
        Arguments.Get('desc', Desc);
        Arguments.Get('type', Type);

        TaskNumber += 10000;
        TempJobTask.Init();
        TempJobTask."Job Task No." := Format(TaskNumber);
        TempJobTask.Description := Desc.AsValue().AsText();
        TempJobTask."Job Task Type" := TempJobTask."Job Task Type"::Posting;
        TempJobTask.Insert();
        exit('Completed creating job task');
    end;

    procedure GetName(): Text
    begin
        exit(FunctionNameLbl);
    end;

    // NOTE: this is not part of the interface
    procedure GetJobTasks(var LocalTempJobTask: Record "Job Task" temporary)
    begin
        LocalTempJobTask.Copy(TempJobTask, true);
    end;

    var
        TempJobTask: Record "Job Task" temporary;
        TaskNumber: Integer;
}
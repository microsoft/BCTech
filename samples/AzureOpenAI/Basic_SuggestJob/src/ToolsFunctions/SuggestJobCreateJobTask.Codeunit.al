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
                '"startDate": { "type": "string", "format": "date", "description": "The estimated start date for the task"},' +
                '"endDate": { "type": "string", "format": "date", "description": "The estimated end date for the task"},' +
                '"type": { "type": "string", "description": "resource if the task needs a resource, item if the task needs a physical item, both if it needs both"}' +
            '},"required": ["desc", "startDate", "endDate", "type"]}'
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
        StartDate: JsonToken;
        EndDate: JsonToken;
        Type: JsonToken;
    begin
        Arguments.Get('desc', Desc);
        Arguments.Get('startDate', StartDate);
        Arguments.Get('endDate', EndDate);
        Arguments.Get('type', Type);

        TaskNumber += 10000;
        TempJobTask.Init();
        TempJobTask."Job Task No." := Format(TaskNumber);
        TempJobTask.Description := Desc.AsValue().AsText();
        TempJobTask."Job Task Type" := TempJobTask."Job Task Type"::Posting;
        TempJobTask."Start Date" := StartDate.AsValue().AsDate();
        TempJobTask."End Date" := EndDate.AsValue().AsDate();
        TempJobTask.Insert();
    end;

    procedure GetName(): Text
    begin
        exit(FunctionNameLbl);
    end;

    procedure GetJobTasks(var LocalTempJobTask: Record "Job Task" temporary)
    begin
        LocalTempJobTask.Copy(TempJobTask, true);
    end;

    var
        TempJobTask: Record "Job Task" temporary;
        TaskNumber: Integer;
}
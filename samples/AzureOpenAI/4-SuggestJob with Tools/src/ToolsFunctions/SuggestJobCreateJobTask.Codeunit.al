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
                '"subtasks": { "type": "array", "description": "Subtasks (jobs) needed to complete the overall task, with the role description keywords that would be best for it (max. 100 characters, keywords)",' +
                '"items": { "type": "object", properties: {"desc": { "type": "string", "description": "A short description for this subtask (max 100 characters)."}, "roleDesc": { "type": "string", "description": "Role description keywords that would be best for this subtask (max 100 characters, keywords)"} }' +
                '} },' +
                '"type": { "type": "string", "description": "resource if the task needs a resource, item if the task needs a physical item, both if it needs both"}' +
            '},"required": ["desc", "type", "subtasks"]}'
            );

        FunctionDefinition.Add('name', FunctionNameLbl);
        FunctionDefinition.Add('description', 'Call this function to create a new job line (also called job task) and create subtasks for it. Those subtasks are jobs that are needed to complete the job task.');
        FunctionDefinition.Add('parameters', ParametersDefinition);

        ToolDefinition.Add('type', 'function');
        ToolDefinition.Add('function', FunctionDefinition);

        exit(ToolDefinition);
    end;

    procedure Execute(Arguments: JsonObject): Variant
    var
        JobPlannigLineNo: Integer;
        ActionDescription: JsonToken;
        RoleDescription: JsonToken;
        Desc: JsonToken;
        Type: JsonToken;
        Subtasks: JsonToken;
        Subtask: JsonToken;
        SubtaskObject: JsonObject;
    begin
        Arguments.Get('desc', Desc);
        Arguments.Get('type', Type);
        Arguments.Get('subtasks', Subtasks);

        TaskNumber += 10000;
        TempJobTask.Init();
        TempJobTask."Job Task No." := Format(TaskNumber);
        TempJobTask.Description := Desc.AsValue().AsText();
        TempJobTask."Job Task Type" := TempJobTask."Job Task Type"::Posting;
        TempJobTask.Insert();

        JobPlannigLineNo := TaskNumber + 1000;
        foreach Subtask in Subtasks.AsArray() do begin
            TempJobTaskLines.Init();
            TempJobTaskLines."Job Task No." := TempJobTask."Job Task No.";
            TempJobTaskLines."Job No." := TempJobTask."Job No.";
            TempJobTaskLines."Line No." := JobPlannigLineNo;

            SubtaskObject := Subtask.AsObject();
            SubtaskObject.Get('desc', ActionDescription);
            SubtaskObject.Get('roleDesc', RoleDescription);

            TempJobTaskLines."Description" := CopyStr(ActionDescription.AsValue().AsText(), 1, MaxStrLen(TempJobTaskLines.Description));
            TempJobTaskLines."Description 2" := CopyStr(RoleDescription.AsValue().AsText(), 1, MaxStrLen(TempJobTaskLines."Description 2"));

            TempJobTaskLines.Insert();
            JobPlannigLineNo += 1000;
        end;

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

    procedure GetJobTaskLines(var LocalTempJobTaskLines: Record "Job Planning Line" temporary)
    begin
        LocalTempJobTaskLines.Copy(TempJobTaskLines, true);
    end;

    var
        TempJobTask: Record "Job Task" temporary;

        TempJobTaskLines: Record "Job Planning Line" temporary;
        TaskNumber: Integer;
}
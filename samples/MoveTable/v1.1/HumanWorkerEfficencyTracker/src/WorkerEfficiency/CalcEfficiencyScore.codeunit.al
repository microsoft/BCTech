#pragma warning disable AL0801
//Table 'Task Entry' is marked to be moved. Reason: moved to separate Task Tracker app.. Tag: 1.1.0.0.
codeunit 50100 "Calc. Efficiency Score"
{
    Access = Internal;

    [EventSubscriber(ObjectType::Table, Database::"Task Entry", 'OnAfterValidateEvent', 'TaskCode', true, true)]
    local procedure SetExpectedDurationOnAfterValidateTaskCode(var Rec: Record "Task Entry")
    var
        Task: Record "Task";
    begin
        Clear(Rec."Start Datetime");
        Clear(Rec."End Datetime");
        Clear(Rec."Expected Duration");
        Clear(Rec."Efficiency Score");
        if Task.Get(Rec.TaskCode) then
            Rec."Expected Duration" := Task."Expected Duration";
    end;

    [EventSubscriber(ObjectType::Table, Database::"Task Entry", 'OnAfterValidateEvent', 'Start Datetime', true, true)]
    local procedure CalculateEfficiencyScoreOnAfterValidateStartDatetime(var Rec: Record "Task Entry")
    begin
        CalculateEfficiencyScore(Rec)
    end;

    [EventSubscriber(ObjectType::Table, Database::"Task Entry", 'OnAfterValidateEvent', 'End Datetime', true, true)]
    local procedure CalculateEfficiencyScoreOnAfterValidateEndDatetime(var Rec: Record "Task Entry")
    begin
        CalculateEfficiencyScore(Rec)
    end;

    local procedure CalculateEfficiencyScore(var TaskEntry: Record "Task Entry")
    begin
        if (TaskEntry."Start Datetime" <> 0DT) and (TaskEntry."End Datetime" <> 0DT) and (TaskEntry."Expected Duration" <> 0) and (TaskEntry."Start Datetime" <> TaskEntry."End Datetime") then begin
            TaskEntry."Actual Duration" := TaskEntry."End Datetime" - TaskEntry."Start Datetime";
            TaskEntry."Efficiency Score" := Round(TaskEntry."Expected Duration" / TaskEntry."Actual Duration" * 100, 1);
        end else begin
            Clear(TaskEntry."Actual Duration");
            Clear(TaskEntry."Efficiency Score");
        end;
    end;
}
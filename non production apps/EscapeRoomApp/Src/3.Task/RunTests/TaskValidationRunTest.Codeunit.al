codeunit 73921 "Task Validation Run Test"
{
    procedure RunTest(Task: Interface iEscapeRoomTask): Boolean
    var
        TaskRec: Record "Escape Room Task";
        TestQueue: Record "Test Queue";
    begin
        TaskRec := task.GetTaskRec();
        TestQueue.Get(TaskRec.TestCodeunitId);

        Codeunit.Run(codeunit::"Task Validation Test Runner", TestQueue);

        SelectLatestVersion(database::"Test Queue");
        TestQueue.Get(TaskRec.TestCodeunitId);
        exit(TestQueue.Success);
    end;
}
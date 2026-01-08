codeunit 73920 "Task Validation Test Runner"
{
    Subtype = TestRunner;
    TestIsolation = Codeunit;
    TableNo = "Test Queue";

    trigger OnRun()
    begin
        codeunit.run(Rec."Codeunit Id");
    end;

    trigger OnAfterTestRun(CodeunitId: Integer; CodeunitName: Text; FunctionName: Text; Permissions: TestPermissions; Success: Boolean)
    var
        TestQueue: Record "Test Queue";
    begin
        if FunctionName <> '' then exit; //Only full codeunit tests are supported

        TestQueue.ReadIsolation := IsolationLevel::UpdLock;
        TestQueue.Get(CodeunitId);
        TestQueue.Success := Success;
        TestQueue.Modify();
        Commit;
    end;
}
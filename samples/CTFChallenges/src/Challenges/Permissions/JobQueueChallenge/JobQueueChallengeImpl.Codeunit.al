codeunit 50149 JobQueueChallenge
{
    trigger OnRun()
    begin
        TableToInsert.Insert();
    end;

    var
        myInt: Integer;
        TableToInsert: Record TableToInsert;
}
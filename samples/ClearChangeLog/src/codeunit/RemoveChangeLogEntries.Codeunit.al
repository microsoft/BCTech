codeunit 50100 "BCTech_RemoveChangeLogEntries"
{

    Permissions = tabledata "Change Log Entry" = rimd,
             tabledata "Change Log Setup" = rimd;

    trigger OnRun()
    begin
        RemoveChangeLogEntries();
    end;

    internal procedure RemoveChangeLogEntries()
    var
        ChangeLogEntry: Record "Change Log Entry";
        ChangeLogSetup: Record "Change Log Setup";
        ChangeLogFrom: BigInteger;
        ChangeLogTo: BigInteger;
    begin
        if not ChangeLogSetup.Get() then
            exit;
        if ChangeLogSetup."Change Log Activated" then begin
            ChangeLogSetup.Validate("Change Log Activated", false);
            ChangeLogSetup.Modify(true);
            Commit();
        end;

        ChangeLogEntry.Reset();
        if not ChangeLogEntry.FindFirst() then
            exit;
        ChangeLogFrom := ChangeLogEntry."Entry No.";
        ChangeLogTo := ChangeLogFrom + 100000;
        if DeleteEntriesFromUntil(ChangeLogFrom, ChangeLogTo) then
            TaskScheduler.CreateTask(Codeunit::"BCTech_RemoveChangeLogEntries", 0, true, CompanyName);
    end;

    local procedure DeleteEntriesFromUntil(ChangeLogFrom: BigInteger; ChangeLogTo: BigInteger): Boolean
    var
        ChangeLogEntry: Record "Change Log Entry";
    begin

        ChangeLogEntry.Reset();
        ChangeLogEntry.SetRange("Entry No.", ChangeLogFrom, ChangeLogTo);
        if not ChangeLogEntry.FindSet(true) then
            exit;
        ChangeLogEntry.DeleteAll(true);
        Commit();
        ChangeLogEntry.Reset();
        exit(ChangeLogEntry.FindFirst());
    end;

}
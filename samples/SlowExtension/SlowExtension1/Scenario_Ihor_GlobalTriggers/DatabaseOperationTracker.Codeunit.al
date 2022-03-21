codeunit 50100 "Database Operation Tracker"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Global Triggers", 'GetDatabaseTableTriggerSetup', '', false, false)]
    local procedure GetDatabaseTableTriggerSetup(TableId: Integer; var OnDatabaseInsert: Boolean; var OnDatabaseModify: Boolean; var OnDatabaseDelete: Boolean; var OnDatabaseRename: Boolean)
    begin
        OnDatabaseInsert := true;
        OnDatabaseModify := true;
        OnDatabaseDelete := true;
        OnDatabaseRename := true;
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Global Triggers", 'OnDatabaseInsert', '', false, false)]
    local procedure OnDatabaseInsert(RecRef: RecordRef)
    begin
        UpdateDatabaseOperationStatistics(RecRef, Enum::"Database Operation"::Insert);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Global Triggers", 'OnDatabaseModify', '', false, false)]
    local procedure OnDatabaseModify(RecRef: RecordRef)
    begin
        UpdateDatabaseOperationStatistics(RecRef, Enum::"Database Operation"::Modify);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Global Triggers", 'OnDatabaseRename', '', false, false)]
    local procedure OnDatabaseRename(RecRef: RecordRef)
    begin
        UpdateDatabaseOperationStatistics(RecRef, Enum::"Database Operation"::Modify);
    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Global Triggers", 'OnDatabaseDelete', '', false, false)]
    local procedure OnDatabaseDelete(RecRef: RecordRef)
    begin
        UpdateDatabaseOperationStatistics(RecRef, Enum::"Database Operation"::Delete);
    end;

    local procedure UpdateDatabaseOperationStatistics(RecRef: RecordRef; DatabaseOperation: Enum "Database Operation")
    var
        DatabaseOperationStatistics: Record "Database Operation Statistics";
        TableID: Integer;
    begin
        TableID := RecRef.RecordId.TableNo();
        if TableID = Database::"Database Operation Statistics" then
            exit;

        DatabaseOperationStatistics.LockTable();
        if DatabaseOperationStatistics.Get(TableID) then begin
            IncrementDatabaseOperationCounter(DatabaseOperationStatistics, DatabaseOperation);
            DatabaseOperationStatistics.Modify();
        end else begin
            DatabaseOperationStatistics."Table ID" := TableID;
            IncrementDatabaseOperationCounter(DatabaseOperationStatistics, DatabaseOperation);
            DatabaseOperationStatistics.Insert();
        end;
    end;

    local procedure IncrementDatabaseOperationCounter(var DatabaseOperationStatistics: Record "Database Operation Statistics"; DatabaseOperation: Enum "Database Operation")
    begin
        case DatabaseOperation of
            DatabaseOperation::Insert:
                DatabaseOperationStatistics.Inserts += 1;
            DatabaseOperation::Modify:
                DatabaseOperationStatistics.Modifies += 1;
            DatabaseOperation::Delete:
                DatabaseOperationStatistics.Deletes += 1;
        end;
    end;
}
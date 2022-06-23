codeunit 50311 "MSFT - Transform Data"
{
    trigger OnRun()
    begin

    end;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Data Migration Mgt.", 'OnAfterMigrationFinished', '', false, false)]
    local procedure TransformCustomData()
    begin
        // Transactions - Code to post from batch
        // Master data - Direct transformations
    end;
}
codeunit 50131 ImplicitWith
{
    TableNo = Customer;

    trigger OnRun()
    begin
        SetRange("No.", '10000', '20000');
        if Find() then
            repeat
            until Next() = 0;

        if IsDirty() then
            Message('Something is not clean');
    end;

    procedure IsDirty(): Boolean;
    begin
        exit(false);
    end;
}

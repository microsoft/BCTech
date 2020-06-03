codeunit 50138 ExplicitWith
{
    procedure ProcessCustomer(Name: Text)
    var
        Customer: Record Customer;
    begin
        with Customer do begin
            Name := UpperCase(Name);

            if IsDirty() then Insert();
        end;
    end;

    local procedure IsDirty(): Boolean;
    begin
        exit(true);
    end;
}

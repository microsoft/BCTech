codeunit 50107 "Contoso Shoes Posted Sales"
{
    trigger OnRun()
    var
        SalesHeader: Record "Sales Header";
        ContosoShoesSalesOrder: Codeunit "Contoso Shoes Sales Order";
    begin
        SalesHeader.SetRange("Document Type", SalesHeader."Document Type"::Order);
        SalesHeader.SetRange("Your Reference", ContosoShoesSalesOrder.PostYourReference());
        if SalesHeader.Findset() then
            repeat
                SalesHeader.Validate(Ship, true);
                SalesHeader.Validate(Invoice, true);
                CODEUNIT.Run(CODEUNIT::"Sales-Post", SalesHeader);
            until SalesHeader.Next() = 0;
    end;
}
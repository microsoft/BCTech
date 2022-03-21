codeunit 50123 AddLotsOfCustomers
{
    trigger OnRun()
    begin
        CreateCustomers(2000);
    end;

    procedure CreateCustomers(numbersToRun: Integer)
    var
        CustTemplate: Record Customer;
        Customer: Record Customer;
        I: Integer;
        NextNumber: Code[10];
        NoSeriesManagement: Codeunit NoSeriesManagement;
        BalanceLCI: Decimal;
    begin
        CustTemplate.GET('10000');
        NextNumber := NoSeriesManagement.GetNextNo(PrefCustNoSeries, WORKDATE, FALSE);

        FOR I := 1 TO numbersToRun DO BEGIN
            if I mod 500 = 0 then
                BalanceLCI := RANDOM(1000)
            else
                BalanceLCI := 0;

            Customer.INIT;
            Customer."No." := NextNumber;
            NextNumber := INCSTR(NextNumber);
            Customer.INSERT(TRUE);
            if (BalanceLCI > 0) then
                Customer."Balance (LCY)" := BalanceLCI;

            Customer."No. Series" := PrefCustNoSeries;
            Customer.VALIDATE(Name, FORMAT(RANDOM(10000)));
            Customer.VALIDATE(Address, FORMAT(RANDOM(10000)));
            Customer.VALIDATE(City, FORMAT(RANDOM(10000)));
            Customer.VALIDATE("Address 2", FORMAT(RANDOM(10000)));
            Customer.VALIDATE("VAT Registration No.", '012345678'); //VAT Regno. overhead
            Customer."VAT Registration No." := '';
            Customer.VALIDATE("Gen. Bus. Posting Group", CustTemplate."Gen. Bus. Posting Group");
            Customer.VALIDATE("Customer Posting Group", CustTemplate."Customer Posting Group");
            Customer.MODIFY(TRUE);
        END;
        Commit();
    end;

    var
        PrefCustNoSeries: Label 'CUST';

}
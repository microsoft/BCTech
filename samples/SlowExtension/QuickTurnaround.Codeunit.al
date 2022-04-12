// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license inFormation.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50103 "Quick Turnaround"
{
    Access = Internal;

    procedure EasyComeEasyGo()
    begin
        AddCustomers(200);
        RemoveCustomers();
    end;

    local procedure AddCustomers(Number: Integer)
    var
        Customer: Record Customer;
        NoSeriesManagement: Codeunit NoSeriesManagement;
        Iterator: Integer;
        NextNumber: Code[10];
    begin
        NextNumber := NoSeriesManagement.GetNextNo(CustNoSeriesCode, WorkDate(), false);

        for Iterator := 1 to Number do begin
            Customer.Init();
            Customer."No." := NextNumber;
            NextNumber := IncStr(NextNumber);
            Customer.Insert(true);

            Customer."No. Series" := CustNoSeriesCode;
            Customer.Validate(Name, QuickTurnaroundCustomer);
            Customer.Validate(Address, Format(Random(10000)));
            Customer.Validate(City, Format(Random(10000)));
            Customer.Validate("Address 2", Format(Random(10000)));
            Customer."VAT Registration No." := '';
            Customer.Modify(true);
        end;
    end;

    local procedure RemoveCustomers()
    var
        Customer: Record Customer;
    begin
        Customer.SetRange(Name, QuickTurnaroundCustomer);
        Customer.DeleteAll();
    end;

    var
        CustNoSeriesCode: Label 'CUST';
        QuickTurnaroundCustomer: Label 'Quick Turnaround Customer';
}
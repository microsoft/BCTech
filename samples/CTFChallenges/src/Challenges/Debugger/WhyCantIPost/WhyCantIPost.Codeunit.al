codeunit 50140 WhyCantIPost implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge();
    var
        SalesHeader: record "Sales Header";
        ScenarioLabel: Label 'Try selling an ANTWERP desk to ''The Cannon Group PLC'' using the ''Post and Send'' action.\The challenge is to figure out what is going on. What is blocking posting of this sales order?';
    begin
        SalesHeader.SetRange("Document Type", "Sales Document Type"::Order);
        SalesHeader.SetFilter(SalesHeader."Sell-to Customer No.", '10000');

        SalesHeader.SetCurrentKey(SalesHeader."Amount Including VAT");
        SalesHeader.SetAscending(SalesHeader."Amount Including VAT", false);

        Page.Run(Page::"Sales Order List", SalesHeader);
        Message(ScenarioLabel);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Use the event recorder while trying to execute the action ''Post and Send''. Any suspicious event (OnCodeOnBeforePostSalesHeader)?';
        HintLine2: Label '\Create an extension that extends the "Sales Order List" card ';
        HintLine3: Label '\Try putting a breakpoint on the call for the event and F11 until you reach the result.';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2 + HintLine3);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Debugging);
    end;
}
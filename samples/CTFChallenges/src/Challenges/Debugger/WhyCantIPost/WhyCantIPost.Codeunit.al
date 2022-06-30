codeunit 50140 WhyCantIPost implements "CTF Challenge"
{
    Access = Internal;
    
    procedure RunChallenge();
    var
     SalesHeader: record "Sales Header";      
     ScenarioLabel: Label 'Try selling an ANTWERP desk to ''The Cannon Group PLC''.\The challenge is to figure out what is going on. What is blocking posting of this sales order?'; 
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
     HintLine1: Label 'Use your ''Hello World'' app to start debugging. Don''t forget to add a dependency to the ''CTF Challenges'' app. And download symbols.';
     HintLine2: Label '\Try finding an entry point. In order to do that you can:\Use the page inspector on the Help and Support Page to find the entry page.';
     HintLine3: Label '\You can ''gotodefinition'' to that page. Use any technique at hand. Find the suspicious action and put a breakpoint there';
     HintLine4: Label '\Or you can use the event recorder while you are running your scenario to record all events and put a breakpoint in the ones that are suspicious.';
     Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2 + HintLine3 + HintLine4);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Debugging);
    end;
}
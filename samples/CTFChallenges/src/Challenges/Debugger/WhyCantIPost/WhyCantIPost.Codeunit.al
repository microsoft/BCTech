codeunit 50140 WhyCantIPost implements "CTF Challenge"
{
    Access = Internal;

    trigger OnRun()
    begin       
    end;

    [EventSubscriber(ObjectType::Codeunit, codeunit::"Sales-Post and Send", 'OnBeforePostAndSend', '', false, false)]
    local procedure OnBeforePostAndSend(var SalesHeader: Record "Sales Header"; var HideDialog: Boolean; var TempDocumentSendingProfile: Record "Document Sending Profile" temporary)
    begin
        if (SalesHeader.Amount >= 120000) then 
            HideDialog := true;
    end;
    
    [EventSubscriber(ObjectType::Codeunit, codeunit::"Sales-Post and Send", 'OnCodeOnBeforePostSalesHeader', '', false, false)]
    local procedure OnCodeOnBeforePostSalesHeader(var SalesHeader: Record "Sales Header"; var TempDocumentSendingProfile: Record "Document Sending Profile" temporary; var IsHandled: Boolean)
    begin
         if (SalesHeader.Amount >= 120000) then 
            IsHandled:= true;
    end;

    procedure RunChallenge();
    var
     SalesHeader: record "Sales Header";      
     ScenarioLabel: Label 'Try posting a sales order where the Amount is greater then 120.000.\The challenge is to figure out what is going on. What is blocking posting...'; 
    begin            
        SalesHeader.SetRange(Status, "Sales Document Status"::Open);
        SalesHeader.SetRange("Document Type", "Sales Document Type"::Order); 
        SalesHeader.SetFilter(Amount, '>120000');
        SalesHeader.SetCurrentKey(SalesHeader."Amount Including VAT");   
        SalesHeader.SetAscending(SalesHeader."Amount Including VAT", false);            
        Page.Run(Page::"Sales Order List", SalesHeader);
        Message(ScenarioLabel);
    end;

    procedure GetHints(): List of [Text];
    var
     HintLine1: Label 'Try finding an entry point. In order to do that you can:\Use the page inspector on the Help and Support Page to find the entry page.';
     HintLine2: Label '\You can ''gotodefinition'' to that page. Use any technique at hand. Find the suspicious action and put a breakpoint there';
     HintLine3: Label '\Or you can use the event recorder while you are running your scenario to record all events and put a breakpoint in the ones that are suspicious.';
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
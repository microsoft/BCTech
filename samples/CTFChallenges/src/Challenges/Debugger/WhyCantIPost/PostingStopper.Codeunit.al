codeunit 50146 PostingStopper
{
    Access = Internal;

    [EventSubscriber(ObjectType::Codeunit, codeunit::"Sales-Post and Send", 'OnBeforePostAndSend', '', false, false)]
    local procedure OnBeforePostAndSend(var SalesHeader: Record "Sales Header"; var HideDialog: Boolean; var TempDocumentSendingProfile: Record "Document Sending Profile" temporary)
    begin
        if (SalesHeader."Sell-to Customer No." = '10000') then
            HideDialog := true;
    end;

    [EventSubscriber(ObjectType::Codeunit, codeunit::"Sales-Post and Send", 'OnCodeOnBeforePostSalesHeader', '', false, false)]
    local procedure Flag_f5509017(var SalesHeader: Record "Sales Header"; var TempDocumentSendingProfile: Record "Document Sending Profile" temporary; var IsHandled: Boolean)
    begin
        if (SalesHeader."Sell-to Customer No." = '10000') then
            IsHandled := true;
    end;
}
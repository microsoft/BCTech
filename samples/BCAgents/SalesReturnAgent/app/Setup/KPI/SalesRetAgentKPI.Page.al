// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.KPI;

page 53701 "Sales Ret. Agent KPI"
{
    PageType = CardPart;
    ApplicationArea = All;
    Caption = 'Sales Return Agent Summary';
    SourceTable = "Sales Ret. Agent KPI";
    Editable = false;
    Extensible = false;
    InherentEntitlements = X;
    InherentPermissions = X;

    layout
    {
        area(Content)
        {
            cuegroup(KeyMetrics)
            {
                Caption = 'Key Performance Indicators';

                field(CreditMemosCreated; Rec."Credit Memos Created")
                {
                    Caption = 'Credit Memos Created';
                    ToolTip = 'Specifies the number of sales credit memos created by the agent.';
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        GetRelevantAgent();
    end;

    /// <summary>
    /// Retrieves the relevant agent's KPI record for display.
    /// This page is launched via IAgentMetadata.GetSummaryPageId(). The platform sets a filter on the
    /// "User Security ID" field before opening the page, so the source record may not be fully populated
    /// on open - the filter is evaluated here to resolve and load the correct record.
    /// </summary>
    local procedure GetRelevantAgent()
    var
        UserSecurityIDFilter: Text;
    begin
        if IsNullGuid(Rec."User Security ID") then begin
            UserSecurityIDFilter := Rec.GetFilter("User Security ID");
            if not Evaluate(Rec."User Security ID", UserSecurityIDFilter) then
                Error(AgentDoesNotExistErr);
        end;

        if not Rec.Get(Rec."User Security ID") then
            Rec.Insert();
    end;

    var
        AgentDoesNotExistErr: Label 'The agent does not exist. Please check the configuration.';
}

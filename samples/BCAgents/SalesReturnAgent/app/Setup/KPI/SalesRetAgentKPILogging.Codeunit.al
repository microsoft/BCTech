// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.KPI;

using Microsoft.Sales.Document;
using SalesReturnAgent.Setup;
using System.Agents;

/// <summary>
/// Subscribes to business events to automatically log KPI metrics
/// for the Sales Return Agent.
///
/// - Credit Memos Created: incremented each time the agent creates a sales credit memo.
/// </summary>
codeunit 53704 "Sales Ret. Agent KPI Logging"
{
    Access = Internal;
    EventSubscriberInstance = StaticAutomatic;
    SingleInstance = true;
    InherentEntitlements = X;
    InherentPermissions = X;

    [EventSubscriber(ObjectType::Table, Database::"Sales Header", OnAfterInsertEvent, '', false, false)]
    local procedure OnAfterInsertSalesHeader(var Rec: Record "Sales Header"; RunTrigger: Boolean)
    var
        AgentSession: Codeunit "Agent Session";
        SalesRetAgentSetup: Codeunit "Sales Ret. Agent Setup";
        AgentUserSecurityId: Guid;
        AgentMetadataProvider: Enum "Agent Metadata Provider";
    begin
        if not AgentSession.IsAgentSession(AgentMetadataProvider) then
            exit;

        if AgentMetadataProvider <> Enum::"Agent Metadata Provider"::"Sales Return Agent" then
            exit;

        if Rec."Document Type" <> Rec."Document Type"::"Credit Memo" then
            exit;

        if not SalesRetAgentSetup.TryGetAgent(AgentUserSecurityId) then
            exit;

        if UserSecurityId() <> AgentUserSecurityId then
            exit;

        UpdateKPI(AgentUserSecurityId, 1);
    end;

    local procedure UpdateKPI(AgentUserSecurityId: Guid; CreatedIncrement: Integer)
    var
        SalesRetAgentKPI: Record "Sales Ret. Agent KPI";
    begin
        if not SalesRetAgentKPI.Get(AgentUserSecurityId) then begin
            SalesRetAgentKPI.Init();
            SalesRetAgentKPI."User Security ID" := AgentUserSecurityId;
            SalesRetAgentKPI.Insert();
        end;

        SalesRetAgentKPI."Credit Memos Created" += CreatedIncrement;
        SalesRetAgentKPI.Modify();
    end;
}

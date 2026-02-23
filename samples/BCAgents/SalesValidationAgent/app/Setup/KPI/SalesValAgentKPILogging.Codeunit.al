// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup.KPI;

using Microsoft.Sales.Document;
using SalesValidationAgent.Setup;
using System.Agents;

/// <summary>
/// Subscribes to business events to automatically log KPI metrics
/// for the Sales Validation Agent.
///
/// - Orders Released: incremented each time the agent releases a sales order.
/// </summary>
codeunit 50106 "Sales Val. Agent KPI Logging"
{
    Access = Internal;
    EventSubscriberInstance = StaticAutomatic;
    SingleInstance = true;

    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Release Sales Document", OnAfterReleaseSalesDoc, '', false, false)]
    local procedure OnAfterReleaseSalesDoc(var SalesHeader: Record "Sales Header")
    var
        AgentSession: Codeunit "Agent Session";
        SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
        AgentUserSecurityId: Guid;
        AgentMetadataProvider: Enum "Agent Metadata Provider";
    begin
        if not AgentSession.IsAgentSession(AgentMetadataProvider) then
            exit;

        if AgentMetadataProvider <> Enum::"Agent Metadata Provider"::"Sales Validation Agent" then
            exit;

        // Only count sales orders (not quotes, invoices, etc.)
        if SalesHeader."Document Type" <> SalesHeader."Document Type"::Order then
            exit;

        if not SalesValAgentSetup.TryGetAgent(AgentUserSecurityId) then
            exit;

        if UserSecurityId() <> AgentUserSecurityId then
            exit;

        UpdateKPI(AgentUserSecurityId, 1);
    end;

    local procedure UpdateKPI(AgentUserSecurityId: Guid; ReleasedIncrement: Integer)
    var
        SalesValAgentKPI: Record "Sales Val. Agent KPI";
    begin
        if not SalesValAgentKPI.Get(AgentUserSecurityId) then begin
            SalesValAgentKPI.Init();
            SalesValAgentKPI."User Security ID" := AgentUserSecurityId;
            SalesValAgentKPI.Insert();
        end;

        SalesValAgentKPI."Orders Released" += ReleasedIncrement;
        SalesValAgentKPI.Modify();
    end;
}

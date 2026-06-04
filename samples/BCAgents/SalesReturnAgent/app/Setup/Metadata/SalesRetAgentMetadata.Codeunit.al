// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Metadata;

using SalesReturnAgent.Setup;
using System.Agents;

codeunit 53701 SalesRetAgentMetadata implements IAgentMetadata
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure GetInitials(AgentUserId: Guid): Text[4]
    begin
        exit(SalesRetAgentSetup.GetInitials());
    end;

    procedure GetSetupPageId(AgentUserId: Guid): Integer
    begin
        exit(SalesRetAgentSetup.GetSetupPageId());
    end;

    procedure GetSummaryPageId(AgentUserId: Guid): Integer
    begin
        exit(SalesRetAgentSetup.GetSummaryPageId());
    end;

    procedure GetAgentTaskMessagePageId(AgentUserId: Guid; MessageId: Guid): Integer
    begin
        exit(Page::"Agent Task Message Card");
    end;

    procedure GetAgentAnnotations(AgentUserId: Guid; var Annotations: Record "Agent Annotation")
    begin
        Clear(Annotations);
    end;

    var
        SalesRetAgentSetup: Codeunit "Sales Ret. Agent Setup";
}

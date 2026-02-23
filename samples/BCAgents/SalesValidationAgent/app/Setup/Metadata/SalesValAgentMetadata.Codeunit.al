// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup.Metadata;

using SalesValidationAgent.Setup;
using System.Agents;

codeunit 50102 SalesValAgentMetadata implements IAgentMetadata
{
    Access = Internal;

    procedure GetInitials(AgentUserId: Guid): Text[4]
    begin
        exit(SalesValAgentSetup.GetInitials());
    end;

    procedure GetSetupPageId(AgentUserId: Guid): Integer
    begin
        exit(SalesValAgentSetup.GetSetupPageId());
    end;

    procedure GetSummaryPageId(AgentUserId: Guid): Integer
    begin
        exit(SalesValAgentSetup.GetSummaryPageId());
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
        SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
}
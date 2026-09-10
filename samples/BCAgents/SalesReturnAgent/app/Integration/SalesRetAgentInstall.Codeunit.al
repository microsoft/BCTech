// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Integration;

using SalesReturnAgent.Setup;
using System.Agents;
using System.AI;

codeunit 53702 "Sales Ret. Agent Install"
{
    Subtype = Install;
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnInstallAppPerDatabase()
    var
        SalesRetAgentSetupRec: Record "Sales Ret. Agent Setup";
    begin
        RegisterCapability();

        if not SalesRetAgentSetupRec.FindSet() then
            exit;

        repeat
            InstallAgent(SalesRetAgentSetupRec);
        until SalesRetAgentSetupRec.Next() = 0;
    end;

    local procedure InstallAgent(var SalesRetAgentSetupRec: Record "Sales Ret. Agent Setup")
    begin
        InstallAgentInstructions(SalesRetAgentSetupRec);
    end;

    local procedure InstallAgentInstructions(var SalesRetAgentSetupRec: Record "Sales Ret. Agent Setup")
    var
        Agent: Codeunit Agent;
        SalesRetAgentSetup: Codeunit "Sales Ret. Agent Setup";
    begin
        Agent.SetInstructions(SalesRetAgentSetupRec."User Security ID", SalesRetAgentSetup.GetInstructions());
    end;

    [EventSubscriber(ObjectType::Page, Page::"Copilot AI Capabilities", 'OnRegisterCopilotCapability', '', false, false)]
    local procedure OnRegisterCopilotCapability()
    begin
        RegisterCapability();
    end;

    local procedure RegisterCapability()
    var
        CopilotCapability: Codeunit "Copilot Capability";
        LearnMoreUrlTok: Label 'https://go.microsoft.com/fwlink/?linkid=2350506', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Sales Return Agent") then
            CopilotCapability.RegisterCapability(
            Enum::"Copilot Capability"::"Sales Return Agent",
            Enum::"Copilot Availability"::Preview,
            "Copilot Billing Type"::"Microsoft Billed",
            LearnMoreUrlTok);
    end;
}

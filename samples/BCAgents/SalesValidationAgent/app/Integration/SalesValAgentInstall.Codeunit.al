// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Integration;

using SalesValidationAgent.Setup;
using System.Agents;
using System.AI;
using System.Security.AccessControl;

codeunit 50101 "Sales Val. Agent Install"
{
    Subtype = Install;
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnInstallAppPerDatabase()
    var
        SalesValAgentSetupRec: Record "Sales Val. Agent Setup";
    begin
        RegisterCapability();

        if not SalesValAgentSetupRec.FindSet() then
            exit;

        repeat
            InstallAgent(SalesValAgentSetupRec);
        until SalesValAgentSetupRec.Next() = 0;
    end;

    local procedure InstallAgent(var SalesValAgentSetupRec: Record "Sales Val. Agent Setup")
    begin
        InstallAgentInstructions(SalesValAgentSetupRec);
    end;

    local procedure InstallAgentInstructions(var SalesValAgentSetupRec: Record "Sales Val. Agent Setup")
    var
        Agent: Codeunit Agent;
        SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
    begin
        Agent.SetInstructions(SalesValAgentSetupRec."User Security ID", SalesValAgentSetup.GetInstructions());
    end;

    local procedure RegisterCapability()
    var
        CopilotCapability: Codeunit "Copilot Capability";
        LearnMoreUrlTxt: Label 'https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/ai/ai-development-toolkit-sales-validation', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Sales Validation Agent") then
            CopilotCapability.RegisterCapability(
            Enum::"Copilot Capability"::"Sales Validation Agent",
            Enum::"Copilot Availability"::Preview,
            "Copilot Billing Type"::"Microsoft Billed",
            LearnMoreUrlTxt);
    end;
}
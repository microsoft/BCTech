// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using System.AI;

/// <summary>
/// Install codeunit for the Order Copilot extension.
/// </summary>
codeunit 50104 "Order Copilot Install"
{
    Subtype = Install;
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnInstallAppPerDatabase()
    begin
        RegisterCapability();
    end;

    internal procedure RegisterCapability()
    var
        CopilotCapability: Codeunit "Copilot Capability";
        LearnMoreUrlTxt: Label 'https://mycapability/learnmore', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Order Copilot") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Order Copilot", Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);
    end;

    [EventSubscriber(ObjectType::Page, Page::"Copilot AI Capabilities", 'OnRegisterCopilotCapability', '', false, false)]
    local procedure OnRegisterCopilotCapability()
    begin
        RegisterCapability();
    end;
}
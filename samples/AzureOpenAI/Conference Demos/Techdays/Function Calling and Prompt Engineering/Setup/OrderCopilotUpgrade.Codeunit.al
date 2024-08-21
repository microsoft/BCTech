// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using System.Upgrade;

/// <summary>
/// Upgrade codeunit for the Order Copilot extension.
/// </summary>
codeunit 50105 "Order Copilot Upgrade"
{
    Subtype = Upgrade;
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnUpgradePerDatabase()
    begin
        RegisterCapability();
    end;

    local procedure RegisterCapability()
    var
        OrderCopilotInstall: Codeunit "Order Copilot Install";
        UpgradeTag: Codeunit "Upgrade Tag";
    begin
        if not UpgradeTag.HasUpgradeTag(GetRegisterOrderCopilotTag()) then begin
            OrderCopilotInstall.RegisterCapability();

            UpgradeTag.SetUpgradeTag(GetRegisterOrderCopilotTag());
        end;
    end;

    internal procedure GetRegisterOrderCopilotTag(): Code[250]
    begin
        exit('AI-RegisterOrderCopilot-20240506');
    end;

}
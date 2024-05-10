// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

codeunit 50104 "DOK Copilot Capability Upgrade"
{
    Subtype = Upgrade;
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnUpgradePerDatabase()
    var
        DOKCopilotCapabilityInstall: Codeunit "DOK Copilot Capability Install";
    begin
        DOKCopilotCapabilityInstall.RegisterCapability();
    end;
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup.Metadata;

using SalesReturnAgent.Setup;
using System.Agents;
using System.AI;
using System.Reflection;
using System.Security.AccessControl;

codeunit 53700 SalesRetAgentFactory implements IAgentFactory
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure GetDefaultInitials(): Text[4]
    begin
        exit(SalesRetAgentSetup.GetInitials());
    end;

    procedure GetFirstTimeSetupPageId(): Integer
    begin
        exit(SalesRetAgentSetup.GetSetupPageId());
    end;

    procedure ShowCanCreateAgent(): Boolean
    var
        SalesRetAgentSetupRec: Record "Sales Ret. Agent Setup";
    begin
        exit(SalesRetAgentSetupRec.IsEmpty());
    end;

    procedure GetCopilotCapability(): Enum "Copilot Capability"
    begin
        exit("Copilot Capability"::"Sales Return Agent");
    end;

    procedure GetDefaultProfile(var TempAllProfile: Record "All Profile" temporary)
    begin
        SalesRetAgentSetup.GetDefaultProfile(TempAllProfile);
    end;

    procedure GetDefaultAccessControls(var TempAccessControlTemplate: Record "Access Control Buffer" temporary)
    begin
        SalesRetAgentSetup.GetDefaultAccessControls(TempAccessControlTemplate);
    end;

    var
        SalesRetAgentSetup: Codeunit "Sales Ret. Agent Setup";
}

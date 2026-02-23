// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup.Metadata;

using SalesValidationAgent.Setup;
using System.Agents;
using System.AI;
using System.Reflection;
using System.Security.AccessControl;

codeunit 50100 SalesValAgentFactory implements IAgentFactory
{
    Access = Internal;

    procedure GetDefaultInitials(): Text[4]
    begin
        exit(SalesValAgentSetup.GetInitials());
    end;

    procedure GetFirstTimeSetupPageId(): Integer
    begin
        exit(SalesValAgentSetup.GetSetupPageId());
    end;

    procedure ShowCanCreateAgent(): Boolean
    var
        SalesValAgentSetupRec: Record "Sales Val. Agent Setup";
    begin
        // ShowCanCreateAgent controls UI visibility only; to truly enforce single-instance, the Setup table/page also needs to prevent duplicate records.
        exit(SalesValAgentSetupRec.IsEmpty());
    end;

    procedure GetCopilotCapability(): Enum "Copilot Capability"
    begin
        exit("Copilot Capability"::"Sales Validation Agent");
    end;

    procedure GetDefaultProfile(var TempAllProfile: Record "All Profile" temporary)
    begin
        SalesValAgentSetup.GetDefaultProfile(TempAllProfile);
    end;

    procedure GetDefaultAccessControls(var TempAccessControlTemplate: Record "Access Control Buffer" temporary)
    begin
        SalesValAgentSetup.GetDefaultAccessControls(TempAccessControlTemplate);
    end;

    var
        SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
}
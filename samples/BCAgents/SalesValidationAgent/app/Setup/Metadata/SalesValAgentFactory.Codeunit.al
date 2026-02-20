namespace ThirdPartyPublisher.SalesValidationAgent.Setup.Metadata;

using System.Agents;
using System.AI;
using System.Reflection;
using System.Security.AccessControl;
using ThirdPartyPublisher.SalesValidationAgent.Setup;

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
        // Single instance agent
        exit(not SalesValAgentSetupRec.FindFirst());
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
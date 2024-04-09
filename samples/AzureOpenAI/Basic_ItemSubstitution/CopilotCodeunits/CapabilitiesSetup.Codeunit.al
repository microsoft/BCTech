namespace CopilotToolkitDemo.ItemSubstitution;

using System.AI;

codeunit 54340 "Capabilities Setup"
{
    Subtype = Install;
    InherentEntitlements = X;
    InherentPermissions = X;
    Access = Internal;

    trigger OnInstallAppPerDatabase()
    begin
        RegisterCapability();
    end;

    local procedure RegisterCapability()
    var
        CopilotCapability: Codeunit "Copilot Capability";
        LearnMoreUrlTxt: Label 'https://example.com/CopilotToolkit', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Find Item Substitutions") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Find Item Substitutions", Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);
    end;
}
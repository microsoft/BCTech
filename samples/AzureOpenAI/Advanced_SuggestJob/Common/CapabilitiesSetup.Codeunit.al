namespace CopilotToolkitDemo.Common;

using System.AI;
using System.Environment;

codeunit 54310 "Capabilities Setup"
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
        cd: Codeunit "AOAI Deployments";
        CopilotCapability: Codeunit "Copilot Capability";
        EnvironmentInformation: Codeunit "Environment Information";
        LearnMoreUrlTxt: Label 'https://example.com/CopilotToolkit', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Describe Project") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Describe Project", Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);
    end;
}
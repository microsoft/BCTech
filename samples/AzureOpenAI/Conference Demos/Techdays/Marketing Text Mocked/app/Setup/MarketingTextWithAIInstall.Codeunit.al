namespace Techdays.AITestToolkitDemo;
using System.Environment;
using System.AI;

codeunit 50101 "Marketing Text With AI Install"
{
    Subtype = Install;
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnInstallAppPerDatabase()
    begin
        RegisterCapability();
    end;

    local procedure RegisterCapability()
    var
        EnvironmentInfo: Codeunit "Environment Information";
        CopilotCapability: Codeunit "Copilot Capability";
        LearnMoreUrlTxt: Label 'https://microsoft.com', Locked = true;
    begin
        // Verify that environment in a Business Central online environment
        if EnvironmentInfo.IsSaaSInfrastructure() then
            // Register capability 
            if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Marketing Text Simple") then
                CopilotCapability.RegisterCapability(
                Enum::"Copilot Capability"::"Marketing Text Simple",
                Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);
    end;
}
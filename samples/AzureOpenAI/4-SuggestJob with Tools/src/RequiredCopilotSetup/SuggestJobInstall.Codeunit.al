namespace CopilotToolkitDemo.SuggestJobBasic;

using System.AI;

codeunit 54393 "SuggestJob - Install"
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
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Suggest Project") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Suggest Project", Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);
    end;
}
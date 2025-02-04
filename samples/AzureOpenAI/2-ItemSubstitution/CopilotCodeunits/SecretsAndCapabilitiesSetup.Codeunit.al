namespace CopilotToolkitDemo.ItemSubstitution;

using System.AI;

codeunit 54310 "Secrets And Capabilities Setup"
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
        IsolatedStorageWrapper: Codeunit "Isolated Storage Wrapper";
        LearnMoreUrlTxt: Label 'https://example.com/CopilotToolkit', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Find Item Substitutions") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Find Item Substitutions", Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);

        // You will need to use your own key for Azure OpenAI for all your Copilot features (for both development and production).
        Error('Set up your secrets here before publishing the app.');
        // IsolatedStorageWrapper.SetSecretKey('');
        // IsolatedStorageWrapper.SetDeployment('');
        // IsolatedStorageWrapper.SetEndpoint('');
    end;
}
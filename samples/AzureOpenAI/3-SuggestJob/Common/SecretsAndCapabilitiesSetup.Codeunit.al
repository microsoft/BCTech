namespace CopilotToolkitDemo.Common;

using System.AI;
using System.Environment;

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
        EnvironmentInformation: Codeunit "Environment Information";
        IsolatedStorageWrapper: Codeunit "Isolated Storage Wrapper";
        LearnMoreUrlTxt: Label 'https://example.com/CopilotToolkit', Locked = true;
    begin
        if not CopilotCapability.IsCapabilityRegistered(Enum::"Copilot Capability"::"Describe Project") then
            CopilotCapability.RegisterCapability(Enum::"Copilot Capability"::"Describe Project", Enum::"Copilot Availability"::Preview, LearnMoreUrlTxt);

        // You will need to use your own key for Azure OpenAI for all your Copilot features (for both development and production).
        Error('Set up your secrets here before publishing the app.');
        // IsolatedStorageWrapper.SetSecretKey('');
        // IsolatedStorageWrapper.SetDeployment('');
        // IsolatedStorageWrapper.SetEndpoint('');
    end;
}
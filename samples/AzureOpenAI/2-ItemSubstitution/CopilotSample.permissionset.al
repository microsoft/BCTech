namespace CopilotToolkitDemo.ItemSubstitution;

permissionset 54300 CopilotSample
{
    Assignable = true;
    Permissions = tabledata "Copilot Item Sub Proposal" = RIMD,
        table "Copilot Item Sub Proposal" = X,
        codeunit "Generate Item Sub Proposal" = X,
        codeunit "Isolated Storage Wrapper" = X,
        codeunit "Secrets And Capabilities Setup" = X,
        page "Copilot Item Sub Proposal" = X,
        page "Copilot Item Subs Proposal Sub" = X;
}
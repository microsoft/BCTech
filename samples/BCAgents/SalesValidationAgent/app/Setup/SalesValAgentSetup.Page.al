namespace ThirdPartyPublisher.SalesValidationAgent.Setup;

using System.Agents;
using System.AI;

page 50100 "Sales Val. Agent Setup"
{
    PageType = ConfigurationDialog;
    Extensible = false;
    ApplicationArea = All;
    IsPreview = true;
    Caption = 'Set up Sales Validation Agent';
    InstructionalText = 'The Sales Validation Agent validates and processes sales orders by checking inventory reservation and releasing eligible orders to the warehouse.';
    AdditionalSearchTerms = 'Sales Validation Agent, Agent';
    SourceTable = "Sales Val. Agent Setup";
    SourceTableTemporary = true;
    InherentEntitlements = X;
    InherentPermissions = X;

    layout
    {
        area(Content)
        {
            part(AgentSetupPart; "Agent Setup Part")
            {
                ApplicationArea = All;
                UpdatePropagation = Both;
            }
        }
    }
    actions
    {
        area(SystemActions)
        {
            systemaction(OK)
            {
                Caption = 'Update';
                Enabled = IsUpdated;
                ToolTip = 'Apply the changes to the agent setup.';
            }

            systemaction(Cancel)
            {
                Caption = 'Cancel';
                ToolTip = 'Discards the changes and closes the setup page.';
            }
        }
    }

    trigger OnOpenPage()
    begin
        if not AzureOpenAI.IsEnabled(Enum::"Copilot Capability"::"Sales Validation Agent") then
            Error(SalesValAgentNotEnabledErr);

        IsUpdated := false;
        InitializePage();
    end;

    trigger OnAfterGetRecord()
    begin
        InitializePage();
    end;

    trigger OnAfterGetCurrRecord()
    begin
        IsUpdated := IsUpdated or CurrPage.AgentSetupPart.Page.GetChangesMade();
    end;

    trigger OnModifyRecord(): Boolean
    begin
        IsUpdated := true;
    end;

    trigger OnQueryClosePage(CloseAction: Action): Boolean
    var
        Agent: Codeunit Agent;
        SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
    begin
        if CloseAction = CloseAction::Cancel then
            exit(true);

        CurrPage.AgentSetupPart.Page.GetAgentSetupBuffer(AgentSetupBuffer);

        if IsNullGuid(AgentSetupBuffer."User Security ID") then
            AgentSetupBuffer."Agent Metadata Provider" := Enum::"Agent Metadata Provider"::"Sales Validation Agent";

        if GlobalAgentSetup.GetChangesMade(AgentSetupBuffer) then begin
            Rec."User Security ID" := GlobalAgentSetup.SaveChanges(AgentSetupBuffer);

            Agent.SetInstructions(Rec."User Security ID", SalesValAgentSetup.GetInstructions());
        end;

        SalesValAgentSetup.EnsureSetupExists(Rec."User Security ID");
        exit(true);
    end;

    local procedure InitializePage()
    var
        AgentSetup: Codeunit "Agent Setup";
    begin
        if Rec.IsEmpty() then
            Rec.Insert();

        CurrPage.AgentSetupPart.Page.GetAgentSetupBuffer(AgentSetupBuffer);
        if AgentSetupBuffer.IsEmpty() then
            AgentSetup.GetSetupRecord(
                AgentSetupBuffer,
                Rec."User Security ID",
                Enum::"Agent Metadata Provider"::"Sales Validation Agent",
                AgentNameLbl + ' - ' + CompanyName(),
                DefaultDisplayNameLbl,
                AgentSummaryLbl);

        CurrPage.AgentSetupPart.Page.SetAgentSetupBuffer(AgentSetupBuffer);
        CurrPage.AgentSetupPart.Page.Update(false);

        IsUpdated := IsUpdated or CurrPage.AgentSetupPart.Page.GetChangesMade();
    end;

    var
        AgentSetupBuffer: Record "Agent Setup Buffer";
        GlobalAgentSetup: Codeunit "Agent Setup";
        AzureOpenAI: Codeunit "Azure OpenAI";
        IsUpdated: Boolean;
        SalesValAgentNotEnabledErr: Label 'The Sales Validation Agent capability is not enabled in Copilot capabilities.\\Please enable the capability before setting up the agent.';
        AgentNameLbl: Label 'Sales Validation Agent';
        DefaultDisplayNameLbl: Label 'Sales Validation Agent';
        AgentSummaryLbl: Label 'Validates and processes sales orders by checking inventory reservation and releasing eligible orders to the warehouse.';
}
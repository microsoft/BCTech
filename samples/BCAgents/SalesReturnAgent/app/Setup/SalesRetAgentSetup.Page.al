// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup;

using System.Agents;
using System.AI;

#pragma warning disable AL0906
page 53700 "Sales Ret. Agent Setup"
#pragma warning restore AL0906
{
    PageType = ConfigurationDialog;
    Extensible = false;
    ApplicationArea = All;
    IsPreview = true;
    Caption = 'Set up Sales Return Agent';
    InstructionalText = 'The Sales Return Agent creates credit memos for customer returns, populates work descriptions with return justifications, and generates PDF summaries.';
    AdditionalSearchTerms = 'Sales Return Agent, Agent';
    SourceTable = "Sales Ret. Agent Setup";
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
            group(GetStarted)
            {
                Caption = 'Get started';
                group(HowItWorksGroup)
                {
                    Caption = 'How it works';
                    InstructionalText = 'Assign tasks to the Sales Return Agent by sending messages. The agent receives return requests from customers via email and creates credit memos accordingly.';
                }
                group(LearnMore)
                {
                    Caption = 'Learn more';
                    InstructionalText = 'To learn more about the Sales Return Agent''s capabilities and how to prepare the data for the agent to complete its tasks, see the documentation article.';

                    field(OpenDocumentation; OpenDocumentationTxt)
                    {
                        ShowCaption = false;
                        Editable = false;
                        ToolTip = 'Open the Sales Return Agent documentation article in your browser.';

                        trigger OnDrillDown()
                        begin
                            Hyperlink(AgentLearnMoreUrlTok);
                        end;
                    }
                }
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
        if not AzureOpenAI.IsEnabled(Enum::"Copilot Capability"::"Sales Return Agent") then
            Error(SalesRetAgentNotEnabledErr);

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
        SalesRetAgentSetup: Codeunit "Sales Ret. Agent Setup";
    begin
        if CloseAction = CloseAction::Cancel then
            exit(true);

        CurrPage.AgentSetupPart.Page.GetAgentSetupBuffer(TempAgentSetupBuffer);
        Rec."User Security ID" := SalesRetAgentSetup.SaveAgent(TempAgentSetupBuffer);
        exit(true);
    end;

    local procedure InitializePage()
    var
        AgentSetup: Codeunit "Agent Setup";
    begin
        if Rec.IsEmpty() then
            Rec.Insert();

        CurrPage.AgentSetupPart.Page.GetAgentSetupBuffer(TempAgentSetupBuffer);
        if TempAgentSetupBuffer.IsEmpty() then
            AgentSetup.GetSetupRecord(
                TempAgentSetupBuffer,
                Rec."User Security ID",
                Enum::"Agent Metadata Provider"::"Sales Return Agent",
                AgentNameLbl + ' - ' + CompanyName(),
                DefaultDisplayNameLbl,
                AgentSummaryLbl);

        CurrPage.AgentSetupPart.Page.SetAgentSetupBuffer(TempAgentSetupBuffer);
        CurrPage.AgentSetupPart.Page.Update(false);

        IsUpdated := IsUpdated or CurrPage.AgentSetupPart.Page.GetChangesMade();
    end;

    var
        TempAgentSetupBuffer: Record "Agent Setup Buffer";
        AzureOpenAI: Codeunit "Azure OpenAI";
        IsUpdated: Boolean;
        SalesRetAgentNotEnabledErr: Label 'The Sales Return Agent capability is not enabled in Copilot capabilities.\\Please enable the capability before setting up the agent.';
        AgentNameLbl: Label 'Sales Return Agent';
        DefaultDisplayNameLbl: Label 'Sales Return Agent';
        AgentSummaryLbl: Label 'Creates credit memos for customer returns, populates work descriptions with return justifications, and generates PDF summaries.';
        OpenDocumentationTxt: Label 'Learn more';
        AgentLearnMoreUrlTok: Label 'https://go.microsoft.com/fwlink/?linkid=2350506', Locked = true;
}

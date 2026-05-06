// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup;

using Microsoft.Sales.Document;
using System.Agents;
using System.AI;

#pragma warning disable AL0906
page 53606 "Sales Val. Agent Setup"
#pragma warning restore AL0906
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
            group(GetStarted)
            {
                Caption = 'Get started';
                group(OpenSalesOrdersGroups)
                {
                    Caption = ' Try out the agent';
                    InstructionalText = 'Open the Sales Orders list and choose the "Validate with Agent" action.';

                    field(OpenSalesOrders; OpenSalesOrdersTxt)
                    {
                        ShowCaption = false;
                        Editable = false;
                        ToolTip = 'Open the Sales Orders list to assign the first task using the Validate with Agent action.';

                        trigger OnDrillDown()
                        var
                            Agent: Codeunit Agent;
                        begin
                            if not Agent.IsActive(Rec."User Security ID") then
                                Error(AgentMustBeActivatedErr);
                            Page.Run(Page::"Sales Order List");
                            CurrPage.Close();
                        end;
                    }
                }
                group(LearnMore)
                {
                    Caption = 'Learn more';
                    InstructionalText = 'To learn more about the Sales Validation Agent''s capabilities and how to prepare the data for the agent to complete its tasks, see the documentation article.';

                    field(OpenDocumentation; OpenDocumentationTxt)
                    {
                        ShowCaption = false;
                        Editable = false;
                        ToolTip = 'Open the Sales Validation Agent documentation article in your browser.';

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
        SalesValAgentSetup: Codeunit "Sales Val. Agent Setup";
    begin
        if CloseAction = CloseAction::Cancel then
            exit(true);

        CurrPage.AgentSetupPart.Page.GetAgentSetupBuffer(TempAgentSetupBuffer);
        Rec."User Security ID" := SalesValAgentSetup.SaveAgent(TempAgentSetupBuffer);
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
                Enum::"Agent Metadata Provider"::"Sales Validation Agent",
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
        SalesValAgentNotEnabledErr: Label 'The Sales Validation Agent capability is not enabled in Copilot capabilities.\\Please enable the capability before setting up the agent.';
        AgentNameLbl: Label 'Sales Validation Agent';
        DefaultDisplayNameLbl: Label 'Sales Validation Agent';
        AgentSummaryLbl: Label 'Validates and processes sales orders by checking inventory reservation and releasing eligible orders to the warehouse.';
        OpenSalesOrdersTxt: Label 'Open Sales Orders list';
        OpenDocumentationTxt: Label 'Learn more';
        AgentMustBeActivatedErr: Label 'The Sales Validation Agent must be activated before opening the Sales Orders list. Please activate the agent to proceed.';
        AgentLearnMoreUrlTok: Label 'https://go.microsoft.com/fwlink/?linkid=2350506', Locked = true;
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;

using System.Agents;
using System.Email;
using System.Agents.Playground.CustomAgent;
using System.Threading;

page 50100 "Sample Setup"
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Sample Setup";

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                field(Name; AgentName)
                {
                    Caption = 'Agent';
                    ApplicationArea = All;
                    Editable = false;

                    trigger OnAssistEdit()
                    var
                        CustomAgentInfo: Record "Custom Agent Info";
                        CustomAgent: Codeunit "Custom Agent";
                    begin
                        Rec."Agent User Security ID" := SelectAgents(); // TODO: BCApps change required

                        CustomAgent.GetCustomAgentById(Rec."Agent User Security ID", CustomAgentInfo);
                        AgentName := CustomAgentInfo."User Name";
                    end;
                }

                field(EmailAccount; EmailAccountName)
                {
                    Caption = 'Email Account';
                    ApplicationArea = All;
                    Editable = false;

                    trigger OnAssistEdit()
                    var
                        TempEmailAccount: Record "Email Account" temporary;
                        EmailAccounts: Page "Email Accounts";
                    begin
                        if not CheckMailboxExists() then
                            Page.RunModal(Page::"Email Account Wizard");

                        if not CheckMailboxExists() then
                            exit;

                        EmailAccounts.EnableLookupMode();
                        EmailAccounts.FilterConnectorV4Accounts(true);
                        if EmailAccounts.RunModal() = Action::LookupOK then begin
                            EmailAccounts.GetAccount(TempEmailAccount);
                            Rec."Email Account ID" := TempEmailAccount."Account Id";
                            Rec."Email Connector" := TempEmailAccount.Connector;
                            Rec."Email Address" := TempEmailAccount."Email Address";
                            EmailAccountName := TempEmailAccount."Email Address";
                        end;
                    end;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(ScheduleSync)
            {
                Caption = 'Schedule Sync';
                ApplicationArea = All;

                trigger OnAction()
                var
                    Sample: Codeunit "Sample";
                begin
                    Sample.ScheduleNextRun(Rec);
                end;
            }
            action(RemoveScheduledSync)
            {
                Caption = 'Remove Scheduled Sync';
                ApplicationArea = All;

                trigger OnAction()
                var
                    Sample: Codeunit "Sample";
                begin
                    Sample.RemoveScheduledTask(Rec);
                end;
            }
            action(ScheduledTasks)
            {
                Caption = 'Scheduled Tasks';
                ApplicationArea = All;

                trigger OnAction()
                var
                    ScheduledTask: Page "Scheduled Tasks";
                begin
                    ScheduledTask.Run();
                end;
            }
        }
    }

    trigger OnOpenPage()
    var
        CustomAgentInfo: Record "Custom Agent Info";
        CustomAgent: Codeunit "Custom Agent";
    begin
        if not Rec.FindFirst() then begin
            Rec.Init();
            Rec.Insert();
        end else begin
            CustomAgent.GetCustomAgentById(Rec."Agent User Security ID", CustomAgentInfo);
            AgentName := CustomAgentInfo."User Name";
            EmailAccountName := Rec."Email Address";
        end;
    end;

    trigger OnQueryClosePage(CloseAction: Action): Boolean
    begin
        Rec.Modify();
    end;

    var
        AgentName: Text;
        EmailAccountName: Text;


    local procedure CheckMailboxExists(): Boolean
    var
        EmailAccounts: Record "Email Account";
        EmailAccount: Codeunit "Email Account";
        IConnector: Interface "Email Connector";
    begin
        EmailAccount.GetAllAccounts(false, EmailAccounts);
        if EmailAccounts.IsEmpty() then
            exit(false);

        repeat
            IConnector := EmailAccounts.Connector;
            if IConnector is "Email Connector v4" then
                exit(true);
        until EmailAccounts.Next() = 0;
    end;
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesValidationAgent.Setup;

using SalesValidationAgent.Setup.KPI;
using System.Agents;
using System.Reflection;
using System.Security.AccessControl;

codeunit 53609 "Sales Val. Agent Setup"
{
    InherentEntitlements = X;
    InherentPermissions = X;
    procedure TryGetAgent(var AgentUserSecurityId: Guid): Boolean
    var
        SalesValAgentSetupRec: Record "Sales Val. Agent Setup";
    begin
        if SalesValAgentSetupRec.FindFirst() then begin
            AgentUserSecurityId := SalesValAgentSetupRec."User Security ID";
            exit(true);
        end;

        exit(false);
    end;

    procedure GetInitials(): Text[4]
    begin
        exit(AgentInitialsLbl);
    end;

    procedure GetSetupPageId(): Integer
    begin
        exit(Page::"Sales Val. Agent Setup");
    end;

    procedure GetSummaryPageId(): Integer
    begin
        exit(Page::"Sales Val. Agent KPI");
    end;

    procedure CreateDefaultAgent(): Guid
    var
        TempAgentSetupBuffer: Record "Agent Setup Buffer";
        AgentSetup: Codeunit "Agent Setup";
        AgentUserSecurityId: Guid;
    begin
        if TryGetAgent(AgentUserSecurityId) then
            exit(AgentUserSecurityId);

        AgentSetup.GetSetupRecord(
            TempAgentSetupBuffer,
            AgentUserSecurityId,
            Enum::"Agent Metadata Provider"::"Sales Validation Agent",
            AgentNameTok + ' - ' + CompanyName(),
            DefaultDisplayNameTok,
            AgentSummaryTok);

        exit(SaveAgent(TempAgentSetupBuffer));
    end;

    internal procedure SaveAgent(var TempAgentSetupBuffer: Record "Agent Setup Buffer"): Guid
    var
        AgentSetup: Codeunit "Agent Setup";
        AgentUserSecurityId: Guid;
        IsNewAgent: Boolean;
    begin
        IsNewAgent := IsNullGuid(TempAgentSetupBuffer."User Security ID");

        if IsNewAgent then begin
            // Create: always save new agent
            TempAgentSetupBuffer."Agent Metadata Provider" := Enum::"Agent Metadata Provider"::"Sales Validation Agent";
            AgentUserSecurityId := AgentSetup.SaveChanges(TempAgentSetupBuffer);
            Agent.SetInstructions(AgentUserSecurityId, GetInstructions());
            EnsureSetupExists(AgentUserSecurityId);
        end else begin
            // Update: only save if changes were made
            AgentUserSecurityId := TempAgentSetupBuffer."User Security ID";
            if AgentSetup.GetChangesMade(TempAgentSetupBuffer) then begin
                AgentUserSecurityId := AgentSetup.SaveChanges(TempAgentSetupBuffer);
                Agent.SetInstructions(AgentUserSecurityId, GetInstructions());
            end;
        end;

        exit(AgentUserSecurityId);
    end;

    procedure EnsureSetupExists(UserSecurityID: Guid)
    var
        SalesValAgentSetupRec: Record "Sales Val. Agent Setup";
    begin
        if not SalesValAgentSetupRec.Get(UserSecurityID) then begin
            SalesValAgentSetupRec."User Security ID" := UserSecurityID;
            SalesValAgentSetupRec.Insert();
        end;
    end;

    [NonDebuggable]
    procedure GetInstructions(): SecretText
    var
        Instructions: Text;
    begin
        Instructions := NavApp.GetResourceAsText('Instructions/InstructionsV1.md');
        exit(Instructions);
    end;

    procedure GetDefaultProfile(var TempAllProfile: Record "All Profile" temporary)
    var
        CurrentModuleInfo: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(CurrentModuleInfo);
        Agent.PopulateDefaultProfile(DefaultProfileTxt, CurrentModuleInfo.Id, TempAllProfile);
    end;

    procedure GetDefaultAccessControls(var TempAccessControlBuffer: Record "Access Control Buffer" temporary)
    begin
        Clear(TempAccessControlBuffer);
        TempAccessControlBuffer."Company Name" := CopyStr(CompanyName(), 1, MaxStrLen(TempAccessControlBuffer."Company Name"));
        TempAccessControlBuffer.Scope := TempAccessControlBuffer.Scope::System;
        TempAccessControlBuffer."App ID" := BaseApplicationAppIdTok;
        TempAccessControlBuffer."Role ID" := D365ReadPermissionSetTok;
        TempAccessControlBuffer.Insert();

        TempAccessControlBuffer.Init();
        TempAccessControlBuffer."Company Name" := CopyStr(CompanyName(), 1, MaxStrLen(TempAccessControlBuffer."Company Name"));
        TempAccessControlBuffer.Scope := TempAccessControlBuffer.Scope::System;
        TempAccessControlBuffer."App ID" := BaseApplicationAppIdTok;
        TempAccessControlBuffer."Role ID" := D365SalesPermissionSetTok;
        TempAccessControlBuffer.Insert();
    end;

    var
        Agent: Codeunit Agent;
        DefaultProfileTxt: Label 'SV AGENT', Locked = true;
        AgentInitialsLbl: Label 'SV', MaxLength = 4;
        BaseApplicationAppIdTok: Label '437dbf0e-84ff-417a-965d-ed2bb9650972', Locked = true;
        D365ReadPermissionSetTok: Label 'D365 READ', Locked = true;
        D365SalesPermissionSetTok: Label 'D365 SALES', Locked = true;
        AgentNameTok: Label 'Sales Validation Agent', Locked = true;
        DefaultDisplayNameTok: Label 'Sales Validation Agent', Locked = true;
        AgentSummaryTok: Label 'Validates and processes sales orders by checking inventory reservation and releasing eligible orders to the warehouse.', Locked = true;
}
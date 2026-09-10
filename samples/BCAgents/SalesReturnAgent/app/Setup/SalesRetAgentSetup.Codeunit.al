// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace SalesReturnAgent.Setup;

using SalesReturnAgent.Setup.KPI;
using System.Agents;
using System.Reflection;
using System.Security.AccessControl;

codeunit 53703 "Sales Ret. Agent Setup"
{
    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure TryGetAgent(var AgentUserSecurityId: Guid): Boolean
    var
        SalesRetAgentSetupRec: Record "Sales Ret. Agent Setup";
    begin
        if SalesRetAgentSetupRec.FindFirst() then begin
            AgentUserSecurityId := SalesRetAgentSetupRec."User Security ID";
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
        exit(Page::"Sales Ret. Agent Setup");
    end;

    procedure GetSummaryPageId(): Integer
    begin
        exit(Page::"Sales Ret. Agent KPI");
    end;

    procedure GetOrCreateDefaultAgent(): Guid
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
            Enum::"Agent Metadata Provider"::"Sales Return Agent",
            AgentNameTok + ' - ' + CompanyName(),
            DefaultDisplayNameTok,
            AgentSummaryLbl);

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
            TempAgentSetupBuffer."Agent Metadata Provider" := Enum::"Agent Metadata Provider"::"Sales Return Agent";
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
        SalesRetAgentSetupRec: Record "Sales Ret. Agent Setup";
    begin
        if not SalesRetAgentSetupRec.Get(UserSecurityID) then begin
            SalesRetAgentSetupRec."User Security ID" := UserSecurityID;
            SalesRetAgentSetupRec.Insert();
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
        Agent.PopulateDefaultProfile(DefaultProfileTok, CurrentModuleInfo.Id, TempAllProfile);
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
        DefaultProfileTok: Label 'SR AGENT', Locked = true;
        AgentInitialsLbl: Label 'SR', MaxLength = 4;
        BaseApplicationAppIdTok: Label '437dbf0e-84ff-417a-965d-ed2bb9650972', Locked = true;
        D365ReadPermissionSetTok: Label 'D365 READ', Locked = true;
        D365SalesPermissionSetTok: Label 'D365 SALES', Locked = true;
        AgentNameTok: Label 'Sales Return Agent', Locked = true;
        DefaultDisplayNameTok: Label 'Sales Return Agent', Locked = true;
        AgentSummaryLbl: Label 'Creates credit memos for customer returns, populates work descriptions with return justifications, and generates PDF summaries.';
}

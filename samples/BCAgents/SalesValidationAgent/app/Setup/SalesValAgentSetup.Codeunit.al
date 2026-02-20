namespace SalesValidationAgent.Setup;

using System.Agents;
using System.Reflection;
using System.Security.AccessControl;
using SalesValidationAgent.Setup.KPI;

codeunit 50103 "Sales Val. Agent Setup"
{
    Access = Internal;

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
        Instructions := NavApp.GetResourceAsText('Instructions/InstructionsV1.txt');
        exit(Instructions);
    end;

    internal procedure GetDefaultProfile(var TempAllProfile: Record "All Profile" temporary)
    var
        CurrentModuleInfo: ModuleInfo;
    begin
        NavApp.GetCurrentModuleInfo(CurrentModuleInfo);
        Agent.PopulateDefaultProfile(DefaultProfileTok, CurrentModuleInfo.Id, TempAllProfile);
    end;

    internal procedure GetDefaultAccessControls(var TempAccessControlBuffer: Record "Access Control Buffer" temporary)
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
        DefaultProfileTok: Label 'SALES VALIDATION AGENT', Locked = true;
        AgentInitialsLbl: Label 'SV', MaxLength = 4;
        BaseApplicationAppIdTok: Label '437dbf0e-84ff-417a-965d-ed2bb9650972', Locked = true;
        D365ReadPermissionSetTok: Label 'D365 READ', Locked = true;
        D365SalesPermissionSetTok: Label 'D365 SALES', Locked = true;
}
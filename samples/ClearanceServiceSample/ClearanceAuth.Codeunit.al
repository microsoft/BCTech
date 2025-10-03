// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace BCTech.EServices.EDocumentConnector;

using System.Security.Authentication;
using System.Environment;

codeunit 50100 "Clearance Auth."
{
    Access = Internal;
    Permissions = tabledata "OAuth 2.0 Setup" = rim;

    internal procedure InitConnectionSetup()
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
        OAuth2: Codeunit OAuth2;
        RedirectUrl: Text;
    begin
        if not EDocExtConnectionSetup.Get() then begin
            EDocExtConnectionSetup."OAuth Feature GUID" := CreateGuid();
            EDocExtConnectionSetup."Authentication URL" := AuthURLTxt;
            EDocExtConnectionSetup."FileAPI URL" := FileAPITxt;
            OAuth2.GetDefaultRedirectURL(RedirectUrl);
            EDocExtConnectionSetup.Insert();
        end;
    end;

    internal procedure ResetConnectionSetup()
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
        OAuth2: Codeunit OAuth2;
        RedirectUrl: Text;
    begin
        if EDocExtConnectionSetup.Get() then begin
            EDocExtConnectionSetup."Authentication URL" := AuthURLTxt;
            EDocExtConnectionSetup."FileAPI URL" := FileAPITxt;
            OAuth2.GetDefaultRedirectURL(RedirectUrl);
            EDocExtConnectionSetup.Modify();
        end;
    end;

    [NonDebuggable]
    internal procedure SetClientId(var ClienId: Guid; ClientID: Text)
    var
    begin
        SetIsolatedStorageValue(ClienId, ClientID, DataScope::Company);
    end;

    internal procedure SetClientSecret(var ClienSecret: Guid; ClientSecret: SecretText)
    begin
        SetIsolatedStorageValue(ClienSecret, ClientSecret, DataScope::Company);
    end;

    internal procedure IsClientCredsSet(var ClientId: Text; var ClientSecret: Text): Boolean
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
    begin
        EDocExtConnectionSetup.Get();

        if EnvironmentInfo.IsSaaSInfrastructure() then
            exit(true);

        if HasToken(EDocExtConnectionSetup."Client ID", DataScope::Company) then
            ClientId := '*';
        if HasToken(EDocExtConnectionSetup."Client Secret", DataScope::Company) then
            ClientSecret := '*';
    end;

    internal procedure OpenOAuthSetupPage()
    var
        OAuth20Setup: Record "OAuth 2.0 Setup";
    begin
        InitOAuthSetup(OAuth20Setup);
        Commit();
        Page.RunModal(Page::"OAuth 2.0 Setup", OAuth20Setup);
    end;

    internal procedure GetAuthBearerTxt(): SecretText;
    var
        OAuth20Setup: Record "OAuth 2.0 Setup";
        HttpError: Text;
    begin
        GetOAuth2Setup(OAuth20Setup);
        if OAuth20Setup."Access Token Due DateTime" < CurrentDateTime() + 60 * 1000 then
            if not RefreshAccessToken(HttpError) then
                Error(HttpError);

        exit(SecretStrSubstNo(BearerTxt, GetToken(OAuth20Setup."Access Token", OAuth20Setup.GetTokenDataScope())));
    end;

    [NonDebuggable]
    local procedure RefreshAccessToken(var HttpError: Text): Boolean;
    var
        OAuth20Setup: Record "OAuth 2.0 Setup";
    begin
        GetOAuth2Setup(OAuth20Setup);
        exit(OAuth20Setup.RefreshAccessToken(HttpError));
    end;

    [NonDebuggable]
    local procedure InitOAuthSetup(var OAuth20Setup: Record "OAuth 2.0 Setup")
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
        Exists: Boolean;
    begin
        EDocExtConnectionSetup.Get();

        if OAuth20Setup.Get(GetAuthSetupCode()) then
            Exists := true;

        OAuth20Setup.Code := GetAuthSetupCode();
        OAuth20Setup."Client ID" := CreateGuid();
        OAuth20Setup."Client Secret" := CreateGuid();
        OAuth20Setup."Service URL" := EDocExtConnectionSetup."Authentication URL";
        OAuth20Setup.Description := 'Clearance Online';
        OAuth20Setup.Scope := 'all';
        OAuth20Setup."Authorization URL Path" := AuthorizationURLPathTxt;
        OAuth20Setup."Access Token URL Path" := AccessTokenURLPathTxt;
        OAuth20Setup."Refresh Token URL Path" := RefreshTokenURLPathTxt;
        OAuth20Setup."Authorization Response Type" := AuthorizationResponseTypeTxt;
        OAuth20Setup."Token DataScope" := OAuth20Setup."Token DataScope"::Company;
        OAuth20Setup."Daily Limit" := 1000;
        OAuth20Setup."Feature GUID" := EDocExtConnectionSetup."OAuth Feature GUID";
        OAuth20Setup."User ID" := CopyStr(UserId(), 1, MaxStrLen(OAuth20Setup."User ID"));
        OAuth20Setup."Code Challenge Method" := OAuth20Setup."Code Challenge Method"::S256;
        if not Exists then
            OAuth20Setup.Insert()
        else
            OAuth20Setup.Modify();
    end;

    [NonDebuggable]
    local procedure GetOAuth2Setup(var OAuth20Setup: Record "OAuth 2.0 Setup"): Boolean;
    var
        ExternalConnectionSetup: Record "ClearModelExtConnectionSetup";
    begin
        if not ExternalConnectionSetup.Get() then
            Error(MissingAuthErr);

        ExternalConnectionSetup.TestField("OAuth Feature GUID");

        OAuth20Setup.Get(GetAuthSetupCode());
        exit(true);
    end;

    local procedure SetIsolatedStorageValue(var ValueKey: Guid; Value: SecretText; TokenDataScope: DataScope) NewToken: Boolean
    begin
        if IsNullGuid(ValueKey) then
            NewToken := true;
        if NewToken then
            ValueKey := CreateGuid();

        IsolatedStorage.Set(ValueKey, Value, TokenDataScope);
    end;

    local procedure GetToken(TokenKey: Text; TokenDataScope: DataScope) TokenValueAsSecret: SecretText
    begin
        if not HasToken(TokenKey, TokenDataScope) then
            exit(TokenValueAsSecret);

        IsolatedStorage.Get(TokenKey, TokenDataScope, TokenValueAsSecret);
    end;

    [NonDebuggable]
    local procedure HasToken(TokenKey: Text; TokenDataScope: DataScope): Boolean
    begin
        exit(IsolatedStorage.Contains(TokenKey, TokenDataScope));
    end;

    [NonDebuggable]
    local procedure GetAuthSetupCode(): Code[20]
    begin
        exit(ClearanceAuthCodeLbl);
    end;


    var
        EnvironmentInfo: Codeunit "Environment Information";
        AuthorizationURLPathTxt: Label '/oauth-authorize', Locked = true;
        AccessTokenURLPathTxt: Label '/oauth-token', Locked = true;
        RefreshTokenURLPathTxt: Label '/oauth-token', Locked = true;
        AuthorizationResponseTypeTxt: Label 'code', Locked = true;
        BearerTxt: Label 'Bearer %1', Comment = '%1 = text value', Locked = true;
        AuthURLTxt: Label 'http://localhost:5050', Locked = true;
        FileAPITxt: Label 'http://localhost:5050', Locked = true;
        ClearanceAuthCodeLbl: Label 'EDocClearance', Locked = true;
        MissingAuthErr: Label 'You must set up authentication to the service integration in the E-Document service card.';
}
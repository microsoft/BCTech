// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocumentConnector;

using System.Security.Authentication;
using System.Environment;

codeunit 9601 "Clearance Auth."
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

    [NonDebuggable]
    local procedure CheckOAuthConsistencySetup(OAuth20Setup: Record "OAuth 2.0 Setup")
    begin
        OAuth20Setup.TestField("Authorization URL Path", AuthorizationURLPathTxt);
        OAuth20Setup.TestField("Access Token URL Path", AccessTokenURLPathTxt);
        OAuth20Setup.TestField("Refresh Token URL Path", RefreshTokenURLPathTxt);
        OAuth20Setup.TestField("Authorization Response Type", AuthorizationResponseTypeTxt);
        OAuth20Setup.TestField("Daily Limit");
    end;

    local procedure SaveTokens(var OAuth20Setup: Record "OAuth 2.0 Setup"; TokenDataScope: DataScope; AccessToken: SecretText; RefreshToken: SecretText)
    begin
        SetIsolatedStorageValue(OAuth20Setup."Access Token", AccessToken, TokenDataScope);
        SetIsolatedStorageValue(OAuth20Setup."Refresh Token", RefreshToken, TokenDataScope);

        OAuth20Setup.Modify();
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

    [NonDebuggable]
    local procedure GetClientId(): Text
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
    begin

        if EDocExtConnectionSetup.Get() then
            exit(CreateGuid());
    end;

    local procedure GetClientSecret(): SecretText
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
    begin

        if EDocExtConnectionSetup.Get() then
            exit(GetToken(EDocExtConnectionSetup."Client Secret", DataScope::Company));
    end;

    [NonDebuggable]
    [EventSubscriber(ObjectType::Table, Database::"OAuth 2.0 Setup", 'OnBeforeRequestAccessToken', '', true, true)]
    local procedure OnBeforeRequestAccessToken(var OAuth20Setup: Record "OAuth 2.0 Setup"; AuthorizationCode: Text; var Result: Boolean; var MessageText: Text; var Processed: Boolean)
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
        RequestJSON: Text;
        AccessToken: SecretText;
        RefreshToken: SecretText;
        AuthorizationCodeSecret: SecretText;
        TokenDataScope: DataScope;
    begin
        if not EDocExtConnectionSetup.Get() then
            exit;

        Processed := true;

        CheckOAuthConsistencySetup(OAuth20Setup);
        TokenDataScope := OAuth20Setup.GetTokenDataScope();
        AuthorizationCodeSecret := AuthorizationCode;
        Result := OAuth20Mgt.RequestAccessTokenWithContentType(OAuth20Setup, RequestJSON, MessageText, AuthorizationCodeSecret, GetClientId(), GetClientSecret(), AccessToken, RefreshToken, true);

        if not Result then
            Error(AuthenticationFailedErr);

        SaveTokens(OAuth20Setup, TokenDataScope, AccessToken, RefreshToken);
        Message(AuthorizationSuccessfulTxt);
    end;

    [NonDebuggable]
    [EventSubscriber(ObjectType::Table, Database::"OAuth 2.0 Setup", 'OnBeforeRefreshAccessToken', '', true, true)]
    local procedure OnBeforeRefreshAccessToken(var OAuth20Setup: Record "OAuth 2.0 Setup"; var Result: Boolean; var MessageText: Text; var Processed: Boolean)
    var
        EDocExtConnectionSetup: Record "ClearModelExtConnectionSetup";
        RequestJSON: Text;
        AccessToken: SecretText;
        RefreshToken: SecretText;
        TokenDataScope: DataScope;
        OldServiceUrl: Text[250];
    begin
        if not EDocExtConnectionSetup.Get() then
            exit;
        if not GetOAuth2Setup(OAuth20Setup) or Processed then
            exit;

        CheckOAuthConsistencySetup(OAuth20Setup);

        Processed := true;

        TokenDataScope := OAuth20Setup.GetTokenDataScope();
        RefreshToken := GetToken(OAuth20Setup."Refresh Token", TokenDataScope);
        OldServiceUrl := OAuth20Setup."Service URL";

        Result := OAuth20Mgt.RefreshAccessTokenWithContentType(OAuth20Setup, RequestJSON, MessageText, GetClientId(), GetClientSecret(), AccessToken, RefreshToken, true);

        OAuth20Setup."Service URL" := OldServiceUrl;

        if not Result then
            Error(AuthenticationFailedErr);

        SaveTokens(OAuth20Setup, TokenDataScope, AccessToken, RefreshToken);
    end;

    [NonDebuggable]
    [EventSubscriber(ObjectType::Table, Database::"OAuth 2.0 Setup", 'OnBeforeInvokeRequest', '', true, true)]
    local procedure OnBeforeInvokeRequest(var OAuth20Setup: Record "OAuth 2.0 Setup"; RequestJSON: Text; var ResponseJSON: Text; var HttpError: Text; var Result: Boolean; var Processed: Boolean; RetryOnCredentialsFailure: Boolean)
    var
        ClearanceSetup: Record "ClearModelExtConnectionSetup";
        TokenValue: SecretText;
        RequestJsonObj: JsonObject;
        ResponseJsonObj: JsonObject;
    begin
        if not ClearanceSetup.Get() or Processed then
            exit;
        Processed := true;

        CheckOAuthConsistencySetup(OAuth20Setup);
        TokenValue := GetToken(OAuth20Setup."Access Token", OAuth20Setup.GetTokenDataScope());

        Result := OAuth20Mgt.InvokeRequest(OAuth20Setup, RequestJSON, ResponseJSON, HttpError, TokenValue, RetryOnCredentialsFailure);

        if RequestJsonObj.ReadFrom(RequestJSON) then;
        if ResponseJsonObj.ReadFrom(ResponseJSON) then;
    end;

    var
        EnvironmentInfo: Codeunit "Environment Information";
        OAuth20Mgt: Codeunit "OAuth 2.0 Mgt.";
        AuthorizationURLPathTxt: Label '/oauth-authorize', Locked = true;
        AccessTokenURLPathTxt: Label '/oauth-token', Locked = true;
        RefreshTokenURLPathTxt: Label '/oauth-token', Locked = true;
        AuthorizationResponseTypeTxt: Label 'code', Locked = true;
        BearerTxt: Label 'Bearer %1', Comment = '%1 = text value', Locked = true;
        AuthURLTxt: Label 'http://localhost:5050', Locked = true;
        FileAPITxt: Label 'http://localhost:5050', Locked = true;
        ClearanceAuthCodeLbl: Label 'EDocClearance', Locked = true;
        AuthorizationSuccessfulTxt: Label 'Authorization successful.';
        MissingAuthErr: Label 'You must set up authentication to the service integration in the E-Document service card.';
        AuthenticationFailedErr: Label 'Authentication failed, check your credentials in the E-Document service card.';
}
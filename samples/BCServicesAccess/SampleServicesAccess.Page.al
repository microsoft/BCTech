page 50105 "Sample Services Access"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Sample Services Access";

    layout
    {
        area(Content)
        {
            group("AAD Client Settings")
            {
                field("Aad Client Id"; AadClientId)
                {
                    ApplicationArea = All;
                }
                field("Aad Client Secret"; AadClientSecret)
                {
                    ApplicationArea = All;
                    ExtendedDatatype = Masked;
                }
                field("Redirect Url"; RedirectUrl)
                {
                    ApplicationArea = All;
                }
                field("Application ID Url"; AppIdUrl)
                {
                    ApplicationArea = All;
                }
                field("Login Hint"; LoginHint)
                {
                    ApplicationArea = All;
                }
            }
            group("AAD Settings")
            {
                field("Authority Url"; AuthorityUrl)
                {
                    ApplicationArea = All;
                }
            }
            group("Initial Access Token + Token Cache")
            {
                field("Access Token"; AccessToken)
                {
                    ApplicationArea = All;
                    Caption = 'Token';
                }
                field("Token Cache"; TokenCache)
                {
                    ApplicationArea = All;
                    Caption = 'Cache';
                }
            }
            repeater("Data Content")
            {
                ShowCaption = false;
                field(ID; Rec.ID)
                {
                    ApplicationArea = All;
                }
                field(Scope; Rec.Scope)
                {
                    ApplicationArea = All;
                }
                field("Access Token Status"; Rec.Status)
                {
                    ApplicationArea = All;
                    Editable = false;
                }
                field(Error; Rec.Error)
                {
                    ApplicationArea = All;
                    Editable = false;
                }
            }
            group("Status")
            {
                ShowCaption = false;

                field("Status Message"; StatusMessage)
                {
                    ApplicationArea = All;
                    ShowCaption = false;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action("Acquire Initial Token")
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    PromptInteraction: Enum "Prompt Interaction";
                    Error: Text;
                    Scopes: List of [Text];
                begin
                    Scopes.Add(StrSubstNo('%1/.default', AadClientId));
                    if not OAuth2.AcquireTokenAndTokenCacheByAuthorizationCode(
                            AadClientId, AadClientSecret,
                            AuthorityUrl, RedirectUrl,
                            Scopes, PromptInteraction::Login,
                            AccessToken, TokenCache, Error) then begin
                        SetStatus(StrSubstNo('Error: %1', Error));
                        exit;
                    end;

                    if Error <> '' then
                        SetStatus(StrSubstNo('Error on token acquisition: %1', Error))
                    else
                        SetStatus('Token acquired');
                end;
            }

            action("Acquire On-Behalf Access Token")
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    TempAccessToken: Text;
                    Scopes: List of [Text];
                    NewTokenCache: Text;
                begin
                    Scopes.Add(Rec.Scope);
                    if not OAuth2.AcquireOnBehalfOfTokenByTokenCache(
                            AadClientId, AadClientSecret, LoginHint, '', Scopes, TokenCache, TempAccessToken, NewTokenCache) then begin
                        SetStatus(StrSubstNo('Error Acquiring Token for %1', Rec.Scope));
                        exit;
                    end;

                    if TempAccessToken <> '' then begin
                        SetStatus(StrSubstNo('Token acquired for %1, token cache renewed.', Rec.Scope));
                        TokenCache := NewTokenCache;
                    end else
                        SetStatus(StrSubstNo('Cannot acquire token for %1', Rec.Scope));

                    Rec.SetResult(TempAccessToken, '');
                    Rec.Modify(false);
                end;
            }

            action("See Access Token for Scope")
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    PromptInteraction: Enum "Prompt Interaction";
                begin
                    Message(Rec.GetResult());
                end;
            }
        }
    }

    var
        OAuth2: Codeunit OAuth2;
        AadClientId: Text;
        AadClientSecret: Text;
        AuthorityUrl: Text;
        RedirectUrl: Text;
        AppIdUrl: Text;
        AccessToken: Text;
        TokenCache: Text;
        StatusMessage: Text;
        LoginHint: Text;

    local procedure SetStatus(Status: Text)
    begin
        StatusMessage := StrSubstNo('[%1]: %2', CurrentDateTime, Status);
    end;
}
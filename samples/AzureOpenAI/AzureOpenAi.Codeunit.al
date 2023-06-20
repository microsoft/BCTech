codeunit 50100 "Azure OpenAi"
{
    var
        EndpointKeyTok: Label 'AOAI-Endpoint', Locked = true;
        SecretKeyTok: Label 'AOAI-Secret', Locked = true;
        TestPromptTxt: Label 'A user has been setting up an integration between Dynamics 365 Business Central and Azure OpenAI to utilize GPT for an AI integration. Tell the user that the service integration connection between Azure OpenAI and Dynamics 365 Business Central was successful in a friendly manner.%1%1Message:', Locked = true;
        CompletionFailedErr: Label 'The completion did not return a success status code.';
        CompletionFailedWithCodeErr: Label 'The completion failed with status code %1', Comment = '%1 = the error status code';
        MissingSecretQst: Label 'The secret has not been set. Would you like to open the setup page?';
        MissingEndpointQst: Label 'The endpoint has not been set. Would you like to open the setup page?';

    /// <summary>
    /// Generates a completion for a given prompt with a default temperature and max token count.
    /// </summary>
    /// <param name="Prompt">The prompt to generate the completion for.</param>
    /// <returns>The completion to the prompt.</returns>
    procedure GenerateCompletion(Prompt: Text): Text
    begin
        exit(GenerateCompletion(Prompt, 1000, 0.7));
    end;

    /// <summary>
    /// Generates a completion for a given prompt with a default temperature and max token count.
    /// </summary>
    /// <param name="Prompt">The prompt to generate the completion for.</param>
    /// <param name="MaxTokens">The maximum number of tokens to user for the request.</param>
    /// <param name="Temperature">The temperature to user for the request.</param>
    /// <returns>The completion to the prompt.</returns>
    procedure GenerateCompletion(Prompt: Text; MaxTokens: Integer; Temperature: Decimal): Text
    var
        Configuration: JsonObject;
        Payload: Text;
        Completion: Text;
        TokenLimit: Integer;
        StatusCode: Integer;
        NewLineChar: Char;
    begin
        if not HasEndpoint() then begin
            if Confirm(MissingEndpointQst) then
                Page.Run(Page::"Azure OpenAi Setup");
            Error('');
        end;

        if not HasSecret() then begin
            if Confirm(MissingSecretQst) then
                Page.Run(Page::"Azure OpenAi Setup");
            Error('');
        end;

        TokenLimit := 1000;

        if (MaxTokens < 1) or (MaxTokens > TokenLimit) then
            MaxTokens := TokenLimit;

        if Temperature < 0 then
            Temperature := 0;

        if Temperature > 1 then
            Temperature := 1;

        NewLineChar := 10;
        Prompt := Prompt.Replace('\', NewLineChar);

        Configuration.Add('prompt', Prompt);
        Configuration.Add('max_tokens', MaxTokens);
        Configuration.Add('temperature', Temperature);

        Configuration.WriteTo(Payload);

        if not SendRequest(Payload, StatusCode, Completion) then
            Error(CompletionFailedWithCodeErr, StatusCode);

        if StrLen(Completion) > 1 then
            Completion := CopyStr(Completion, 2, StrLen(Completion) - 2);
        Completion := Completion.Replace('\n', NewLineChar);
        Completion := DelChr(Completion, '<>', ' ');
        Completion := Completion.Replace('\"', '"');
        Completion := Completion.Trim();

        exit(Completion);
    end;

    procedure TestPrompt()
    var
        Completion: Text;
        NewLineChar: Char;
    begin
        NewLineChar := 10;

        Completion := GenerateCompletion(StrSubstNo(TestPromptTxt, NewLineChar));
        Completion := DelChr(Completion, '<>', '"').Trim();
        if Completion <> '' then
            Message(Completion.Trim())
        else
            Error(CompletionFailedErr);
    end;

    procedure HasSecret(): Boolean
    begin
        exit(GetSecret() <> '');
    end;

    procedure SetSecret(Secret: Text)
    begin
        if EncryptionEnabled() then
            IsolatedStorage.SetEncrypted(SecretKeyTok, Secret, DataScope::Module)
        else
            IsolatedStorage.Set(SecretKeyTok, Secret, DataScope::Module);
    end;

    procedure HasEndpoint(): Boolean
    begin
        exit(GetEndpoint() <> '');
    end;

    procedure SetEndpoint(Endpoint: Text)
    begin
        if EncryptionEnabled() then
            IsolatedStorage.SetEncrypted(EndpointKeyTok, Endpoint, DataScope::Module)
        else
            IsolatedStorage.Set(EndpointKeyTok, Endpoint, DataScope::Module);
    end;

    procedure GetEndpoint(): Text
    var
        Endpoint: Text;
    begin
        if IsolatedStorage.Get(EndpointKeyTok, DataScope::Module, Endpoint) then;
        exit(Endpoint);
    end;

    [TryFunction]
    local procedure SendRequest(Payload: Text; var StatusCode: Integer; var Completion: Text)
    var
        HttpClient: HttpClient;
        HttpRequestMessage: HttpRequestMessage;
        HttpResponseMessage: HttpResponseMessage;
        RequestHeaders: HttpHeaders;
        HttpContent: HttpContent;
        ResponseText: Text;
        ResponseJson: JsonObject;
        CompletionToken: JsonToken;
    begin
        HttpRequestMessage.Method('POST');
        HttpRequestMessage.SetRequestUri(GetEndpoint());

        HttpClient.DefaultRequestHeaders().Add('api-key', GetSecret());

        HttpContent.WriteFrom(Payload);

        HttpContent.GetHeaders(RequestHeaders);
        RequestHeaders.Remove('Content-Type');
        RequestHeaders.Add('Content-Type', 'application/json');

        HttpRequestMessage.Content(HttpContent);

        HttpClient.Send(HttpRequestMessage, HttpResponseMessage);

        StatusCode := HttpResponseMessage.HttpStatusCode();
        if not HttpResponseMessage.IsSuccessStatusCode() then
            Error(CompletionFailedErr);

        HttpResponseMessage.Content().ReadAs(ResponseText);
        ResponseJson.ReadFrom(ResponseText);
        ResponseJson.SelectToken('$.choices[:].text', CompletionToken);
        CompletionToken.WriteTo(Completion);
    end;

    local procedure GetSecret(): Text
    var
        Secret: Text;
    begin
        if IsolatedStorage.Get(SecretKeyTok, DataScope::Module, Secret) then;
        exit(Secret);
    end;
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace System.TestLibraries.AdversarialSimulation;

codeunit 135396 "Adversarial Simulation Impl."
{
    Access = Internal;

    var
        SimulationPathTxt: Label '/simulation', Locked = true;
        AdversarialQuestionAndAnswerScenarioTok: Label 'ADVERSARIAL_QA', Locked = true;
        AdversarialConversationScenarioTok: Label 'ADVERSARIAL_CONVERSATION', Locked = true;
        AdversarialXPIAScenarioTok: Label 'ADVERSARIAL_INDIRECT_JAILBREAK', Locked = true;
        DefaultSimulationUriTxt: Label 'http://localhost:8000', Locked = true;
        BaseSimulationUri: Text;
        Simulation: Text;
        SimulationResultNumber: Integer;
        ConversationTurnNumber: Integer;
        RandomizationSeed: Integer;

    procedure SetSeed(Seed: Integer)
    begin
        RandomizationSeed := Seed;
    end;

    procedure Start()
    begin
        StartInternal(AdversarialQuestionAndAnswerScenarioTok, 1, false)
    end;

    procedure Start(MaxConversationTurns: Integer)
    begin
        StartInternal(AdversarialConversationScenarioTok, MaxConversationTurns, false);
    end;

    procedure StartUPIA()
    begin
        StartInternal(AdversarialQuestionAndAnswerScenarioTok, 1, true)
    end;

    procedure StartUPIA(MaxConversationTurns: Integer)
    begin
        StartInternal(AdversarialConversationScenarioTok, MaxConversationTurns, true);
    end;

    procedure StartXPIA()
    begin
        StartInternal(AdversarialXPIAScenarioTok, 1, false);
    end;

    local procedure StartInternal(Scenario: Text; MaxConversationTurns: Integer; DirectJailbreak: Boolean)
    var
        SimulationHttpClient: HttpClient;
        HttpRequestMessage: HttpRequestMessage;
        HttpResponseMessage: HttpResponseMessage;
        HttpHeaders: HttpHeaders;
        RequestJson: JsonObject;
        RequestText: Text;
        ResponseJson: JsonObject;
        ResponseText: Text;
        JsonToken: JsonToken;
        StartSimulationErr: Label 'Failed to start simulation: %1', Comment = '%1 = reason';
    begin
        if (Scenario = AdversarialConversationScenarioTok) and (MaxConversationTurns < 2) then
            error('The maximum number of conversation turns for multi-turn simulation must be at least 2.');

        HttpRequestMessage.Method('POST');
        HttpRequestMessage.SetRequestUri(GetBaseSimulationUri());

        RequestJson.Add('scenario', Scenario);
        RequestJson.Add('max_conversation_turns', MaxConversationTurns);
        RequestJson.Add('max_simulation_results', GetMaxSimulationResults());
        RequestJson.Add('randomization_seed', RandomizationSeed);
        RequestJson.Add('upia', DirectJailbreak);
        RequestJson.WriteTo(RequestText);
        HttpRequestMessage.Content.WriteFrom(RequestText);

        HttpRequestMessage.Content.GetHeaders(HttpHeaders);
        if HttpHeaders.Contains('Content-Type') then
            HttpHeaders.Remove('Content-Type');
        HttpHeaders.Add('Content-Type', 'application/json');

        SimulationHttpClient.Send(HttpRequestMessage, HttpResponseMessage);
        if not HttpResponseMessage.IsSuccessStatusCode then
            Error(StartSimulationErr, HttpResponseMessage.ReasonPhrase);

        HttpResponseMessage.Content.ReadAs(ResponseText);
        ResponseJson.ReadFrom(ResponseText);
        ResponseJson.Get('id', JsonToken);
        Simulation := JsonToken.AsValue().AsText();
    end;

    local procedure GetMaxSimulationResults(): Integer
    begin
        // TODO: currently max across all scenarios (cf. https://learn.microsoft.com/en-us/azure/ai-studio/how-to/develop/simulator-interaction-data),
        // but should be input size (+ 1 for peeking)
        exit(1015);
    end;

    procedure HasNextTurn(): Boolean
    var
        NextSimulationResultNumber: Integer;
        NextConversationTurnNumber: Integer;
        XPIAAttackSentence: Text;
        Harm: Text;
    begin
        if SimulationResultNumber = 0 then
            exit(true);

        GetOrPeekHarmInternal(Harm, NextSimulationResultNumber, NextConversationTurnNumber, XPIAAttackSentence, true);
        exit(NextSimulationResultNumber = SimulationResultNumber);
    end;

    procedure GetHarm(): Text
    var
        Harm: Text;
        XPIAAttackSentence: Text;
    begin
        GetOrPeekHarmInternal(Harm, SimulationResultNumber, ConversationTurnNumber, XPIAAttackSentence, false);

        exit(harm);
    end;

    procedure GetHarmWithXPIA(var XPIAAttackSentence: Text): Text
    var
        Harm: Text;
    begin
        if not GetOrPeekHarmInternal(Harm, SimulationResultNumber, ConversationTurnNumber, XPIAAttackSentence, false) then
            Error('No XPIA attack sentence found');

        exit(harm);
    end;

    local procedure GetOrPeekHarmInternal(
        var Harm: Text;
        var LocalSimulationResultNumber: Integer;
        var LocalConversationTurnNumber: Integer;
        var XPIAAttackSentence: Text;
        Peek: Boolean) HasXPIA: Boolean
    var
        SimulationHttpClient: HttpClient;
        HttpRequestMessage: HttpRequestMessage;
        HttpResponseMessage: HttpResponseMessage;
        ResponseJson: JsonObject;
        ResponseText: Text;
        JsonToken: JsonToken;
        PeekPathTxt: Label '%1/%2/queries/peek', Locked = true;
        GetPathTxt: Label '%1/%2/queries', Locked = true;
        GetHarmErr: Label 'Failed to get simulated harm: %1', Comment = '%1 = reason';
    begin
        HttpRequestMessage.Method('GET');
        if Peek then
            HttpRequestMessage.SetRequestUri(StrSubstNo(PeekPathTxt, GetBaseSimulationUri(), Simulation))
        else
            HttpRequestMessage.SetRequestUri(StrSubstNo(GetPathTxt, GetBaseSimulationUri(), Simulation));

        SimulationHttpClient.Send(HttpRequestMessage, HttpResponseMessage);
        if not HttpResponseMessage.IsSuccessStatusCode then
            Error(GetHarmErr, HttpResponseMessage.ReasonPhrase);

        HttpResponseMessage.Content.ReadAs(ResponseText);
        ResponseJson.ReadFrom(ResponseText);
        ResponseJson.Get('query', JsonToken);
        Harm := JsonToken.AsValue().AsText();
        ResponseJson.Get('conversation_turn_number', JsonToken);
        LocalConversationTurnNumber := JsonToken.AsValue().AsInteger();
        ResponseJson.Get('simulation_result_number', JsonToken);
        LocalSimulationResultNumber := JsonToken.AsValue().AsInteger();
        if ResponseJson.Contains('xpia_attack_sentence') then begin
            ResponseJson.Get('xpia_attack_sentence', JsonToken);
            XPIAAttackSentence := JsonToken.AsValue().AsText();
            HasXPIA := true;
        end;
    end;

    procedure RespondWithStopWord()
    var
        StopwordLbl: Label 'GOODBYE!', Locked = true;
    begin
        Respond(StopwordLbl);
    end;

    procedure Respond(Response: Text)
    var
        SimulationHttpClient: HttpClient;
        HttpRequestMessage: HttpRequestMessage;
        HttpResponseMessage: HttpResponseMessage;
        HttpHeaders: HttpHeaders;
        RequestJson: JsonObject;
        RequestText: Text;
        ResponsesPathTxt: Label '%1/%2/responses', Locked = true;
        ResponseSendErr: Label 'Failed to send response to simulated harm: %1', Comment = '%1 = reason';
    begin
        HttpRequestMessage.Method('PUT');
        HttpRequestMessage.SetRequestUri(StrSubstNo(ResponsesPathTxt, GetBaseSimulationUri(), Simulation));

        RequestJson.Add('response', Response);
        RequestJson.WriteTo(RequestText);
        HttpRequestMessage.Content.WriteFrom(RequestText);
        HttpRequestMessage.Content.GetHeaders(HttpHeaders);
        if HttpHeaders.Contains('Content-Type') then
            HttpHeaders.Remove('Content-Type');
        HttpHeaders.Add('Content-Type', 'application/json');

        SimulationHttpClient.Send(HttpRequestMessage, HttpResponseMessage);
        if not HttpResponseMessage.IsSuccessStatusCode then
            Error(ResponseSendErr, HttpResponseMessage.ReasonPhrase);
    end;

    procedure SetBaseSimulationUri(Uri: Text)
    begin
        BaseSimulationUri := Uri;
    end;

    local procedure GetBaseSimulationUri(): Text
    begin
        if BaseSimulationUri = '' then
            exit(DefaultSimulationUriTxt + SimulationPathTxt);

        exit(BaseSimulationUri + SimulationPathTxt);
    end;
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50110 GreetingsManagement
{
    var
        SubscriptionKeyTxt: label '<your own key>', Locked = true;

    procedure GetRandomGreeting(): Text;
    var
        LanguageID: Text;
        LanguageName: Text;
    begin
        Randomize();
        LanguageID := GetRandomLanguage(LanguageName);
        exit(LanguageName + ': ' + Translate(LanguageID, 'Hello World'));
    end;

    local procedure GetRandomLanguage(var LanguageName: Text) LanguageID: Text;
    var
        JObject: JsonObject;
        JToken: JsonToken;
        Client: HttpClient;
        Response: HttpResponseMessage;
        Result: Text;
    begin
        Client.Get('https://api.cognitive.microsofttranslator.com/languages?api-version=3.0&scope=translation', Response);

        Response.Content().ReadAs(Result);
        JObject.ReadFrom(Result);
        JObject.SelectToken('$.translation', JToken);
        JObject := JToken.AsObject();
        JObject.Keys().Get(Random(JObject.Keys().Count()), LanguageID);
        JObject.Get(LanguageID, JToken);
        JObject := JToken.AsObject();
        JObject.Get('name', JToken);
        LanguageName := JToken.AsValue().AsText();
    end;

    // Translate Text to TargetLanguage.
    // API Documentation: https://learn.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-translate
    local procedure Translate(TargetLanguage: Text; Text: Text): Text;
    var
        JArray: JsonArray;
        JObject: JsonObject;
        JToken: JsonToken;
        Client: HttpClient;
        Request: HttpRequestMessage;
        Response: HttpResponseMessage;
        Headers: HttpHeaders;
    begin
        // Set the HTTP method and the resource uri.
        Request.Method := 'POST';
        Request.SetRequestUri(StrSubstNo('https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&to=%1', TargetLanguage));

        // Add the subscription key the Request Headers
        Request.GetHeaders(Headers);
        Headers.Add('Ocp-Apim-Subscription-Key', SubscriptionKeyTxt);

        // Add the text to translate as Json to the content
        JObject.Add('Text', Text);
        JArray.Add(JObject);
        Request.Content().WriteFrom(Format(JArray));

        // Add the Content-Type to the Content headesr, but først remove
        // the existing Content-Type.
        Request.Content().GetHeaders(Headers);
        Headers.Remove('Content-Type');
        Headers.Add('Content-Type', 'application/json');

        Client.Send(Request, Response);

        // Read the response
        Response.Content().ReadAs(Text);
        JArray.ReadFrom(Text);
        JArray.SelectToken(StrSubstNo('$..translations[?(@.to == ''%1'')].text', TargetLanguage), JToken);
        exit(JToken.AsValue().AsText());
    end;
}
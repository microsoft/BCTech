// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

// Remark: This code doesn't re-use any of the currencies feature of Business Central for demo purpose.
codeunit 50100 CurrencyRetriever
{
    trigger OnRun()
    var
        date: Text;
        currencyBase: Text;
        currencies: Text; // Commas separated list of currencies to get
        results: Dictionary of [Text, Text];
        jsonContentText: Text;
        sleepSimulation: Integer;
    begin
        // Getting the page background tasks parameters
        date := Page.GetBackgroundParameters().Get('Date');
        currencyBase := Page.GetBackgroundParameters().Get('CurrencyBase');
        currencies := Page.GetBackgroundParameters().Get('Currencies');

        // For demo purpose of slow calls
        if (Page.GetBackgroundParameters().ContainsKey('SleepSimulation')) then begin
            Evaluate(sleepSimulation, Page.GetBackgroundParameters().Get('SleepSimulation'));
            Sleep(sleepSimulation);
        end;

        // Getting the exchange rates.
        // Remark: For testability, the Http request can be mocked.
        if Page.GetBackgroundParameters().ContainsKey('MockHttpResponse') then
            jsonContentText := Page.GetBackgroundParameters().Get('MockHttpResponse')
        else
            jsonContentText := GetExchangeRate(date, currencyBase, currencies);

        // Parsing the exchange rates
        if (jsonContentText <> 'null') then
            ParseExchangeRate(results, jsonContentText);

        // Setting the page background task result
        Page.SetBackgroundTaskResult(results);
    end;

    // Parse exchange rates through exchangeratesapi.io
    local procedure GetExchangeRate(Date: Text; CurrencyBase: Text; Currencies: Text): Text
    var
        client: HttpClient;
        responseMessage: HttpResponseMessage;
        jsonContentText: Text;
        Url: Text;
        RequestErr: Label 'An error occured when trying to get the exchange rates: \\%1:\\%2';
        Currency: Text;
        CurrencyList: List of [Text];
        SupportedCurrencies: Text;
        TextBuilder: TextBuilder;
    begin
        // Removing all non-supported currencies from the request
        SupportedCurrencies := 'EUR,USD,JPY,BGN,CZK,DKK,GBP,HUF,PLN,RON,SEK,CHF,ISK,NOK,HRK,RUB,TRY,AUD,BRL,CAD,CNY,HKD,IDR,ILS,INR,KRW,MXN,MYR,NZD,PHP,SGD,THB,ZAR';
        if not SupportedCurrencies.Contains(CurrencyBase) then
            exit('null');

        CurrencyList := Currencies.Split(',');
        foreach currency in CurrencyList do
            if SupportedCurrencies.Contains(currency) then begin
                TextBuilder.Append(currency);
                TextBuilder.Append(',');
            end;

        if TextBuilder.Length > 0 then
            TextBuilder.Length(TextBuilder.Length - 1)
        else
            exit('null');

        // Querying
        Url := 'https://api.exchangeratesapi.io/' + Date + '?base=' + CurrencyBase + '&symbols=' + TextBuilder.ToText();
        client.Get(Url, responseMessage);

        if not responseMessage.IsSuccessStatusCode() then
            Error(RequestErr, responseMessage.HttpStatusCode, responseMessage.ReasonPhrase);

        responseMessage.Content.ReadAs(jsonContentText);
        exit(jsonContentText);
    end;

    // Structure: {"rates":{"USD":decimal,"GBP":decimal},"base":"DKK","date":"2019-09-26"}
    local procedure ParseExchangeRate(var results: Dictionary of [Text, Text]; jsonContentText: Text)
    var
        jsonObject: JsonObject;
        jsonToken: JsonToken;
        jsonArray: JsonArray;
        i: Decimal;
        currentCurrency: Text;
        WrongFormatErr: Label 'The response is not formatted correctly.';
    begin
        if not jsonObject.ReadFrom(jsonContentText) then
            Error(WrongFormatErr);

        GetJsonToken(jsonObject, 'rates', jsonToken);
        jsonObject := jsonToken.AsObject();

        for i := 1 to jsonObject.Keys.Count do begin
            currentCurrency := jsonObject.Keys.Get(i);
            GetJsonToken(jsonObject, currentCurrency, jsonToken);
            results.Set(currentCurrency, Format(jsonToken.AsValue().AsDecimal()));
        end;
    end;

    local procedure GetJsonToken(JsonObject: JsonObject; KeyText: Text; VAR JsonToken: JsonToken)
    var
        CannotFindKeyErr: Label 'Cannot find the following key: %1';
    begin
        if not JsonObject.Get(KeyText, JsonToken) then
            Error(CannotFindKeyErr, KeyText);
    end;
}

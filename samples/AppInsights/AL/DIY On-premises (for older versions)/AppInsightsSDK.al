codeunit 50110 AppInsightsSDK
{
    var
        AppInsightKey: Text;
        URL: label 'https://dc.services.visualstudio.com/v2/track', Locked = true;

    procedure Init(apiKey: Text)
    begin
        AppInsightKey := apiKey;
    end;

    //
    // Track Event
    //
    procedure TrackEvent(eventName: Text; properties: Dictionary of [Text, Text]; metrics: Dictionary of [Text, Decimal]): Text
    var
        JSONEventData: JsonObject;
        Result: Text;
    begin
        JSONEventData.Add('name', eventName);
        Result := Track('EventData', JSONEventData, properties, metrics);
        Exit(Result);
    end;

    //
    // Track Trace 
    //
    procedure TrackTrace(message: Text; properties: Dictionary of [Text, Text]): Text
    var
        JSONEventData: JsonObject;
        EmptyDictionary: Dictionary of [Text, Decimal];
        Result: Text;
    begin
        JSONEventData.Add('message', message);
        Result := Track('MessageData', JSONEventData, properties, EmptyDictionary);
        Exit(Result);
    end;

    //
    // Track page view
    //
    procedure TrackPageView(pageName: Text): Text
    var
        JSONEventData: JsonObject;
        EmptyMetrics: Dictionary of [Text, Decimal];
        EmptyProperties: Dictionary of [Text, Text];
        Result: Text;
    begin
        JSONEventData.Add('name', pageName);
        JSONEventData.Add('duration', '00:00:00');
        Result := Track('PageViewData', JSONEventData, EmptyProperties, EmptyMetrics);
        Exit(Result);
    end;

    //
    // Track exception
    //
    procedure TrackException(message: Text; properties: Dictionary of [Text, Text]; metrics: Dictionary of [Text, Decimal]): Text
    var
        JSONEventData: JsonObject;
        JSONExceptions: JsonArray;
        JSONException: JsonObject;
        Result: Text;
    begin
        JSONException.Add('typeName', 'AL.ExtensionException');
        JSONException.Add('message', message);
        JSONExceptions.Add(JSONException);
        JSONEventData.Add('exceptions', JSONExceptions);
        Result := Track('ExceptionData', JSONEventData, properties, metrics);
        Exit(Result);
    end;

    //
    // Track core
    //
    local procedure Track(baseEventType: Text; JSONEventData: JsonObject; properties: Dictionary of [Text, Text]; metrics: Dictionary of [Text, Decimal]): Text
    var
        Client: HttpClient;
        Response: HttpResponseMessage;
        Headers: HttpHeaders;
        Content: HttpContent;
        Result: Text;
        JArray: JsonArray;
        JSON: JsonObject;
        JSONData: JsonObject;
        JSONProperties: JsonObject;
        JSONMeasurements: JsonObject;
        JSONText: Text;
        TimeStamp: Text;
        ValueName: Text;
        PropValue: Text;
        MetricValue: Decimal;
    begin
        // Build the JSON data
        JSON.Add('name', 'ALExtension.Event');
        TimeStamp := Format(CurrentDateTime(), 0, '<Year4>-<Month,2>-<Day,2>T<Hours24,2>:<Minutes,2>:<Seconds,2><Second dec.>');
        JSON.Add('time', TimeStamp);
        JSON.Add('iKey', AppInsightKey);

        JSONData.Add('baseType', baseEventType);
        JSONEventData.Add('ver', 2);

        if (properties.Count() > 0) then begin
            foreach ValueName in properties.Keys() do begin
                properties.Get(ValueName, PropValue);
                JSONProperties.Add(ValueName, PropValue)
            end;
            JSONEventData.Add('properties', JSONProperties);
        end;

        if (metrics.Count() > 0) then begin
            foreach ValueName in metrics.Keys() do begin
                metrics.Get(ValueName, MetricValue);
                JSONMeasurements.Add(ValueName, MetricValue)
            end;
            JSONEventData.Add('measurements', JSONMeasurements);
        end;

        JSONData.Add('baseData', JSONEventData);
        JSON.Add('data', JSONData);

        // For debugging
        JSON.WriteTo(JSONText);
        Message('JSON: %1', JSONText);

        // Add the JSON data to the request content
        JArray.Add(JSON);
        Content.Clear();
        Content.WriteFrom(Format(JSON));

        // Add the Content-Type to the Content headers, but first remove
        // the existing Content-Type.
        Content.GetHeaders(Headers);
        Headers.Remove('Content-Type');
        Headers.Add('Content-Type', 'application/json');

        Client.Post(URL, Content, Response);
        Response.Content().ReadAs(Result);
        Exit(Result);
    end;

}
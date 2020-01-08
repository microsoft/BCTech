codeunit 50114 TestAppInsights
{
    TableNo = AppInsightsEventTestData;

    var
        AppInsightsInstance: Codeunit AppInsightsSDK;
        apiKey: label '5be7a45c-81b6-4505-affe-479b464da3e4', Locked = true;

    procedure Init()
    begin
        AppInsightsInstance.Init(apiKey);
    end;

    procedure TrackEvent(var EventTestData: Record AppInsightsEventTestData)
    var
        Result: Text;
        Properties: Dictionary of [Text, Text];
        Metric: Dictionary of [Text, Decimal];
    begin

        Properties.Add(EventTestData.Property1Name, EventTestData.Property1Value);
        Properties.Add(EventTestData.Property2Name, EventTestData.Property2Value);
        Properties.Add(EventTestData.Property3Name, EventTestData.Property3Value);

        Metric.Add(EventTestData.Metric1Name, EventTestData.Metric1Value);
        Metric.Add(EventTestData.Metric2Name, EventTestData.Metric2Value);
        Metric.Add(EventTestData.Metric3Name, EventTestData.Metric3Value);

        Result := AppInsightsInstance.TrackEvent(EventTestData.EventText, Properties, Metric);
        Message('TrackEvent emited. Response: %1', Result);
    end;

    procedure TrackTrace(var EventTestData: Record AppInsightsEventTestData)
    var
        Result: Text;
        Properties: Dictionary of [Text, Text];
    begin
        Properties.Add(EventTestData.Property1Name, EventTestData.Property1Value);
        Properties.Add(EventTestData.Property2Name, EventTestData.Property2Value);
        Properties.Add(EventTestData.Property3Name, EventTestData.Property3Value);

        Result := AppInsightsInstance.TrackTrace(EventTestData.EventText, Properties);
        Message('TrackTrace emited. Response: %1', Result);
    end;

    procedure TrackPageView(var EventTestData: Record AppInsightsEventTestData)
    var
        Result: Text;
        Properties: Dictionary of [Text, Text];
    begin

        Result := AppInsightsInstance.TrackPageView(EventTestData.EventText);
        Message('TrackPageView emited. Response: %1', Result);
    end;

    procedure TrackException(var EventTestData: Record AppInsightsEventTestData)
    var
        Result: Text;
        Properties: Dictionary of [Text, Text];
        Metric: Dictionary of [Text, Decimal];
    begin
        Properties.Add(EventTestData.Property1Name, EventTestData.Property1Value);
        Properties.Add(EventTestData.Property2Name, EventTestData.Property2Value);
        Properties.Add(EventTestData.Property3Name, EventTestData.Property3Value);

        Metric.Add(EventTestData.Metric1Name, EventTestData.Metric1Value);
        Metric.Add(EventTestData.Metric2Name, EventTestData.Metric2Value);
        Metric.Add(EventTestData.Metric3Name, EventTestData.Metric3Value);

        Result := AppInsightsInstance.TrackException(EventTestData.EventText, Properties, Metric);
        Message('TrackException emited. Response: %1', Result);
    end;
}
page 50114 TestAppInsightsPage
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = AppInsightsEventTestData;
    SourceTableTemporary = true;
    Editable = true;
    Caption = 'Test Application Insights';

    layout
    {
        area(Content)
        {
            group(EventNameGroup)
            {
                Caption = 'Event name';
                field(EventText; EventText)
                {
                    ApplicationArea = All;
                    Caption = 'Event name/Message/Page name';
                }
            }

            group(EventProperties)
            {
                Caption = 'Event properties';
                field(Property1Name; Property1Name)
                {
                    ApplicationArea = All;
                    Caption = 'Prop1 name';
                }
                field(Property2Name; Property2Name)
                {
                    ApplicationArea = All;
                    Caption = 'Prop2 name';
                }
                field(Property3Name; Property3Name)
                {
                    ApplicationArea = All;
                    Caption = 'Prop3 name';
                }
                field(Property1Value; Property1Value)
                {
                    ApplicationArea = All;
                    Caption = 'Prop1 value';
                }
                field(Property2Value; Property2Value)
                {
                    ApplicationArea = All;
                    Caption = 'Prop2 value';
                }
                field(Property3Value; Property3Value)
                {
                    ApplicationArea = All;
                    Caption = 'Prop1 value';
                }
            }

            group(EventMetrics)
            {
                Caption = 'Event metrics';
                field(Metric1Name; Metric1Name)
                {
                    ApplicationArea = All;
                    Caption = 'Metric1 name';
                }
                field(Metric2Name; Metric2Name)
                {
                    ApplicationArea = All;
                    Caption = 'Metric2 name';
                }
                field(Metric3Name; Metric3Name)
                {
                    ApplicationArea = All;
                    Caption = 'Metric3 name';
                }
                field(Metric1Value; Metric1Value)
                {
                    ApplicationArea = All;
                    Caption = 'Metric1 value';
                }
                field(Metric2Value; Metric2Value)
                {
                    ApplicationArea = All;
                    Caption = 'Metric2 value';
                }
                field(Metric3Value; Metric3Value)
                {
                    ApplicationArea = All;
                    Caption = 'Metric3 value';
                }
            }
        }
    }

    actions
    {
        area(Creation)
        {
            action(TrackEvent)
            {
                ApplicationArea = All;
                Caption = 'Emit track event';
                Promoted = true;
                PromotedIsBig = true;
                PromotedOnly = true;
                Image = Action;
                trigger OnAction()
                begin
                    TestAppInsightsMgt.TrackEvent(Rec);
                end;
            }

            action(TrackTrace)
            {
                ApplicationArea = All;
                Caption = 'Emit track trace';
                Promoted = true;
                PromotedIsBig = true;
                PromotedOnly = true;
                Image = Action;
                trigger OnAction()
                begin
                    TestAppInsightsMgt.TrackTrace(Rec);
                end;
            }

            action(TrackPageView)
            {
                ApplicationArea = All;
                Caption = 'Emit track page view';
                Promoted = true;
                PromotedIsBig = true;
                PromotedOnly = true;
                Image = Action;
                trigger OnAction()
                begin
                    TestAppInsightsMgt.TrackPageView(Rec);
                end;
            }

            action(TrackException)
            {
                ApplicationArea = All;
                Caption = 'Emit track exception';
                Promoted = true;
                PromotedIsBig = true;
                PromotedOnly = true;
                Image = Action;
                trigger OnAction()
                begin
                    TestAppInsightsMgt.TrackException(Rec);
                end;
            }
        }
    }

    var
        TestAppInsightsMgt: Codeunit TestAppInsights;

    trigger OnInit()
    begin
        TestAppInsightsMgt.Init();
        rec.EventText := 'Some event';
        rec.Property1Name := 'Prop1';
        rec.Property1Value := 'Prop1Value';
        rec.Property2Name := 'Prop2';
        rec.Property2Value := 'Prop2Value';
        rec.Property3Name := 'Prop3';
        rec.Property3Value := 'Prop3Value';

        Randomize();
        rec.Metric1Name := 'Metric1';
        rec.Metric1Value := Random(100);
        rec.Metric2Name := 'Metric2';
        rec.Metric2Value := Random(100);
        rec.Metric3Name := 'Metric3';
        rec.Metric3Value := Random(100);

        CurrPage.Update();
    end;
}
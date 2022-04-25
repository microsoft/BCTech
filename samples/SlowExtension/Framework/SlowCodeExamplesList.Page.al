// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The list of examples of slowly running code.
/// </summary>
page 50101 "Slow Code Examples List"
{
    Caption = 'Slow Code Examples List';
    PageType = ListPart;
    UsageCategory = Administration;
    SourceTable = "Slow Code Example";

    layout
    {
        area(Content)
        {

            repeater("Slow Code Examples")
            {
                ShowCaption = false;
                FreezeColumn = DisplayText;
                Editable = false;
                IndentationColumn = Indentation;
                IndentationControls = DisplayText;
                ShowAsTree = true;

                field(DisplayText; Rec."Display Text")
                {
                    ApplicationArea = All;
                    Editable = false;
                    Caption = 'Examples';
                    StyleExpr = Style;

                    trigger OnDrillDown()
                    var
                        SlowCodeExample: Interface "Slow Code Example";
                        ScenarioName: Text;
                    begin
                        SlowCodeExample := Rec."Slow Code Example";
                        ScenarioName := Enum::"Slow Code Examples".Names().Get(Rec."Slow Code Example".AsInteger());

                        case Rec."Entry Type" of
                            Rec."Entry Type"::RunCode:
                                RunSlowCode(SlowCodeExample, ScenarioName);
                            Rec."Entry Type"::Hint:
                                Message(SlowCodeExample.GetHint());
                        end
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        SlowCodeExamples: Codeunit "Slow Code Examples";
    begin
        SlowCodeExamples.Get(Rec);
    end;

    trigger OnAfterGetRecord()
    begin
        if Rec."Entry Type" = Rec."Entry Type"::Name then begin
            Indentation := 0;
            Style := 'Strong';
        end else begin
            Indentation := 1;
            Style := 'StandardAccent';
        end;
    end;

    local procedure RunSlowCode(SlowCodeExample: Interface "Slow Code Example"; ScenarioName: Text)
    var
        ExampleStartDateTime: DateTime;
    begin
        if SlowCodeExample.IsBackground() then begin
            SlowCodeExample.RunSlowCode();
            Message(BackgroundRunTxt, ScenarioName);
        end else begin
            ExampleStartDateTime := CurrentDateTime();
            SlowCodeExample.RunSlowCode();
            Message(ForegroundRunTxt, ScenarioName, CurrentDateTime() - ExampleStartDateTime);
        end;
    end;

    var
        Style: Text;
        Indentation: Integer;
        BackgroundRunTxt: Label 'Scenario ''%1'' is being executed in the background - where is the time spent?';
        ForegroundRunTxt: Label 'Scenario ''%1'' took %2 to execute - where was the time spent?';
        BackgroundRun: Label 'Something is running in the background';
        TestEmailChoiceTxt: Label 'Choose the action that you would like to see a hint about:';
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The list of examples of CTF Challenges.
/// </summary>
page 50101 "CTF Challenges List"
{
    Caption = 'CTF Challenges List';
    PageType = ListPart;
    UsageCategory = Administration;
    SourceTable = "CTF Challenge";

    layout
    {
        area(Content)
        {

            repeater("CTF Challenges")
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
                        CTFChallenge: Interface "CTF Challenge";
                        ScenarioName: Text;
                    begin
                        CTFChallenge := Rec."CTF Challenge";
                        ScenarioName := Enum::"CTF Challenge".Names().Get(Rec."CTF Challenge".AsInteger());

                        case Rec."Entry Type" of
                            Rec."Entry Type"::RunCode:
                                RunChallenge(CTFChallenge, ScenarioName);
                            Rec."Entry Type"::Hint:
                                Message(CTFChallenge.GetHints());
                        end
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        CTFChallenges: Codeunit "CTF Challenges";
    begin
        CTFChallenges.Get(Rec);
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

    local procedure RunChallenge(CTFChallenge: Interface "CTF Challenge"; ScenarioName: Text)
    var
        ExampleStartDateTime: DateTime;
    begin
        ExampleStartDateTime := CurrentDateTime();
        CTFChallenge.RunChallenge();
        Message(ForegroundRunTxt, ScenarioName, CurrentDateTime() - ExampleStartDateTime);
    end;

    var
        Style: Text;
        Indentation: Integer;
        ForegroundRunTxt: Label 'Scenario ''%1'' took %2 to execute - where was the time spent?';
        BackgroundRun: Label 'Something is running in the background';
        TestEmailChoiceTxt: Label 'Choose the action that you would like to see a hint about:';
}
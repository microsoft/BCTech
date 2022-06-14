// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The list of CTF Challenges.
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
                    Caption = 'Challanges';
                    StyleExpr = Style;

                    trigger OnDrillDown()
                    var
                        CTFChallenge: Interface "CTF Challenge";
                        ScenarioName: Text;
                        HintIndex: Integer;
                    begin
                        case Rec."Entry Type" of
                            Rec."Entry Type"::RunCode:
                                begin
                                    CTFChallenge := Rec."CTF Challenge";
                                    ScenarioName := Enum::"CTF Challenge".Names().Get(Rec."CTF Challenge".AsInteger());
                                    RunChallenge(CTFChallenge, ScenarioName);
                                end;
                            Rec."Entry Type"::Hint:
                                begin
                                    CTFChallenge := Rec."CTF Challenge";
                                    Evaluate(HintIndex, Rec."Display Text".Split(' ').Get(2));
                                    Message(CTFChallenge.GetHints().Get(HintIndex));
                                end;
                        end
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        UpdateRec();
    end;

    internal procedure SetFilter(FilterText: Text)
    var
        CTFChallenges: Codeunit "CTF Challenges";
        IndexOfFilter: Integer;
        EnumOrdinal: Integer;
        CTFChallenge: Enum "CTF Challenge";
    begin
        IndexOfFilter := Rec."CTF Challenge".Names.IndexOf(FilterText);
        if IndexOfFilter > 0 then begin
            EnumOrdinal := Rec."CTF Challenge".Ordinals.Get(IndexOfFilter);
            CTFChallenge := Enum::"CTF Challenge".FromInteger(EnumOrdinal);
            CurrentFilter := CTFChallenge;
            IsFiltered := true;
            UpdateRec();
        end;
    end;

    local procedure UpdateRec()
    var
        CTFChallenges: Codeunit "CTF Challenges";
    begin
        if IsFiltered then
            CTFChallenges.Get(Rec, CurrentFilter)
        else
            CTFChallenges.Get(Rec);
    end;

    trigger OnAfterGetRecord()
    begin
        case Rec."Entry Type" of
            Rec."Entry Type"::Category:
                begin
                    Indentation := 0;
                    Style := 'Strong';
                end;
            Rec."Entry Type"::Name:
                begin
                    Indentation := 1;
                    Style := 'Standard';
                end;
            else begin
                    Indentation := 2;
                    Style := 'StandardAccent';
                end;
        end;
    end;

    local procedure RunChallenge(CTFChallenge: Interface "CTF Challenge"; ScenarioName: Text)
    var
        ChallengeStartDateTime: DateTime;
    begin
        ChallengeStartDateTime := CurrentDateTime();
        CTFChallenge.RunChallenge();
        Message(ForegroundRunTxt, ScenarioName, CurrentDateTime() - ChallengeStartDateTime);
    end;

    var
        Style: Text;
        Indentation: Integer;
        CurrentFilter: Enum "CTF Challenge";
        IsFiltered: Boolean;
        ForegroundRunTxt: Label 'Scenario ''%1'' took %2 to execute - where was the time spent?';
        BackgroundRun: Label 'Something is running in the background';
        TestEmailChoiceTxt: Label 'Choose the action that you would like to see a hint about:';
}
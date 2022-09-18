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
                    Caption = 'Challenges';
                    StyleExpr = Style;

                    trigger OnDrillDown()
                    var
                        CTFExternalHintVerification: Page "CTF External Hint Verification";
                        CTFChallenge: Interface "CTF Challenge";
                        ScenarioName: Text;
                        HintIndex: Integer;
                        HintText: Text;
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
                                    HintText := CTFChallenge.GetHints().Get(HintIndex);

                                    // Handle the verification from the external CTF portal
                                    if IsExternalCTFMode then begin
                                        CTFExternalHintVerification.SetHint(HintText);
                                        CTFExternalHintVerification.LookupMode := true;
                                        if CTFExternalHintVerification.RunModal() <> Action::LookupOK then
                                            exit;
                                    end;

                                    Message(HintText);
                                end;
                        end
                    end;
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        CTFChallengesSetup: Record "CTF Challenges Setup";
    begin
        CTFChallengesSetup.FindFirst();
        IsExternalCTFMode := CTFChallengesSetup."External Mode";
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
            IsFilterApplied := true;
            UpdateRec();
        end else
            Rec.DeleteAll();
    end;

    local procedure UpdateRec()
    var
        CTFChallenges: Codeunit "CTF Challenges";
    begin
        if IsExternalCTFMode then begin
            if IsFilterApplied then
                CTFChallenges.Get(Rec, CurrentFilter);
        end else
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
        if (CTFChallenge.GetCategory() = "CTF Category"::Performance) then
            Message(ForegroundRunTxt, ScenarioName, CurrentDateTime() - ChallengeStartDateTime);
    end;

    var
        Style: Text;
        Indentation: Integer;
        CurrentFilter: Enum "CTF Challenge";
        IsExternalCTFMode: Boolean;
        IsFilterApplied: Boolean;
        ForegroundRunTxt: Label 'Scenario ''%1'' took %2 to execute - where was the time spent?';
        BackgroundRun: Label 'Something is running in the background';
        TestEmailChoiceTxt: Label 'Choose the action that you would like to see a hint about:';
}
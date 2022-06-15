// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The main page for interacting with CTF challenges.
/// </summary>
page 50100 "CTF Challenges"
{
    Caption = 'CTF Challenges';
    PageType = Document;
    ApplicationArea = All;
    UsageCategory = Administration;

    layout
    {
        area(Content)
        {
            group(CTFChallengesBanner)
            {
                ShowCaption = false;
                Visible = not IsExternalMode;

                group(VerticalAlignment)
                {
                    ShowCaption = false;

                    label("Header")
                    {
                        ApplicationArea = All;
                        Caption = 'This page is used for demo & training purposes';
                        Style = Strong;
                    }
                    field(Description; CTFChallenegesBanner)
                    {
                        ApplicationArea = All;
                        Editable = false;
                        ShowCaption = false;
                        MultiLine = true;
                        Caption = ' ';
                    }
                }
            }
            group(CTFExtenralChallengesBanner)
            {
                ShowCaption = false;
                Visible = IsExternalMode;

                group(VerticalAlignmentExternalChallenge)
                {
                    ShowCaption = false;

                    label(HeaderExternalChallenge)
                    {
                        ApplicationArea = All;
                        Caption = 'This page is used for demo & training purposes';
                        Style = Strong;
                    }
                    field(DescriptionExternalChallenge; CTFExternalChallenegesBanner)
                    {
                        ApplicationArea = All;
                        Editable = false;
                        ShowCaption = false;
                        MultiLine = true;
                        Caption = ' ';

                    }
                }
            }
            grid(Scenarios)
            {
                ShowCaption = false;
                GridLayout = Columns;

                group(FilteredChallenges)
                {
                    ShowCaption = false;

                    field(FilterChallenge; FilterChallenge)
                    {
                        ApplicationArea = All;
                        Caption = 'Challenge Name';
                        Visible = IsExternalMode;

                        trigger OnValidate()
                        begin
                            CurrPage."CTF Challenges List".Page.SetFilter(FilterChallenge);
                            CurrPage."CTF Challenges List".Page.Update();
                        end;
                    }

                    part("CTF Challenges List"; "CTF Challenges List")
                    {
                        Visible = not (IsExternalMode and (FilterChallenge = ''));
                        Caption = 'Challenges';
                        ApplicationArea = All;
                    }
                }

                group(Help)
                {
                    ShowCaption = false;

                    label("LinkText")
                    {
                        ApplicationArea = All;
                        Caption = 'For help on how to troubleshoot see:';
                    }
                    field(LearnMore; PerfToolsTxt)
                    {
                        ApplicationArea = All;
                        Editable = false;
                        ShowCaption = false;
                        Caption = ' ';
                        ToolTip = 'View information about performance troubleshooting tools.';

                        trigger OnDrillDown()
                        begin
                            Hyperlink(PerfToolsUrlTxt);
                        end;
                    }
                    label("HintDescription")
                    {
                        ApplicationArea = All;
                        Caption = 'Each scenario also provides a hint that leads you in the right direction for that scenario.';
                    }
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        CTFChallenges: Codeunit "CTF Challenges";
    begin
        CTFChallenegesBanner := CTFChallenges.GetCTFChallenegesBanner();
        CTFExternalChallenegesBanner := CTFChallenges.GetExternalCTFChallenegesBanner();

        CTFChallengesSetup.Findfirst();
        IsExternalMode := CTFChallengesSetup."External Mode";
    end;

    var
        CTFChallengesSetup: Record "CTF Challenges Setup";
        PerfToolsTxt: Label 'How to Work with a Performance Problem';
        IsExternalMode: Boolean;
        CTFChallenegesBanner: Text;
        CTFExternalChallenegesBanner: Text;
        FilterChallenge: Text;
        PerfToolsUrlTxt: Label 'https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/performance/performance-work-perf-problem#which-tools-are-good-when';
}
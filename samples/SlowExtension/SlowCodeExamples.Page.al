// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides examples of slowly running code.
/// </summary>
page 50100 "Slow Code Examples"
{
    Caption = 'Slow Code Examples';
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    AboutTitle = 'About slow code examples';
    AboutText = 'Use the actions on this page to test performance troubleshooting tools.';

    actions
    {
        area(Processing)
        {
            action(MellowSpectator)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Caption = 'Mellow Spectator';

                trigger OnAction()
                var
                    PingPong: Codeunit "Ping Pong";
                begin
                    PingPong.Ping(0);
                end;
            }
            action(CraftyShark)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Caption = 'Crafty Shark';

                trigger OnAction()
                var
                    HeftySearch: Codeunit "Hefty Search";
                begin
                    HeftySearch.FindCustomerExtensions();
                end;
            }
            action(FeistyWings)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Caption = 'Feisty Wings';

                trigger OnAction()
                var
                    QuickTurnaround: Codeunit "Quick Turnaround";
                begin
                    QuickTurnaround.EasyComeEasyGo();
                end;
            }
            action(LongThunder)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Caption = 'Long Thunder';

                trigger OnAction()
                var
                    SessionId: Integer;
                begin
                    SetupFood();
                    Session.StartSession(SessionId, Codeunit::"Fridge Race");
                    Session.StartSession(SessionId, Codeunit::"Fridge Race");
                    Message(BackgroundRun);
                end;
            }
            action(SilverGambit)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Caption = 'Silver Gambit';

                trigger OnAction()
                var
                    FirstPersonJobQueue: Record "Job Queue Entry";
                    SecondPersonJobQueue: Record "Job Queue Entry";
                    SessionId: Integer;
                begin
                    SetupFood();

                    FirstPersonJobQueue."Parameter String" := 'Milk first';
                    SecondPersonJobQueue."Parameter String" := 'Cereal First';

                    Session.StartSession(SessionId, Codeunit::"What Goes First", CompanyName(), FirstPersonJobQueue);
                    Session.StartSession(SessionId, Codeunit::"What Goes First", CompanyName(), SecondPersonJobQueue);
                    Message(BackgroundRun);
                end;
            }
            action(Hint)
            {
                ApplicationArea = All;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;
                Caption = 'Hint';
                Image = Help;

                trigger OnAction()
                var
                    ActionChoicesLbl: Label 'Mellow Spectator,Crafty Shark,Feisty Wings,Long Thunder,Silver Gambit';
                    SelectedEmailChoice: Integer;
                begin
                    SelectedEmailChoice := StrMenu(ActionChoicesLbl, 1, TestEmailChoiceTxt);

                    case SelectedEmailChoice of
                        0:
                            exit;
                        1:
                            Message('Try using the performance profiler.');
                        2:
                            Message('Try checking long running queries in telemetry and the ''Database Missing Indexes'' page.');
                        3:
                            Message('Try using the performance profiler or check the telemetry for long running queries.');
                        4:
                            Message('Try checking deadlocks telemetry.');
                        5:
                            Message('Try checking lock timeout telemetry or the ''Database Locks'' page.');
                    end
                end;
            }
        }
    }

    local procedure SetupFood()
    var
        Milk: Record Milk;
        Cereal: Record Cereal;
    begin
        Milk.DeleteAll();
        Cereal.DeleteAll();

        Milk.Insert();
        Cereal.Insert();

        Commit();
    end;

    var
        BackgroundRun: Label 'Something is running in the background';
        TestEmailChoiceTxt: Label 'Choose the action that you would like to see a hint about:';
}
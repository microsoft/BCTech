// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The list of all CTF challenges.
/// </summary>
enum 50100 "CTF Challenge" implements "CTF Challenge"
{
    Extensible = false;
    Access = Internal;

    value(1; "Mellow Spectator")
    {
        Implementation = "CTF Challenge" = "Ping Pong";
    }

    value(2; "Feisty Wings")
    {
        Implementation = "CTF Challenge" = "Quick Turnaround";
    }

    value(3; "Long Thunder")
    {
        Implementation = "CTF Challenge" = "Fridge Race";
    }

    value(4; "Silver Gambit")
    {
        Implementation = "CTF Challenge" = "What Goes First";
    }

    value(5; "Why can't I see a contact entry on map?")
    {
        Implementation = "CTF Challenge" = VeryAnnoyingScenario;
    }

    value(6; "Why Can't I Post?")
    {
        Implementation = "CTF Challenge" = WhyCantIPost;
    }

    value(7; "What is going on with the customer list?")
    {
        Implementation = "CTF Challenge" = SubstituteReportChallenge;
    }

    value(8; "Intro challenge 1")
    {
        Implementation = "CTF Challenge" = IntroChallenge1;
    }

    value(9; "Intro challenge 2")
    {
        Implementation = "CTF Challenge" = IntroChallenge2;
    }

    value(10; "Blank Space")
    {
        Implementation = "CTF Challenge" = "Blank Space";
    }

    value(11; "Misconfiguration Package")
    {
        Implementation = "CTF Challenge" = "Misconfiguration Package";
    }

    value(12; "Security Check")
    {
        Implementation = "CTF Challenge" = "Security Check";
    }

    value(13; "Hidden Treasure")
    {
        Implementation = "CTF Challenge" = "Hidden Treasure";
    }

    value(14; "Quiet Widow")
    {
        Implementation = "CTF Challenge" = "Quiet Widow";
    }

    value(15; "Alien Dancer")
    {
        Implementation = "CTF Challenge" = "Alien Dancer";
    }

    value(16; "Why does my job queue entry fail?")
    {
        Implementation = "CTF Challenge" = JobQueueChallengeImpl;
    }

    value(17; "Challenging Action")
    {
        Implementation = "CTF Challenge" = ChallengingActionOnItems;
    }
}
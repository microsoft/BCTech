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

    value(7; "What is going on with the customer sales list?")
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

    value(12; "Quiet Widow")
    {
        Implementation = "CTF Challenge" = "Quiet Widow";
    }
}
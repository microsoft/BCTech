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
}
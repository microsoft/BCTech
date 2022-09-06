// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// The inteface for CTF challenges.
/// </summary>
interface "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge();
    procedure GetHints(): List of [Text];
    procedure GetCategory(): Enum "CTF Category";
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A permissions CTF challenge.
/// </summary>
codeunit 50149 JobQueueChallenge
{
    trigger OnRun()
    begin
        TableToInsert.Insert();
    end;

    var
        myInt: Integer;
        TableToInsert: Record TableToInsert;
}
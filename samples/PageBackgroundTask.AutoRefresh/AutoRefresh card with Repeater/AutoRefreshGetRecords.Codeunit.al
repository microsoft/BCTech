// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50111 "AutoRefresh GetRecords"
{
    trigger OnRun()
    var
        Sleep: Integer;
        Results: Dictionary of [Text, Text];
        AutoRefresh: Record AutoRefresh;
        TextBuilder: TextBuilder;
    begin
        // Waiting a bit, before fetching records
        Evaluate(Sleep, Page.GetBackgroundParameters().Get('Sleep'));
        Sleep(Sleep);

        // Getting all the 'AutoRefresh' records
        if AutoRefresh.FindSet() then
            repeat
                TextBuilder.Clear();
                TextBuilder.Append(Format(AutoRefresh.Date));
                TextBuilder.Append(',');
                TextBuilder.Append(Format(AutoRefresh.CreatedBySessionId));

                Results.Add(Format(AutoRefresh.Id), TextBuilder.ToText());
            until AutoRefresh.Next = 0;

        Page.SetBackgroundTaskResult(Results);
    end;
}
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
        RecAfter: Integer;
    begin
        // Waiting a bit, before fetching records
        Evaluate(Sleep, Page.GetBackgroundParameters().Get('Sleep'));
        Sleep(Sleep);

        // Getting new records only
        if (Page.GetBackgroundParameters().ContainsKey('RecAfter')) then begin
            Evaluate(RecAfter, Page.GetBackgroundParameters().Get('RecAfter'));
            AutoRefresh.SetFilter(Id, '>%1', RecAfter);
        end else
            RecAfter := 0;

        if AutoRefresh.FindSet() then
            repeat
                TextBuilder.Clear();
                TextBuilder.Append(Format(AutoRefresh.Date));
                TextBuilder.Append(',');
                TextBuilder.Append(Format(AutoRefresh.CreatedBySessionId));

                Results.Add(Format(AutoRefresh.Id), TextBuilder.ToText());

                if AutoRefresh.Id > RecAfter then
                    RecAfter := AutoRefresh.Id;
            until AutoRefresh.Next = 0;

        Results.Add('RecAfter', Format(RecAfter));
        Page.SetBackgroundTaskResult(Results);
    end;
}
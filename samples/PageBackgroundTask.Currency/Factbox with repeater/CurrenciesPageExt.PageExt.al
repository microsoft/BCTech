// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

pageextension 50101 "Currencies PageExt" extends Currencies
{
    layout
    {
        addlast(factboxes)
        {
            part("Latest rates"; "Latest Rates Factbox")
            {
                ApplicationArea = Basic, Suite;
                SubPageLink = Code = FIELD(Code);
            }

            part("Latest rates repeater"; "Latest Rates Factbox wRepeater")
            {
                ApplicationArea = Basic, Suite;
            }
        }
    }

    // For a factbox with a repeater, we must update it's content on the main page.
    // For the factbox without a repeater, the content is updated inside the factbox.
    trigger OnAfterGetCurrRecord()
    var
        PbtParameters: Dictionary of [Text, Text];
        Currency: Record Currency;
        DemoAlCurrencySetup: Record "PBT Currency Sample Setup";
        CurrenciesToRetrieveBuilder: TextBuilder;
    begin
        CurrPage."Latest rates repeater".Page.ResetTempTable();
        if (Code = '') then
            exit;

        Currency.SetFilter(Code, '<>%1', Code);
        if Currency.FindSet then begin
            repeat
                CurrenciesToRetrieveBuilder.Append(Currency.Code);
                CurrenciesToRetrieveBuilder.Append(',');
            until Currency.Next = 0;
        end;

        if CurrenciesToRetrieveBuilder.Length > 0 then
            CurrenciesToRetrieveBuilder.Length(CurrenciesToRetrieveBuilder.Length - 1);

        PbtParameters.Set('Date', 'latest');
        PbtParameters.Set('CurrencyBase', Code);
        PbtParameters.Set('Currencies', CurrenciesToRetrieveBuilder.ToText());
        DemoAlCurrencySetup.Get('SETUP');
        PbtParameters.Set('SleepSimulation', Format(DemoAlCurrencySetup.SleepDurationFactboxRepeater)); // Delay to simulate a slow HTTP call

        // Testability
        OnBeforePageBackgroundTaskSchedule(PbtParameters);

        CurrPage.EnqueueBackgroundTask(PbtTaskId, Codeunit::CurrencyRetriever, PbtParameters, 100000, PageBackgroundTaskErrorLevel::Warning);
    end;

    trigger OnPageBackgroundTaskCompleted(TaskId: Integer; Results: Dictionary of [Text, Text])
    begin
        if (TaskId = PbtTaskId) then begin
            // Adding the current currency as 1.0 (so it looks better)
            if (Results.Count > 0) then
                Results.Add(Code, '1.0');

            CurrPage."Latest rates repeater".Page.InitTempTable(Results);
        end;
    end;

    // For testability of the page.
    [IntegrationEvent(false, false)]
    local procedure OnBeforePageBackgroundTaskSchedule(var PbtParameters: Dictionary of [Text, Text])
    begin
    end;

    var
        PbtTaskId: Integer;
}


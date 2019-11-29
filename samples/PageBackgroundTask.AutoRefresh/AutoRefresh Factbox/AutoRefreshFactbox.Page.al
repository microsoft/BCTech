// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50105 "AutoRefresh Factbox"
{
    PageType = ListPart;
    InsertAllowed = false;
    DeleteAllowed = false;
    ModifyAllowed = false;
    LinksAllowed = false;
    Editable = false;
    RefreshOnActivate = false;
    Caption = 'AutoRefresh Page Background Task';

    layout
    {
        area(Content)
        {
            field(AutoRefreshValue; AutoRefreshValue)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Auto Refresh';
            }
        }
    }

    // Auto refresh must be restarted on OnAfterGetCurrRecord, because page background task is automatically
    // cancelled on page record change.
    trigger OnAfterGetCurrRecord()
    begin
        CurrPage.EnqueueBackgroundTask(PbtTaskId, Codeunit::"AutoRefresh Delay");
    end;

    trigger OnPageBackgroundTaskCompleted(TaskId: Integer; Results: Dictionary of [Text, Text])
    begin
        if (TaskId = PbtTaskId) then begin
            if (AutoRefreshValue = 'Ping') then
                AutoRefreshValue := 'Pong'
            else
                AutoRefreshValue := 'Ping';

            // Remark: Requires 2019 Wave 2 CU2 to work
            CurrPage.EnqueueBackgroundTask(PbtTaskId, Codeunit::"AutoRefresh Delay");
        end;
    end;

    var
        PbtTaskId: Integer;
        AutoRefreshValue: Text;
}

codeunit 50105 "AutoRefresh Delay"
{
    trigger OnRun()
    begin
        // Nothing to do, triggers update the factbox.
        // Remark: This sleep is taking one of the available child session concurrency slot for doing nothing.
        //         Be careful to not trigger too many of theses, otherwise you may exceed max concurency limits and tasks may be queued,
        //         degrading the end user experience.
        Sleep(2000);
    end;
}
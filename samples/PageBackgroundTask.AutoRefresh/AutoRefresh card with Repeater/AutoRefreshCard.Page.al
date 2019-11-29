// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

page 50110 "AutoRefresh Card"
{
    PageType = Card;
    ApplicationArea = Basic, Suite;
    UsageCategory = Lists;
    Caption = 'Auto Refresh Repeater Sample';

    layout
    {
        area(Content)
        {
            // The page background task will update that sub part.
            part(AutoRefreshList; "AutoRefresh Repeater Part")
            {
                ApplicationArea = Basic, Suite;
            }
        }
    }

    actions
    {
        area(Creation)
        {
            action(AddInBackgroundSession)
            {
                Caption = 'Create in background';
                ToolTip = 'Add new row in a background session';
                ApplicationArea = Basic, Suite;

                trigger OnAction()
                begin
                    StartSession(BackgroundSessionId, Codeunit::"AutoRefresh CreateRow");
                end;
            }
        }
        area(Processing)
        {
            action(EnqueueTooManyPBTs)
            {
                Caption = 'Enqueue too many PBTs';
                ToolTip = 'Will trigger an error - Prefectly normal';
                ApplicationArea = Basic, Suite;

                trigger OnAction()
                var
                    PbtParameters: Dictionary of [Text, Text];
                    i: Integer;
                begin
                    PbtParameters.Add('Sleep', '10');
                    for i := 1 to 200 do
                        CurrPage.EnqueueBackgroundTask(PbtTaskId, Codeunit::"AutoRefresh GetRecords", PbtParameters);
                end;
            }
        }
    }

    // Auto refresh must be started/restarted on OnAfterGetCurrRecord, because page background tasks are automatically
    // cancelled when the page current record change.
    trigger OnAfterGetCurrRecord()
    var
        PbtParameters: Dictionary of [Text, Text];
    begin
        PbtParameters.Add('Sleep', '1');
        CurrPage.EnqueueBackgroundTask(PbtTaskId, Codeunit::"AutoRefresh GetRecords", PbtParameters);
    end;

    trigger OnPageBackgroundTaskCompleted(TaskId: Integer; Results: Dictionary of [Text, Text])
    var
        PbtParameters: Dictionary of [Text, Text];
    begin
        if (TaskId = PbtTaskId) then begin
            // Results contains the records that has to be displayed.
            // Updating the sub page repeater with the new list of results.
            CurrPage.AutoRefreshList.Page.InitTempTable(Results);

            PbtParameters.Add('Sleep', '2000');
            // Remark: Requires 2019 Wave 2 CU2 to work
            CurrPage.EnqueueBackgroundTask(PbtTaskId, Codeunit::"AutoRefresh GetRecords", PbtParameters);
        end;
    end;

    trigger OnPageBackgroundTaskError(TaskId: Integer; ErrorCode: Text; ErrorText: Text; ErrorCallStack: Text; var IsHandled: Boolean)
    begin
        if (TaskId = PbtTaskId) then
            Error('The automatic refresh encountered an error. It will be disabled until you reload the page. Error:' + ErrorText);
    end;

    var
        PbtTaskId: Integer;
        BackgroundSessionId: Integer;
}
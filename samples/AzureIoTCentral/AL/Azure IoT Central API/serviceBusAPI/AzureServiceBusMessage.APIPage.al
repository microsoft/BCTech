// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 51002 "Az. Service Bus Messages"
{
    PageType = API;
    Caption = 'Azure Service Bus Messages';
    APIPublisher = 'IoT';
    APIGroup = 'AzureServiceBus';
    APIVersion = 'v2.0';
    EntityName = 'message';
    EntitySetName = 'messages';
    SourceTable = "Az. Service Bus Message";
    DelayedInsert = true;
    Extensible = false;
    SourceTableTemporary = true;

    layout
    {
        area(Content)
        {
            repeater(GroupName)
            {
                field(QueueName; "Queue Name")
                {
                    ApplicationArea = All;
                }
                field(MessageId; "Message Id")
                {
                    ApplicationArea = All;
                }
                field(ContentType; "Content Type")
                {
                    ApplicationArea = All;
                }
                field(Content; "Content")
                {
                    ApplicationArea = All;
                }
                field(CorrelationId; "Correlation Id")
                {
                    ApplicationArea = All;
                }
                field(Label; "Label")
                {
                    ApplicationArea = All;
                }
                field(LockToken; "Lock Token")
                {
                    ApplicationArea = All;
                }
                field(ReplyTo; "Reply To")
                {
                    ApplicationArea = All;
                }
                field(ReplyToSessionId; "Reply To Session Id")
                {
                    ApplicationArea = All;
                }
                field(ScheduledEnqueueTimeUtc; "Scheduled Enqueue Time Utc")
                {
                    ApplicationArea = All;
                }
                field(SessionId; "Session Id")
                {
                    ApplicationArea = All;
                }
                field(TimeToLive; "Time To Live")
                {
                    ApplicationArea = All;
                }
                field(To; "To")
                {
                    ApplicationArea = All;
                }
            }
        }
    }
}
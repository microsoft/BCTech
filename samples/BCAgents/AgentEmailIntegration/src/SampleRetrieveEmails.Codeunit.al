// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;

using System.Agents;
using System.Email;

codeunit 50102 "Sample Retrieve Emails"
{

    Access = Internal;
    TableNo = "Sample Setup";
    InherentEntitlements = X;
    InherentPermissions = X;
    Permissions = tabledata "Email Inbox" = rd;

    var
        AgentTaskTitleLbl: Label 'Email from %1', Comment = '%1 = Sender Name';
        MessageTemplateLbl: Label '<b>Subject:</b> %1<br/><b>Body:</b> %2', Comment = '%1 = Subject, %2 = Body';

    trigger OnRun()
    begin
        RunRetrieveEmails(Rec);
    end;

    /// <summary>
    /// Retrieves emails and creates/updates agent tasks.
    /// </summary>
    /// <param name="Setup">Agent setup record containing email account configuration.</param>
    procedure RunRetrieveEmails(Setup: Record "Sample Setup")
    var
        EmailInbox: Record "Email Inbox";
        TempFilters: Record "Email Retrieval Filters" temporary;
        Email: Codeunit "Email";
        LastSync: DateTime;
    begin
        LastSync := CurrentDateTime();

        SetEmailFilters(Setup, TempFilters);
        Email.RetrieveEmails(Setup."Email Account ID", Setup."Email Connector", EmailInbox, TempFilters);

        if EmailInbox.FindSet() then;

        repeat
            // Process each email as needed
            AddEmailToAgentTask(Setup, EmailInbox);
        until EmailInbox.Next() = 0;

        UpdateEarliestSyncTime(Setup, EmailInbox.Count(), LastSync);
        Commit();
    end;

    /// <summary>
    /// Updates the earliest sync datetime used for the next retrieval window.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <param name="EmailCount">Number of emails retrieved/processed in this run.</param>
    /// <param name="LastSync">Datetime of the last synchronization.</param>
    local procedure UpdateEarliestSyncTime(var Setup: Record "Sample Setup"; EmailCount: Integer; LastSync: DateTime)
    begin
        Setup.GetBySystemId(Setup.SystemId);

        // Only update the earliest sync time if we processed the fewer emails than the limit
        // This ensures we don't miss any emails in case there are more emails to process
        if EmailCount < MaxNoOfEmailsToProcess() then
            Setup."Earliest Sync At" := LastSync;
        Setup.Modify();
    end;

    /// <summary>
    /// Returns the maximum number of emails to retrieve/process per run.
    /// </summary>
    /// <returns>Maximum number of emails to process.</returns>
    local procedure MaxNoOfEmailsToProcess(): Integer
    begin
        exit(50);
    end;

    /// <summary>
    /// Builds the email retrieval filters.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <param name="TempFilters">Temporary retrieval filters record to populate.</param>
    local procedure SetEmailFilters(Setup: Record "Sample Setup"; var TempFilters: Record "Email Retrieval Filters" temporary)
    begin
        TempFilters."Unread Emails" := true;
        TempFilters."Load Attachments" := true;
        TempFilters."Last Message Only" := true;
        TempFilters."Earliest Email" := Setup."Earliest Sync At";
        TempFilters."Max No. of Emails" := MaxNoOfEmailsToProcess();
        TempFilters.Insert();
    end;

    /// <summary>
    /// Adds a retrieved email to an agent task (existing or new task).
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <param name="EmailInbox">Current email inbox record to process.</param>
    local procedure AddEmailToAgentTask(Setup: Record "Sample Setup"; var EmailInbox: Record "Email Inbox")
    var
        AgentTaskBuilder: Codeunit "Agent Task Builder";
    begin
        if AgentTaskBuilder.TaskExists(Setup."Agent User Security ID", EmailInbox."Conversation Id") then
            AddEmailToExistingTask(EmailInbox)
        else
            AddEmailToNewAgentTask(Setup, EmailInbox);

        MarkEmailAsProcessed(Setup, EmailInbox);
    end;

    /// <summary>
    /// Appends an email as a new message on an existing agent task.
    /// </summary>
    /// <param name="EmailInbox">Email to be appended.</param>
    local procedure AddEmailToExistingTask(var EmailInbox: Record "Email Inbox")
    var
        AgentTaskRecord: Record "Agent Task";
        AgentTaskMessage: Record "Agent Task Message";
        AgentTaskMessageBuilder: Codeunit "Agent Task Message Builder";
        EmailMessage: Codeunit "Email Message";
        MessageText: Text;
    begin
        AgentTaskRecord.ReadIsolation(IsolationLevel::ReadCommitted);
        AgentTaskRecord.SetRange("External ID", EmailInbox."Conversation Id");
        if not AgentTaskRecord.FindFirst() then
            exit;

        AgentTaskMessage.ReadIsolation(IsolationLevel::ReadCommitted);
        AgentTaskMessage.SetRange("Task ID", AgentTaskRecord.ID);
        AgentTaskMessage.SetRange("External ID", EmailInbox."External Message Id");
        if AgentTaskMessage.Count() >= 1 then
            exit;

        EmailMessage.Get(EmailInbox."Message Id");
        MessageText := StrSubstNo(MessageTemplateLbl, EmailMessage.GetSubject(), EmailMessage.GetBody());

        AgentTaskMessageBuilder.Initialize(EmailInbox."Sender Address", MessageText)
            .SetMessageExternalID(EmailInbox."External Message Id")
            .SetIgnoreAttachment(false)
            .SetAgentTask(AgentTaskRecord);

        AddEmailAttachmentsToTaskMessage(EmailMessage, AgentTaskMessageBuilder);
        AgentTaskMessageBuilder.Create();
    end;

    /// <summary>
    /// Creates a new agent task and adds the email as the initial task message.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <param name="EmailInbox">Email used to create the task.</param>
    local procedure AddEmailToNewAgentTask(Setup: Record "Sample Setup"; var EmailInbox: Record "Email Inbox")
    var
        AgentTaskRecord: Record "Agent Task";
        AgentTaskBuilder: Codeunit "Agent Task Builder";
        AgentTaskMessageBuilder: Codeunit "Agent Task Message Builder";
        EmailMessage: Codeunit "Email Message";
        MessageText: Text;
        AgentTaskTitle: Text[150];
    begin
        EmailMessage.Get(EmailInbox."Message ID");
        MessageText := StrSubstNo(MessageTemplateLbl, EmailMessage.GetSubject(), EmailMessage.GetBody());
        AgentTaskTitle := CopyStr(StrSubstNo(AgentTaskTitleLbl, EmailInbox."Sender Name"), 1, MaxStrLen(AgentTaskRecord.Title));

        AgentTaskMessageBuilder.Initialize(EmailInbox."Sender Address", MessageText)
            .SetMessageExternalID(EmailInbox."External Message ID")
            .SetIgnoreAttachment(false);

        AgentTaskBuilder.Initialize(Setup."Agent User Security ID", AgentTaskTitle)
            .SetExternalId(EmailInbox."Conversation Id")
            .AddTaskMessage(AgentTaskMessageBuilder);

        AddEmailAttachmentsToTaskMessage(EmailMessage, AgentTaskMessageBuilder);
        AgentTaskBuilder.Create();
    end;

    /// <summary>
    /// Marks the processed email as read in the external mailbox.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <param name="EmailInbox">Email containing the external message identifier.</param>
    local procedure MarkEmailAsProcessed(var Setup: Record "Sample Setup"; var EmailInbox: Record "Email Inbox")
    var
        Email: Codeunit "Email";
    begin
        Email.MarkAsRead(Setup."Email Account ID", Setup."Email Connector", EmailInbox."External Message Id");
    end;

    /// <summary>
    /// Adds all email attachments to the agent task message being built.
    /// </summary>
    /// <param name="EmailMessage">Email message containing attachments to add.</param>
    /// <param name="AgentTaskMessageBuilder">Builder instance to add the attachments to.</param>
    local procedure AddEmailAttachmentsToTaskMessage(var EmailMessage: Codeunit "Email Message"; var AgentTaskMessageBuilder: Codeunit "Agent Task Message Builder")
    var
        InStream: InStream;
        FileMIMEType: Text[100];
        IsFileMimeTypeSupported: Boolean;
        Ignore: Boolean;
    begin
        if not EmailMessage.Attachments_First() then
            exit;

        repeat
            EmailMessage.Attachments_GetContent(InStream);
            FileMIMEType := CopyStr(EmailMessage.Attachments_GetContentType(), 1, 100);
            // TODO: Add logic to check for supported MIME types
            IsFileMimeTypeSupported := true; // Placeholder for actual MIME type check
            Ignore := IsFileMimeTypeSupported; // Placeholder for actual logic to determine if attachment should be ignored even if file type is supported
            AgentTaskMessageBuilder.AddAttachment(EmailMessage.Attachments_GetName(), FileMIMEType, InStream, Ignore);
        until EmailMessage.Attachments_Next() = 0;
    end;
}
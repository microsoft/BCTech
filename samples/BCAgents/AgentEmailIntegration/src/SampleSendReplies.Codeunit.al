// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;
using System.Agents;
using System.Email;


codeunit 50103 "Sample Send Replies"
{

    Access = Internal;
    TableNo = "Sample Setup";
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnRun()
    begin
        SendEmailReplies(Rec);
    end;

    var
        AllSentSuccessfully: Boolean;
        EmailSubjectTxt: Label 'Agent reply to task %1', Comment = '%1 = Agent Task id';

    /// <summary>
    /// Sends email replies for reviewed output messages and marks them as sent.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    procedure SendEmailReplies(Setup: Record "Sample Setup")
    var
        OutputAgentTaskMessage: Record "Agent Task Message";
        InputAgentTaskMessage: Record "Agent Task Message";
        AgentMessage: Codeunit "Agent Message";
    begin
        AllSentSuccessfully := true;

        OutputAgentTaskMessage.ReadIsolation(IsolationLevel::ReadCommitted);
        OutputAgentTaskMessage.SetRange(Status, OutputAgentTaskMessage.Status::Reviewed);
        OutputAgentTaskMessage.SetRange(Type, OutputAgentTaskMessage.Type::Output);
        OutputAgentTaskMessage.SetRange("Agent User Security ID", Setup."Agent User Security ID");

        if not OutputAgentTaskMessage.FindSet() then
            exit;

        repeat
            if not InputAgentTaskMessage.Get(OutputAgentTaskMessage."Task ID", OutputAgentTaskMessage."Input Message ID") then
                continue;

            if InputAgentTaskMessage."External ID" = '' then
                continue;

            if TryReply(Setup, InputAgentTaskMessage, OutputAgentTaskMessage) then
                AgentMessage.SetStatusToSent(OutputAgentTaskMessage)
            else
                AllSentSuccessfully := false;
        until OutputAgentTaskMessage.Next() = 0;
        Commit();
    end;

    /// <summary>
    /// Returns whether all replies were sent successfully in the last run.
    /// </summary>
    /// <returns>True if all replies were sent successfully; otherwise false.</returns>
    procedure GetAllSentSuccessfully(): Boolean
    begin
        exit(AllSentSuccessfully);
    end;

    /// <summary>
    /// Attempts to create and send a reply email for an agent task output message.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <param name="InputAgentTaskMessage">The original inbound message being replied to.</param>
    /// <param name="OutputAgentTaskMessage">The outbound agent message to send.</param>
    /// <returns>True if the email was sent successfully; otherwise false.</returns>
    local procedure TryReply(Setup: Record "Sample Setup"; var InputAgentTaskMessage: Record "Agent Task Message"; var OutputAgentTaskMessage: Record "Agent Task Message"): Boolean
    var
        AgentMessage: Codeunit "Agent Message";
        Email: Codeunit Email;
        EmailMessage: Codeunit "Email Message";
        Body: Text;
        Subject: Text;
    begin
        Subject := StrSubstNo(EmailSubjectTxt, InputAgentTaskMessage."Task ID");
        Body := AgentMessage.GetText(OutputAgentTaskMessage);
        EmailMessage.CreateReplyAll(Subject, Body, true, InputAgentTaskMessage."External ID");
        AddMessageAttachments(EmailMessage, OutputAgentTaskMessage);

        exit(Email.ReplyAll(EmailMessage, Setup."Email Account ID", Setup."Email Connector"));
    end;

    /// <summary>
    /// Adds agent task message attachments to the email message being sent.
    /// </summary>
    /// <param name="EmailMessage">Email message to add attachments to.</param>
    /// <param name="AgentTaskMessage">Task message with the attachments to be included.</param>
    local procedure AddMessageAttachments(var EmailMessage: Codeunit "Email Message"; var AgentTaskMessage: Record "Agent Task Message")
    var
        AgentTaskFile: Record "Agent Task File";
        AgentTaskMessageAttachment: Record "Agent Task Message Attachment";
        AgentTaskFileInStream: InStream;
    begin
        AgentTaskMessageAttachment.SetRange("Task ID", AgentTaskMessage."Task ID");
        AgentTaskMessageAttachment.SetRange("Message ID", AgentTaskMessage.ID);
        if not AgentTaskMessageAttachment.FindSet() then
            exit;

        repeat
            if not AgentTaskFile.Get(AgentTaskMessageAttachment."Task ID", AgentTaskMessageAttachment."File ID") then
                exit;

            AgentTaskFile.CalcFields(Content);
            AgentTaskFile.Content.CreateInStream(AgentTaskFileInStream, TextEncoding::UTF8);
            EmailMessage.AddAttachment(AgentTaskFile."File Name", AgentTaskFile."File MIME Type", AgentTaskFileInStream);
        until AgentTaskMessageAttachment.Next() = 0;
    end;
}
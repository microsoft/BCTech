// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;

using System.Agents;

codeunit 50100 "Sample Dispatcher"
{

    Access = Internal;
    TableNo = "Sample Setup";
    InherentEntitlements = X;
    InherentPermissions = X;

    trigger OnRun()
    begin
        RunSampleAgent(Rec);
    end;

    /// <summary>
    /// Executes one agent cycle: retrieves emails, sends replies, and schedules the next run.
    /// </summary>
    /// <param name="Setup">Agent setup record.</param>
    procedure RunSampleAgent(Setup: Record "Sample Setup")
    var
        Sample: Codeunit "Sample";
        LastSync: DateTime;
        RetrievalSuccess: Boolean;
    begin
        // Validate task should still run
        if not Sample.ShouldRun(Setup) then
            exit;

        LastSync := CurrentDateTime();

        // Sync emails
        RetrievalSuccess := Codeunit.Run(Codeunit::"Sample Retrieve Emails", Setup);
        Codeunit.Run(Codeunit::"Sample Send Replies", Setup);

        // Reschedule next run
        Sample.ScheduleNextRun(Setup);

        if RetrievalSuccess then
            UpdateLastSync(Setup, LastSync);
    end;

    /// <summary>
    /// Persists the last successful synchronization datetime on the setup record.
    /// </summary>
    /// <param name="Setup">Setup record to update.</param>
    /// <param name="LastSync">Datetime to store as the last sync time.</param>
    local procedure UpdateLastSync(var Setup: Record "Sample Setup"; LastSync: DateTime)
    begin
        Setup.GetBySystemId(Setup.SystemId);
        Setup."Last Sync At" := LastSync;
        Setup.Modify();
        Commit();
    end;

}
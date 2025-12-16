// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;

using System.Agents;

codeunit 50104 "Sample"
{

    Access = Internal;
    InherentEntitlements = X;
    InherentPermissions = X;

    /// <summary>
    /// Determines whether the scheduled agent task should run for the given setup.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    /// <returns>True if the agent is active; otherwise false.</returns>
    procedure ShouldRun(Setup: Record "Sample Setup"): Boolean
    var
        Agent: Codeunit Agent;
    begin
        // TODO: Implement validation logic to determine if the agent should run
        if not Agent.IsActive(Setup."Agent User Security ID") then
            exit(false);

        exit(true);
    end;

    /// <summary>
    /// Schedules the next dispatcher execution.
    /// </summary>
    /// <param name="Setup">Setup record.</param>
    procedure ScheduleNextRun(Setup: Record "Sample Setup")
    begin
        // Ensure no other process can change the setup while we are scheduling the task
        Setup.LockTable();
        // Ensure we have the latest record before modifying
        Setup.GetBySystemId(Setup.SystemId);

        // Remove existing scheduled task if any before rescheduling
        RemoveScheduledTask(Setup);

        Setup."Scheduled Task ID" := TaskScheduler.CreateTask(Codeunit::"Sample Dispatcher", Codeunit::"Sample Error Handler", true, CompanyName(), CurrentDateTime() + ScheduleDelay(), Setup.RecordId);

        Setup.Modify();
        Commit();
    end;

    /// <summary>
    /// Cancels any previously scheduled tasks and clears task IDs on the setup record.
    /// </summary>
    /// <param name="Setup">Setup record containing scheduled task IDs.</param>
    procedure RemoveScheduledTask(Setup: Record "Sample Setup")
    var
        NullGuid: Guid;
    begin
        if TaskScheduler.TaskExists(Setup."Scheduled Task ID") then
            TaskScheduler.CancelTask(Setup."Scheduled Task ID");

        Setup."Scheduled Task ID" := NullGuid;
        Setup.Modify();
    end;

    /// <summary>
    /// Returns the delay (in milliseconds) used when scheduling the next run.
    /// </summary>
    /// <returns>Delay in milliseconds.</returns>
    local procedure ScheduleDelay(): Integer
    begin
        exit(2 * 60 * 1000); // 2 minutes
    end;
}
// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Microsoft.Agent.Sample;

codeunit 50101 "Sample Error Handler"
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
    /// Handles dispatcher failures by rescheduling the next run.
    /// </summary>
    /// <param name="Setup">Agent setup record.</param>
    procedure RunSampleAgent(Setup: Record "Sample Setup")
    var
        Sample: Codeunit "Sample";
    begin
        // Validate task should still run
        if not Sample.ShouldRun(Setup) then
            exit;

        // Reschedule run
        Sample.ScheduleNextRun(Setup);
    end;
}
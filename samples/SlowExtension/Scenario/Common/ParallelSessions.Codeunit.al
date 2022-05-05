// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides functions for managing background sessions
/// </summary>
codeunit 50107 "Parallel Sessions"
{
    Access = Internal;

    procedure WaitForSessionsToComplete(Sessions: List of [Integer])
    var
        ActiveSession: Record "Active Session";
        SessionsFilterString: Text;
        Session: Integer;
    begin
        foreach Session in Sessions do
            SessionsFilterString += Format(Session) + '|';
        SessionsFilterString := SessionsFilterString.TrimEnd('|');

        ActiveSession.SetRange("Server Instance ID", ServiceInstanceId());
        ActiveSession.SetFilter("Session ID", SessionsFilterString);

        while true do begin
            Sleep(500);
            if ActiveSession.IsEmpty() then
                exit;
        end;
    end;
}
codeunit 50148 JobQueueChallengeImpl implements "CTF Challenge"
{
    Access = Internal;

    trigger OnRun()
    begin

    end;


    procedure RunChallenge();

    var
        ScenarioLabel1: Label '1. Please create a job queue entry for the codeunit 50149 JobQueueChallenge.\';
        ScenarioLabel2: Label '2. Set the entry to ''Ready'' state and wait until it is executed.\';
        ScenarioLabel3: Label '3. Go to ''Job Queue Log Entries'' page and read the error message.\';
        ScenarioLabel4: Label '4. What has happened here?';
        ScenarioLabel5: Label '5. This challenge requires access to the file job_queue_query_data_demo.csv';
    begin
        Message(ScenarioLabel1 + ScenarioLabel2 + ScenarioLabel3 + ScenarioLabel4);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Check the Business Central telemetry from job_queue_query_data_demo.csv. Pay attention to what permission set operations have been recently performed on the tenant.';
        HintLine2: Label 'Has any of the permission sets been removed from the user? When did the job queue start erroring?';
        HintLine3: Label 'Check traces from 7th of September';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1);
        Hints.Add(HintLine2);
        Hints.Add(HintLine3);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Permissions);
    end;
}
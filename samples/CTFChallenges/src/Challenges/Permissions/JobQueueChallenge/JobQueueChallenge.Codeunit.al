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
    begin
        Message(ScenarioLabel1 + ScenarioLabel2 + ScenarioLabel3 + ScenarioLabel4);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Check the Business Central telemetry. Pay attention to what permission set operations have been recently performed on the tenant.';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Permissions);
    end;
}
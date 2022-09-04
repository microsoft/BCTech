codeunit 50143 SubstituteReportChallenge implements "CTF Challenge"
{
    Access = Internal;

    trigger OnRun()
    begin
    end;


    procedure RunChallenge();
    var
        ScenarioLabel: Label 'Please search for the ''Customer Sales List Report'' and preview it.\The challenge is to use snapshot debugging to figure out what is going on. Why a broken report is shown?';
    begin
        Message(ScenarioLabel);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'a. Create a ''Hello World '' app  and an ''Snapshot Attach Configuration'' in VSCode (The environment type should be Sandbox)\';
        HintLine2: Label 'b. Be sure you do not have any breakpoints defined in your demo app if you have not found an entry point and are relying on the AL exception to reveal the solution.\';
        HintLine3: Label 'c. This is important for capturing AL exceptions originating from event subscriptions.\';
        HintLine4: Label 'd. Initialize a snapshot debugging session.\';
        HintLine5: Label 'e. Exercise the challenge scenario. Do it at least two times (with loading the ''Customer List Report''). Did you get an error? If you got an error message that will be a snappoint.\';
        HintLine6: Label 'f. Download the snapshot.\';
        HintLine7: Label 'g. Start debugging the snapshot. The snappoint should reveal itself.\';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2 + HintLine3 + HintLine4 + HintLine5 + HintLine6 + HintLine7);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Debugging);
    end;
}
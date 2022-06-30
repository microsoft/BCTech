codeunit 50143 SubstituteReportChallenge implements "CTF Challenge"
{
    Access = Internal;
    
    trigger OnRun()
    begin       
    end;

   
    procedure RunChallenge();
    var       
     ScenarioLabel: Label 'Please preview the ''Customer List Report''.\The challenge is to use snapshot debugging to figure out what is going on. Why a broken report is shown.?'; 
    begin                            
        Message(ScenarioLabel);
        Report.Run(REport::"Customer - List");  
    end;

    procedure GetHints(): List of [Text];
    var  
     HintLine1: Label 'a. Use your ''Hello World'' app to start debugging. Don''t forget to add a dependency to the ''CTF Challenges'' app. And download symbols using the snapshot configuration.';  
     HintLine2: Label 'b. Use the event recorder to record all events that were triggered while running the preview for the ''Customer List Report''\';
     HintLine3: Label 'c. Look for suspicious events like the ones trying to substitute a report.\';
     HintLine4: Label 'd. Gotodefintion on this object and put a snappoint in the handler.\';
     HintLine5: Label 'e. Create a ''Snapshot Attach Configuration'' in VSCode (The environment type should be Sandbox)\';
     HintLine6: Label 'f. Initialize a snapshot debugging session.\';
     HintLine7: Label 'g. Exercise the challenge scenario.\';
     HintLine8: Label 'h. Download the snapshot.\';
     HintLine9: Label 'i. Start debugging the snapshot.\';
     Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2 + HintLine3 + HintLine4 + HintLine5 + HintLine6 + HintLine7 + HintLine8 + HintLine9);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Debugging);
    end;
}
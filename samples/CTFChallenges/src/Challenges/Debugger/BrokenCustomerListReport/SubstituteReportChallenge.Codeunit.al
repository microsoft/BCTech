codeunit 50143 SubstituteReportChallenge implements "CTF Challenge"
{
    Access = Internal;
    
    trigger OnRun()
    begin       
    end;

   
    procedure RunChallenge();
    var       
     ScenarioLabel: Label 'Please search for the ''Customer List Report'' and preview it.\The challenge is to use snapshot debugging to figure out what is going on. Why a broken report is shown.?'; 
    begin                            
        Message(ScenarioLabel);       
    end;

    procedure GetHints(): List of [Text];
    var  
     HintLine1: Label 'a. Use your ''Hello World'' app to start debugging. Don''t forget to add a dependency to the ''CTF Challenges'' app. And download symbols using the snapshot configuration.';      
     HintLine2: Label 'b. Create a ''Snapshot Attach Configuration'' in VSCode (The environment type should be Sandbox)\';
     HintLine3: Label 'd. Initialize a snapshot debugging session.\';
     HintLine4: Label 'e. Exercise the challenge scenario. Do it a few times (with loading the ''Customer List Report''). Dis you get an error? If you got an error message that will be a snappoint.\';     
     HintLine5: Label 'f. Download the snapshot.\';
     HintLine6: Label 'g. Start debugging the snapshot. The snappoint should reveal itself\';
     Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2 + HintLine3 + HintLine4 + HintLine5 + HintLine6);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
        exit(Enum::"CTF Category"::Debugging);
    end;
}
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
     HintLine1: Label 'a. Use your ''Hello World'' app to start debugging.\Don''t forget to add a dependency to the ''CTF Challenges'' app.\Also do not forget to  download symbols using the snapshot configuration. See below.\';      
     HintLine2: Label 'b. Create a ''Snapshot Attach Configuration'' in VSCode (The environment type should be Sandbox)\';
     HintLine3: Label 'c. Initialize a snapshot debugging session.\';
     HintLine4: Label 'd. Exercise the challenge scenario. Do it a few times (with loading the ''Customer List Report''). Did you get an error? If you got an error message that will be a snappoint.\';     
     HintLine5: Label 'e. Download the snapshot.\';
     HintLine6: Label 'f. Start debugging the snapshot. The snappoint should reveal itself\';
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
codeunit 50142 VeryAnnoyingScenario implements "CTF Challenge"
{
    trigger OnRun()    
    var        
        OnlineMapSetup: Record  "Online Map Setup";
    begin
       OnlineMApSetup.DeleteAll();     
    end;

    procedure RunChallenge();
    var
        Contact: Record Contact;
        ScenarioLabel1: Label 'Challenge1: Put a breakpoint on the Show Map action. It can be found on the ''Communication'' group\ \'; 
        ScenarioLabel2: Label 'Challenge2: Try showing on the map a contacts''s address.\Click on the ''Show Map''. Setup what is required, click again. What is blocking it?'; 
    begin
        Contact.SetRange(Contact."Territory Code", 'FOREIGN');
        Contact.FindFirst();
        PAGE.Run(Page::"Contact Card", Contact);
        Message(ScenarioLabel1 + ScenarioLabel2);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Use your ''Hello World'' app to start debugging.\Don''t forget to add a dependency to the ''CTF Challenges'' app.\Also do not forget to download symbols.\';
        HintLine2: Label 'While debugging, declare a variable for the ContactCard page and ''gotodefinition''. Or use an external tool that can perform a ''gotodefinition'', like ''AZ AL Dev Tools''.\';
        HintLine3: Label 'Rely on breakonReadWrite and breakOnError settings.';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1 + HintLine2  + HintLine3);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
         exit(Enum::"CTF Category"::Debugging);
    end;
}
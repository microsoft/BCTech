codeunit 50142 VeryyAnnoying implements "CTF Challenge"
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
        ScenarioLabel: Label 'Try shwoing on the map a contacs''s address (Show On the Map drilldown).\What is blocking it?'; 
    begin
        Contact.SetRange(Contact."Territory Code", 'FOREIGN');
        Contact.FindFirst();
        PAGE.Run(Page::"Contact Card", Contact);
    end;

    procedure GetHints(): List of [Text];
    var
        HintLine1: Label 'Rely on breakonReadWrite and breakOnError settings.';
        Hints: List of [Text];
    begin
        Hints.Add(HintLine1);
        exit(Hints);
    end;

    procedure GetCategory(): enum "CTF Category";
    begin
         exit(Enum::"CTF Category"::Debugging);
    end;
}
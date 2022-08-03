pageextension 50142 ContactCardCTFChallengeViewer extends "Contact Card"
{ 
    actions
    {
        addfirst(processing)
        {
            action(CTFChallenge)
            {
                ApplicationArea = RelationshipMgmt;
                Caption = 'CTF Challenge';               
                Promoted = true;
                PromotedCategory = Process;
                Scope = Repeater;                

                trigger OnAction()
                var
                     ScenarioLabel1: Label 'Challenge1: Put a breakpoint on the Show Map action. It can be found on the ''Communication'' group\ \'; 
                     ScenarioLabel2: Label 'Challenge2: Try showing on the map a contacts''s address.\Click on the ''Show Map''. Setup what is required, click again. What is blocking it?'; 
                begin
                     Message(ScenarioLabel1 + ScenarioLabel2);
                end;
            }
        }
    }    
}
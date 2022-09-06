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
                    ScenarioLabel1: Label 'Challenge: Try showing on the map a contacts''s address.\Click on the ''Show Map'' action. Setup what is required, by opening the ''Online Map Setup'' page. Then click again on the ''Show Map'' action. What is blocking it?';
                    ScenarioLabel2: Label 'You can rerun this action on the Contact card page by clicking on the CTF Challenge action found on the Process group';
                begin
                    Message(ScenarioLabel1 + ScenarioLabel2);
                end;
            }
        }
    }
}
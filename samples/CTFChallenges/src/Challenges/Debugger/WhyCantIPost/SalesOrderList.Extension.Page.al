pageextension 50140 SalesOrderCTFChallengeViewer extends "Sales Order List"
{
    actions
    {
        addfirst("P&osting")
        {
            action(CTFChallenge)
            {
                ApplicationArea = All;
                Caption = 'CTF Challenge';
                Promoted = true;
                PromotedCategory = Category7;
                Scope = Repeater;

                trigger OnAction()
                var
                    ScenarioLabel: Label 'Try selling an ANTWERP desk to ''The Cannon Group PLC'' using the ''Post and Send'' action.\The challenge is to figure out what is going on.  What is blocking posting of this sales order?';
                begin
                    Message(ScenarioLabel);
                end;
            }
        }
    }
}
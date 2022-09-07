pageextension 50149 "Items Challenging Action" extends "Item List"
{
    actions
    {
        addafter("Item Tracing")
        {
            action(CTFChallenge)
            {
                ApplicationArea = All;
                Caption = 'Challenging Action';
                Promoted = true;
                PromotedCategory = Category7;
                Scope = Repeater;

                trigger OnAction()
                var
                    ScenarioLabel: Label 'label';
                    TableToRead: Record TableToRead;
                begin
                    // Codeunit.Run(50149);
                    TableToRead.FindFirst();
                end;
            }
        }
    }
}
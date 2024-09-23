namespace CopilotToolkitDemo.SuggestJobBasic;

using Microsoft.Sales.Customer;

pageextension 54395 "SuggestJob - Customer Card Ext" extends "Customer Card"
{
    actions
    {
        addfirst(Category_New)
        {
            actionref(GenerateCopilotPromoted; GenerateCopilotAction)
            {
            }
        }

        addlast(Prompting)
        {
            action(GenerateCopilotAction)
            {
                Caption = 'Draft a project';
                Ellipsis = true;
                ApplicationArea = All;
                ToolTip = 'Lets Copilot generate a draft project for this customer based on your description.';

                trigger OnAction()
                var
                    SuggestJobProposal: Page "SuggestJob - Proposal";
                begin
                    // SuggestJobProposal.SetCustomer(Rec); // Not implemented
                    SuggestJobProposal.RunModal();
                end;
            }
        }
    }
}
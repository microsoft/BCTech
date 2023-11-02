namespace CopilotToolkitDemo.ItemSubstitution;

using Microsoft.Inventory.Item;
using Microsoft.Inventory.Item.Substitution;

pageextension 54320 "Item Subst. Entry Copilot" extends "Item Substitution Entry"
{
    actions
    {
        addbefore("&Condition")
        {
            action(SuggestItem)
            {
                Caption = 'Suggest with Copilot';
                Image = Sparkle;
                ApplicationArea = All;

                trigger OnAction()
                begin
                    SuggestSubstitutionsWithAI(Rec);
                end;
            }

        }
        addbefore("&Condition_Promoted")
        {
            actionref(SuggestItem_Promoted; SuggestItem) { }
        }
    }

    local procedure SuggestSubstitutionsWithAI(var ItemSubstitution: Record "Item Substitution");
    var
        Item: Record Item;
        ItemSubstAIProposal: Page "Copilot Item Sub Proposal";
    begin
        if Item.Get(ItemSubstitution.GetFilter("No.")) then begin
            ItemSubstAIProposal.SetSourceItem(Item);
            ItemSubstAIProposal.RunModal();
            CurrPage.Update(false);
        end;
    end;
}
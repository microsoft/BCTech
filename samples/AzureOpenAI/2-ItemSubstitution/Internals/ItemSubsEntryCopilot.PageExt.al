namespace CopilotToolkitDemo.ItemSubstitution;

using Microsoft.Inventory.Item;
using System.AI;
using Microsoft.Inventory.Item.Substitution;

pageextension 54320 "Item Subst. Entry Copilot" extends "Item Substitution Entry"
{
    actions
    {
        addLast(Prompting)
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

    }

    local procedure SuggestSubstitutionsWithAI(var ItemSubstitution: Record "Item Substitution");
    var
        Item: Record Item;
        ItemSubstAIProposal: Page "Copilot Item Sub Proposal";
        AzureOpenAI: Codeunit "Azure OpenAI";
    begin
        if Item.Get(ItemSubstitution.GetFilter("No.")) and (AzureOpenAI.IsEnabled("Copilot Capability"::"Find Item Substitutions", false)) then begin
            ItemSubstAIProposal.SetSourceItem(Item);
            ItemSubstAIProposal.RunModal();
            CurrPage.Update(false);
        end;
    end;
}
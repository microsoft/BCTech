namespace CopilotToolkitDemo.ItemSubstitution;

using Microsoft.Inventory.Item;

pageextension 54343 "Item Card Ext" extends "Item Card"
{
    actions
    {
        addfirst(Category_Process)
        {
            actionref(SuggestItem_Promoted; "Substituti&ons") { }
        }
    }
}
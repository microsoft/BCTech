namespace CopilotToolkitDemo.ItemSubstitution;
using Microsoft.Inventory.Item.Substitution;
using Microsoft.Inventory.Item;

page 54338 "Copilot Item Subs Proposal Sub"
{
    PageType = ListPart;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Copilot Item Sub Proposal";

    layout
    {
        area(Content)
        {
            repeater(ItemSubstDetails)
            {
                Caption = ' ';
                ShowCaption = false;

                field(Select; Rec.Select)
                {
                    ApplicationArea = All;
                }

                field("No."; Rec."No.")
                {
                    ApplicationArea = All;
                }
                field(Name; Rec.Description)
                {
                    ApplicationArea = All;
                }
                field(Explanation; Rec.Explanation)
                {
                    ApplicationArea = All;

                    trigger OnAssistEdit()
                    var
                        InStr: InStream;
                        FullExplanation: Text;
                    begin
                        Rec.CalcFields("Full Explanation");
                        Rec."Full Explanation".CreateInStream(InStr);
                        InStr.ReadText(FullExplanation);
                        Message(FullExplanation);
                        CurrPage.Update(false);
                    end;
                }
            }
        }
    }


    procedure Load(var TmpItemSubstAIProposal: Record "Copilot Item Sub Proposal" temporary)
    begin
        Rec.Reset();
        Rec.DeleteAll();

        TmpItemSubstAIProposal.Reset();
        if TmpItemSubstAIProposal.FindSet() then
            repeat
                TmpItemSubstAIProposal.CalcFields("Full Explanation");
                Rec.Copy(TmpItemSubstAIProposal, false);
                Rec."Full Explanation" := TmpItemSubstAIProposal."Full Explanation";
                Rec.Insert();
            until TmpItemSubstAIProposal.Next() = 0;

        CurrPage.Update(false);
    end;

    procedure SaveSubsForItem(Item: Record Item)
    var
        ItemSubstitution: Record "Item Substitution";
        ItemSubstAIProposal2: Record "Copilot Item Sub Proposal" temporary;
        LineNo, LineIncrem : Integer;
    begin
        ItemSubstAIProposal2.Copy(Rec, true);
        ItemSubstAIProposal2.SetRange(Select, true);

        if ItemSubstAIProposal2.FindSet() then
            repeat
                if not ItemSubstitution.Get(ItemSubstitution.Type::Item, Item."No.", '', ItemSubstitution."Substitute Type"::Item, ItemSubstAIProposal2."No.", '') then begin
                    ItemSubstitution.Init();
                    ItemSubstitution.Validate(Type, ItemSubstitution.Type::Item);
                    ItemSubstitution.Validate("No.", Item."No.");
                    ItemSubstitution.Validate("Substitute Type", ItemSubstitution."Substitute Type"::Item);
                    ItemSubstitution.Validate("Substitute No.", ItemSubstAIProposal2."No.");
                    ItemSubstitution.Insert(true);
                end;
            until ItemSubstAIProposal2.Next() = 0;
    end;
}
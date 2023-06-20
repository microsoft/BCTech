pageextension 50100 "Ai Item Card" extends "Item Card"
{
    actions
    {
        addfirst(Functions)
        {

            action(SuggestCategory)
            {
                ApplicationArea = All;
                Caption = 'Suggest Item Category';
                ToolTip = 'Suggests an item category based on the item description';
                Image = SparkleFilled;

                trigger OnAction()
                var
                    ItemCategory: Record "Item Category";
                    ConfirmMessage: Text;
                    SuggestedItemCategory: Code[20];
                begin
                    SuggestedItemCategory := SuggestItemCategory();
                    ItemCategory.Get(SuggestedItemCategory);

                    if ItemCategory.Description <> '' then
                        ConfirmMessage := StrSubstNo(ConfirmCategoryTxt, ItemCategory.Description, ItemCategory.Code)
                    else
                        ConfirmMessage := StrSubstNo(ConfirmCategoryCodeTxt, ItemCategory.Code);

                    if not Confirm(ConfirmMessage) then // RAI: human in the loop
                        exit;

                    Rec.Validate("Item Category Code", SuggestedItemCategory);
                end;
            }
        }

        addfirst(Category_Process)
        {
            actionref(SuggestCategory_Promoted; SuggestCategory)
            {
            }
        }
    }

    var
        CategoryPromptTxt: Label 'Given a list of item categories, pick one that would suit an item with the name ''%1''.\\Item categories: %2\Selected category:', Locked = true;
        ConfirmCategoryCodeTxt: Label 'The suggested item category is %1.\Update this item with the suggestion?', Comment = '%1 = the item category code';
        ConfirmCategoryTxt: Label 'The suggested item category is %1 (%2).\Update this item with the suggestion?', Comment = '%1 = the item category description, %2 = the item category code';

    local procedure SuggestItemCategory(): Code[20]
    var
        AzureOpenAi: Codeunit "Azure OpenAi";
        SuggestedCategory: Text;
        ItemCategories: Text;
        SuggestedCategoryCode: Code[20];
    begin
        ItemCategories := BuildCategoryList(); // RAI: pre-processing grounding (provide item categories)

        SuggestedCategory := AzureOpenAi.GenerateCompletion(StrSubstNo(CategoryPromptTxt, Rec.Description, ItemCategories), 1000, 0);

        SuggestedCategoryCode := ValidateCategory(SuggestedCategory); // RAI: post-processing grounding (prevent fabrication)

        exit(SuggestedCategoryCode);
    end;

    local procedure BuildCategoryList(): Text
    var
        ItemCategory: Record "Item Category";
        CategoryList: Text;
    begin
        ItemCategory.FindSet();

        repeat
            CategoryList += '- ' + ItemCategory.Code + '\';
        until ItemCategory.Next() = 0;

        exit(CategoryList);
    end;

    local procedure ValidateCategory(SuggestedCategory: Text): Code[20]
    var
        ItemCategory: Record "Item Category";
    begin
        ItemCategory.Get(SuggestedCategory);
        exit(ItemCategory.Code);
    end;
}
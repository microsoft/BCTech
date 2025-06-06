codeunit 50105 "Contoso Shoes BE Localization"
{
    [EventSubscriber(ObjectType::Codeunit, Codeunit::"Contoso Demo Tool", 'OnAfterGeneratingDemoData', '', false, false)]
    local procedure LocalizeShoesSizes(Module: Enum "Contoso Demo Data Module"; ContosoDemoDataLevel: Enum "Contoso Demo Data Level")
    var
        Item: Record Item;
        ContosoCoffeeDemoDataSet: Record "Contoso Coffee Demo Data Setup";
        ContosoShoeItemCategory: Codeunit "Contoso Shoes Item Category";
        CreateCountryRegion: Codeunit "Create Country/Region";
        ContosoShoesSize: Codeunit "Contoso Shoes Size";
        ContosoItem: Codeunit "Contoso Item";
    begin
        ContosoCoffeeDemoDataSet.Get();

        if Module = Enum::"Contoso Demo Data Module"::"Contoso Shoes" then
            if ContosoDemoDataLevel = Enum::"Contoso Demo Data Level"::"Master Data" then
                if ContosoCoffeeDemoDataSet."Country/Region Code" = CreateCountryRegion.BE() then begin
                    ContosoItem.SetOverwriteData(true);
                    Item.SetRange("Item Category Code", ContosoShoeItemCategory.Shoes());

                    if Item.FindSet() then
                        repeat
                            ContosoItem.InsertItemVariant(Item."No.", ContosoShoesSize.CM26(), this.EU42SizeDescriptionLbl);
                            ContosoItem.InsertItemVariant(Item."No.", ContosoShoesSize.CM27(), this.EU43SizeDescriptionLbl);
                        until Item.Next() = 0;
                    ContosoItem.SetOverwriteData(false);
                end;
    end;

    var
        EU42SizeDescriptionLbl: Label 'EU 42 Size', MaxLength = 30;
        EU43SizeDescriptionLbl: Label 'EU 43 Size', MaxLength = 30;
}
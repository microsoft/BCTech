codeunit 50104 "Contoso Shoes Size"
{
    trigger OnRun()
    var
        Item: Record Item;
        ContosoItem: Codeunit "Contoso Item";
        ContosoShoesItemCategory: Codeunit "Contoso Shoes Item Category";
    begin
        Item.SetRange("Item Category Code", ContosoShoesItemCategory.Shoes());

        if Item.FindSet() then
            repeat
                ContosoItem.InsertItemVariant(Item."No.", this.CM26(), this.UKSize85Lbl);
                ContosoItem.InsertItemVariant(Item."No.", this.CM27(), this.UKSize95Tok);
            until Item.Next() = 0;
    end;

    var
        Centimeter26Tok: Label 'CM26', MaxLength = 10;
        UKSize85Lbl: Label 'UK 8.5 Size', MaxLength = 30;
        Centimeter27Tok: Label 'CM27', MaxLength = 10;
        UKSize95Tok: Label 'UK 9.5 Size', MaxLength = 30;

    procedure CM26(): Code[10]
    begin
        exit(this.Centimeter26Tok);
    end;

    procedure CM27(): Code[10]
    begin
        exit(this.Centimeter27Tok);
    end;
}
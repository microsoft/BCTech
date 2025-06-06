codeunit 50103 "Contoso Shoes Item"
{
    trigger OnRun()
    var
        ContosoShoesUoM: Codeunit "Contoso Shoes Unit of Measure";
        ContosoShoesItemCategory: Codeunit "Contoso Shoes Item Category";
        CreatePostingGroups: Codeunit "Create Posting Groups";
        CreateInventoryPostingGroup: Codeunit "Create Inventory Posting Group";
        ContosoItem: Codeunit "Contoso Item";
        TempBlob: Codeunit "Temp Blob";
        GenProdPostingGroup, InventoryPostingGroup : Code[20];
    begin
        GenProdPostingGroup := CreatePostingGroups.RetailPostingGroup();
        InventoryPostingGroup := CreateInventoryPostingGroup.Resale();

        TempBlob := this.GetTempBlobFromFile(this.Sneaker() + '.jpeg');
        ContosoItem.InsertInventoryItem(this.Sneaker(), this.SneakersLbl, 49.99, 39.99, GenProdPostingGroup, '', InventoryPostingGroup, Enum::"Costing Method"::Standard,
            ContosoShoesUoM.Pair(), ContosoShoesItemCategory.Shoes(), 0.5, '', TempBlob);

        TempBlob := this.GetTempBlobFromFile(this.FlipFlop() + '.jpeg');
        ContosoItem.InsertInventoryItem(this.FlipFlop(), this.FlipFlopLbl, 19.99, 14.99, GenProdPostingGroup, '', InventoryPostingGroup, Enum::"Costing Method"::Standard,
            ContosoShoesUoM.Pair(), ContosoShoesItemCategory.Shoes(), 0.2, '', TempBlob);

        TempBlob := this.GetTempBlobFromFile(this.Boot() + '.jpeg');
        ContosoItem.InsertInventoryItem(this.Boot(), this.BootLbl, 89.99, 69.99, GenProdPostingGroup, '', InventoryPostingGroup, Enum::"Costing Method"::Standard,
            ContosoShoesUoM.Pair(), ContosoShoesItemCategory.Shoes(), 1.0, '', TempBlob);
    end;

    var
        SneakersTok: Label 'SNEAKER', MaxLength = 20;
        SneakersLbl: Label 'Contoso Sneaker', MaxLength = 100;
        FlipFlopTok: Label 'FLIPFLOP', MaxLength = 20;
        FlipFlopLbl: Label 'Contoso Flip Flop', MaxLength = 100;
        BootTok: Label 'BOOT', MaxLength = 20;
        BootLbl: Label 'Contoso Boot', MaxLength = 100;

    procedure Sneaker(): Code[20]
    begin
        exit(this.SneakersTok);
    end;

    procedure FlipFlop(): Code[20]
    begin
        exit(this.FlipFlopTok);
    end;

    procedure Boot(): Code[20]
    begin
        exit(this.BootTok);
    end;

    local procedure GetTempBlobFromFile(FilePath: Text) result: Codeunit "Temp Blob"
    var
        ObjInStream: InStream;
        OutStr: OutStream;
    begin
        NavApp.GetResource(FilePath, ObjInStream);
        result.CreateOutStream(OutStr);
        CopyStream(OutStr, ObjInStream);
    end;
}
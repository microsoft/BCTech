codeunit 50101 "Contoso Shoes Item Category"
{
    trigger OnRun()
    var
        ContosoItem: Codeunit "Contoso Item";
    begin
        ContosoItem.InsertItemCategory(this.Shoes(), this.ShoesLbl, '');
    end;

    var
        ShoesTok: Label 'SHOES', MaxLength = 20;
        ShoesLbl: Label 'Shoes', MaxLength = 100;

    procedure Shoes(): Code[20]
    begin
        exit(this.ShoesTok);
    end;
}
codeunit 50102 "Contoso Shoes Unit of Measure"
{
    trigger OnRun()
    var
        ContosoUoM: Codeunit "Contoso Unit of Measure";
    begin
        ContosoUoM.InsertUnitOfMeasure(this.Pair(), this.PairLbl, 'PR');
    end;

    var
        PairTok: Label 'PAIR', MaxLength = 10;
        PairLbl: Label 'Pair', MaxLength = 50;

    procedure Pair(): Code[10]
    begin
        exit(this.PairTok);
    end;
}
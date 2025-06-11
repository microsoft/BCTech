codeunit 50106 "Contoso Shoes Sales Order"
{
    trigger OnRun()
    var
        SalesHeader: Record "Sales Header";
        ContosoSales: Codeunit "Contoso Sales";
        ContosoShoesCustomer: Codeunit "Contoso Shoes Customer";
        ContosoShoesItem: Codeunit "Contoso Shoes Item";
    begin
        SalesHeader := ContosoSales.InsertSalesHeader(Enum::"Sales Document Type"::Order, ContosoShoesCustomer.ContosoShoesDomesticCustomer(), 'S-1', Today(), '');
        ContosoSales.InsertSalesLineWithItem(SalesHeader, ContosoShoesItem.Boot(), 1);

        SalesHeader := ContosoSales.InsertSalesHeader(Enum::"Sales Document Type"::Order, ContosoShoesCustomer.ContosoShoesDomesticCustomer(), this.PostYourReference(), Today(), Today(), '', '', Today(), '', '', 0D, '', '');
        ContosoSales.InsertSalesLineWithItem(SalesHeader, ContosoShoesItem.Boot(), 2);
        ContosoSales.InsertSalesLineWithItem(SalesHeader, ContosoShoesItem.Sneaker(), 1);
    end;

    procedure PostYourReference(): Code[35]
    begin
        exit(this.PostingYourReferenceTok);
    end;

    var
        PostingYourReferenceTok: Label 'POST', MaxLength = 35;
}
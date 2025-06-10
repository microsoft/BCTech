codeunit 50108 "Contoso Shoes Customer"
{
    trigger OnRun()
    var
        ContosoCoffeeDemoDataSetup: Record "Contoso Coffee Demo Data Setup";
        ContosoCustomerVendor: Codeunit "Contoso Customer/Vendor";
        CreateCustomerPostingGroup: Codeunit "Create Customer Posting Group";
        CreatePostingGroups: Codeunit "Create Posting Groups";
        CreateVATPostingGroups: Codeunit "Create VAT Posting Groups";
    begin
        ContosoCoffeeDemoDataSetup.Get();

        ContosoCustomerVendor.InsertCustomer(this.ContosoShoesDomesticCustomer(), this.ContosoShoesCustomerLbl, ContosoCoffeeDemoDataSetup."Country/Region Code", '', '', '', CreateCustomerPostingGroup.Domestic(), CreatePostingGroups.DomesticPostingGroup(), CreateVatPostingGroups.Domestic(), '', '', false);
    end;

    procedure ContosoShoesDomesticCustomer(): Code[20]
    begin
        exit('90000');
    end;

    var
        ContosoShoesCustomerLbl: Label 'Contoso Shoes Domestic Customer', MaxLength = 100;
}
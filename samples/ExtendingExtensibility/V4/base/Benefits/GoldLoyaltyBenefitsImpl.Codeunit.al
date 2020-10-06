/// <summary>
/// Implementation of ILoyaltyBenefits for the Gold level
/// </summary>
codeunit 50112 GoldLoyaltyBenefitsImpl implements ILoyaltyBenefits // , ILoyaltyBenefits2
{
    procedure GetDiscount(): Decimal;
    begin
        exit(5);
    end;

    procedure HasImplementation(): Boolean;
    begin
        exit(true);
    end;

    procedure HasFreeShipping(): Boolean;
    begin
        exit(true);
    end;
}
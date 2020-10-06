/// <summary>
/// Implementation of ILoyaltyBenefits for the Gold level
/// </summary>
codeunit 50112 GoldLoyaltyBenefitsImpl implements ILoyaltyBenefits
{
    procedure GetDiscount(): Decimal;
    begin
        exit(5);
    end;
}
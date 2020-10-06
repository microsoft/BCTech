/// <summary>
/// Implementation of ILoyaltyBenefits for the Bronze level
/// </summary>
codeunit 50110 BronzeLoyaltyBenefitsImpl implements ILoyaltyBenefits
{
    procedure GetDiscount(): Decimal;
    begin
        exit(0);
    end;
}
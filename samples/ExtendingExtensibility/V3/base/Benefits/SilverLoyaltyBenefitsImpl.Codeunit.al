/// <summary>
/// Implementation of ILoyaltyBenefits for the Silver level
/// </summary>
codeunit 50111 SilverLoyaltyBenefitsImpl implements ILoyaltyBenefits
{
    procedure GetDiscount(): Decimal;
    begin
        exit(2);
    end;
}
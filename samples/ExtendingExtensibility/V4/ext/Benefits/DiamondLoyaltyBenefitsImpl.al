/// <summary>
/// Implementation of ILoyaltyBenefits for the Diamond level
/// </summary>
codeunit 50113 DiamondLoyaltyBenefitsImpl implements ILoyaltyBenefits
{
    procedure GetDiscount(): Decimal;
    begin
        exit(10);
    end;
}
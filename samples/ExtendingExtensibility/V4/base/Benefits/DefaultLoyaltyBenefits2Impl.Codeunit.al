/// <summary>
/// Default implementation of ILoyaltyBenefits2
/// </summary>
codeunit 50109 DefaultLoyaltyBenefits2Impl implements ILoyaltyBenefits2
{
    procedure HasImplementation(): Boolean;
    begin
        exit(false);
    end;

    procedure HasFreeShipping(): Boolean;
    begin
        Error('We should never get here!')
    end;
}
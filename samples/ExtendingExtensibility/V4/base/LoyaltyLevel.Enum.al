/// <summary>
/// Customer Loyalty Level
/// </summary>
enum 50100 LoyaltyLevel implements ILoyaltyBenefits, ILoyaltyBenefits2
{
    Extensible = true;
    DefaultImplementation = ILoyaltyBenefits2 = DefaultLoyaltyBenefits2Impl;

    value(0; Bronze)
    {
        Implementation = ILoyaltyBenefits = BronzeLoyaltyBenefitsImpl;
    }
    value(10; Silver)
    {
        Implementation = ILoyaltyBenefits = SilverLoyaltyBenefitsImpl;
    }
    value(20; Gold)
    {
        Implementation = ILoyaltyBenefits = GoldLoyaltyBenefitsImpl;
    }
}
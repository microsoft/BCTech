/// <summary>
/// Customer Loyalty Level
/// </summary>
enum 50100 LoyaltyLevel implements ILoyaltyBenefits
{
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
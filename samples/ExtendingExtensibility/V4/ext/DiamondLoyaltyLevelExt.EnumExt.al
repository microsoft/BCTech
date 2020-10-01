/// <summary>
/// Adding the Diamond Loyalty Level.
/// </summary>
enumextension 50100 DiamondLoyaltyLevelExt extends LoyaltyLevel
{
    value(30; Diamond)
    {
        Implementation = ILoyaltyBenefits = DiamondLoyaltyBenefitsImpl;
    }
}
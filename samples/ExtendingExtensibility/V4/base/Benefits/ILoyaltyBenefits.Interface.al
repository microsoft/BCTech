/// <summary>
/// Loyalty Benefit(s) 
/// </summary>
interface ILoyaltyBenefits
{

    /// <summary>
    /// Get discount pct.
    /// </summary>
    /// <returns>Discount %</returns>
    procedure GetDiscount(): Decimal;
}
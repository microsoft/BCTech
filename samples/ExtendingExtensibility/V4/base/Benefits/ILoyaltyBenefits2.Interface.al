/// <summary>
/// More Loyalty Benefit(s) 
/// </summary>
interface ILoyaltyBenefits2
{
    /// <summary>
    /// Specifies if the implementation is not default.
    /// </summary>
    /// <returns>true if has implementation.</returns>
    procedure HasImplementation(): Boolean;

    /// <summary>
    /// Get whether the Loyalty level includes free shipping.
    /// </summary>
    /// <returns>true if we have free shipping</returns>
    procedure HasFreeShipping(): Boolean;
}
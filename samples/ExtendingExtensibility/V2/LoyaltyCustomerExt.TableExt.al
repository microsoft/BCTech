/// <summary>
/// Add the Loyalty fields to the Customer table.
/// </summary>
tableextension 50100 LoyaltyCustomerExt extends Customer
{
    fields
    {
        /// <summary>
        /// Customer loyalty.
        /// </summary>
        field(50100; Loyalty; enum LoyaltyLevel)
        {
        }
    }
}
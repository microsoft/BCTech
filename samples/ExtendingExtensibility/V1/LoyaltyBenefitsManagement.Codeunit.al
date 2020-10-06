/// <summary>
/// Manage Loyalty Benefits
/// </summary>
codeunit 50100 LoyaltyBenefitsManagement
{
    /// <summary>
    /// Adjust a Sales Order with loyalty level
    /// </summary>
    /// <param name="SalesHeader">Sales Header to adjust based on Customer Loyalty</param>
    procedure AdjustForLoyalty(SalesHeader: record "Sales Header")
    var
        Customer: record Customer;
        Discount: Decimal;
    begin
        Customer.Get(SalesHeader."Sell-to Customer No.");

        Discount := GetLoyaltyDiscount(Customer.Loyalty);
        ApplyDiscount(SalesHeader, Discount);
    end;

    /// <summary>
    /// Gets the discount based on Loyalty Level
    /// </summary>
    /// <param name="Loyalty">LOyalty Level</param>
    /// <returns>Discount %</returns>
    local procedure GetLoyaltyDiscount(Loyalty: enum LoyaltyLevel): Decimal;
    begin
        case Loyalty of
            LoyaltyLevel::Bronze:
                exit(0);
            LoyaltyLevel::Silver:
                exit(2);
            LoyaltyLevel::Gold:
                exit(5);
            else
                Error('Unknown LoyaltyLevel');
        end;
    end;

    /// <summary>
    /// Applies the Discount to the Sales Order
    /// </summary>
    /// <param name="SalesHeader">Sales Order</param>
    /// <param name="Discount">Discount to apply</param>
    local procedure ApplyDiscount(SalesHeader: record "Sales Header"; Discount: Decimal)
    begin
        // TODO: Implement
    end;
}

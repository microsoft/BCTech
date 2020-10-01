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
        LoyaltyBenefits: interface ILoyaltyBenefits;
        LoyaltyBenefits2: interface ILoyaltyBenefits2;
        Discount: Decimal;
    begin
        Customer.Get(SalesHeader."Sell-to Customer No.");

        LoyaltyBenefits := Customer.Loyalty;
        Discount := LoyaltyBenefits.GetDiscount();
        ApplyDiscount(SalesHeader, Discount);

        // LoyaltyBenefits2 := Customer.Loyalty;
        // if LoyaltyBenefits2.HasImplementation() then begin
        //     if LoyaltyBenefits2.HasFreeShipping() then
        //         ApplyFreeShipping(SalesHeader);
        // end;
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

    /// <summary>
    /// Applies free shipping to the Sales Order
    /// </summary>
    /// <param name="SalesHeader">Sales Order</param>
    local procedure ApplyFreeShipping(SalesHeader: record "Sales Header")
    begin
        // TODO: Implement
    end;
}

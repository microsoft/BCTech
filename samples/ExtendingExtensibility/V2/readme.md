## Customer Loyalty solution
### Non-extensible interface based version

The LoyaltyLevel enum specifies 3 different levels of Customer loyalty that maps two different benefits.

The `GetDiscount()` procedure in the LoyaltyBenefitsManagement codeunit is in this version replaced with an `ILoyaltyBenefits` interface that have an different implementation for each level.

The implementation codeunits are mapped to the interface in the enum values for each level. For the BronzeLevel it looks like this

``` AL
    value(0; Bronze)
    {
        Implementation = ILoyaltyBenefits = BronzeLoyaltyBenefitsImpl;
    }
```    
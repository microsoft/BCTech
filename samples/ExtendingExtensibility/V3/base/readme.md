## Customer Loyalty solution
### Extensible interface based version

This base app in this version is almost identical version 2. The only different is `Extensible = true` property on the `LoyaltyLevel` enum. This allows an extension (see the 'ext' folder) to add a new level - in the example the Diamond level.

The new level is added using the `enumextension` object type. The new value is required to specify an implementation of the `ILoyaltyBenefits` interface. 

After releasing the base version it will be a breaking change to make any changes to the `ILoyaltyBenefits` interface.
namespace Contoso.Loyalty;

/// <summary>
/// Pattern B — step 1 of 5. Partner's production master table; the *destination* of the
/// migration. Extensible = true so other partners can layer fields on top.
/// </summary>
table 50100 "Contoso Loyalty Tier"
{
    Caption = 'Loyalty Tier';
    DataClassification = CustomerContent;
    LookupPageId = "Contoso Loyalty Tiers";
    DrillDownPageId = "Contoso Loyalty Tiers";
    Extensible = true;

    fields
    {
        field(1; "Code"; Code[20]) { Caption = 'Code'; NotBlank = true; }
        field(2; "Description"; Text[100]) { Caption = 'Description'; }
        field(3; "Discount %"; Decimal)
        {
            Caption = 'Discount %';
            DecimalPlaces = 0 : 2;
            MinValue = 0;
            MaxValue = 100;
        }
    }

    keys
    {
        key(PK; "Code") { Clustered = true; }
    }
}

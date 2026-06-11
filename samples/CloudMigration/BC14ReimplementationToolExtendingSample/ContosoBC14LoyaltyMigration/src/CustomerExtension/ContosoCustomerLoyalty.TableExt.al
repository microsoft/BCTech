namespace Contoso.Loyalty;

using Microsoft.Sales.Customer;

/// <summary>
/// Pattern A — step 1 of 3. Adds the partner's "Loyalty Tier Code" field to the
/// *production* Customer table. The matching field must also be added to the BC14 Customer
/// buffer (see <see cref="tableextension::"Contoso BC14 Customer""/>) and copied across in a
/// subscriber (see <see cref="codeunit::"Contoso Customer Loyalty Sub""/>).
/// </summary>
tableextension 50100 "Contoso Customer Loyalty" extends Customer
{
    fields
    {
        field(50100; "Contoso Loyalty Tier Code"; Code[20])
        {
            Caption = 'Loyalty Tier Code';
            DataClassification = CustomerContent;
            TableRelation = "Contoso Loyalty Tier".Code;
        }
    }
}

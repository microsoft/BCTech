namespace Contoso.Loyalty;

/// <summary>
/// Simple lookup / list page for the production table. Not part of the migration plumbing —
/// included only so the migrated data can be browsed in the cloud tenant.
/// </summary>
page 50100 "Contoso Loyalty Tiers"
{
    PageType = List;
    SourceTable = "Contoso Loyalty Tier";
    UsageCategory = Lists;
    ApplicationArea = All;
    Caption = 'Loyalty Tiers';

    layout
    {
        area(Content)
        {
            repeater(Group)
            {
                field("Code"; Rec."Code") { ApplicationArea = All; }
                field("Description"; Rec."Description") { ApplicationArea = All; }
                field("Discount %"; Rec."Discount %") { ApplicationArea = All; }
            }
        }
    }
}

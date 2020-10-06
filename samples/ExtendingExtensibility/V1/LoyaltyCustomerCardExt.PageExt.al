/// <summary>
/// Adds the Loyalty field to the General group on the "Customer Card"
/// </summary>
pageextension 50100 LoyaltyCustCardExt extends "Customer Card"
{
    layout
    {
        addlast(General)
        {
            field(Loyalty; Rec.Loyalty) { }
        }
    }
}

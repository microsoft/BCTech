pageextension 50103 "FA ledger add analysis action" extends "FA Ledger Entries"
{

    actions
    {
        addlast(Reporting)
        {
            action("FixedAssetsAnalysis")
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Analyze Fixed Assets';
                Image = NonStockItem;
                RunObject = Query FixedAssetsAnalysis;
                ToolTip = 'Analyze (group, summarize, pivot) your Fixed Asset Ledger Entries with related Fixed Asset master data such as Fixed Asset, Asset Class/Subclass, and XXX.';
            }
        }

        addfirst(Category_Report)
        {
            actionref(FixedAssetsAnalysis_Promoted; FixedAssetsAnalysis)
            {
            }
        }
    }

}
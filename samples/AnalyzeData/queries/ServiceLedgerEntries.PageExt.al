pageextension 50105 ServiceLedgerEntries extends "Service Ledger Entries"
{
    actions
    {
        addlast(Reporting)
        {
            action(ServicesAnalysis)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Analyze Services';
                Image = ServiceAgreement;
                RunObject = Query ServiceAnalysis;
                ToolTip = 'Analyze (group, summarize, pivot) your Service Ledger Entries with related Service master data such as Service Contract, Customer, Item, G/L Account, and Job.';
            }
        }

        addfirst(Category_Report)
        {
            actionref(ServicesAnalysis_Promoted; ServicesAnalysis)
            {
            }
        }
    }
}
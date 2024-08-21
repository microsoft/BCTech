pageextension 50106 ServiceContractList extends "Service Contract List"
{
    actions
    {
        addfirst(Reporting)
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
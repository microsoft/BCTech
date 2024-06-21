pageextension 50100 JobLedgerEntries extends "Job Ledger Entries"
{
    actions
    {
        addlast(Reporting)
        {
            action(ProjectsAnalysis)
            {
                ApplicationArea = Basic, Suite;
                Caption = 'Analyze Projects';
                Image = Job;
                RunObject = Query ProjectsAnalysis;
                ToolTip = 'Analyze (group, summarize, pivot) your Project Ledger Entries with related Project master data such as Project Task, Resource, Item, and G/L Account.';
            }
        }

        addfirst(Category_Report)
        {
            actionref(ProjectsAnalysis_Promoted; ProjectsAnalysis)
            {
            }
        }
    }
}
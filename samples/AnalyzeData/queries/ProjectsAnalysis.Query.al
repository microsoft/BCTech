query 50130 ProjectsAnalysis
{
    QueryType = Normal;
    DataAccessIntent = ReadOnly;
    UsageCategory = ReportsAndAnalysis;
    Caption = 'Project Ad-hoc Analysis';
    AboutTitle = 'About Project Analysis Ad-hoc Analysis';
    AboutText = 'The Project Analysis a query that joins data from project ledger entries with Project master data. Use it to...';

    elements
    {
        dataitem(JobLedgerEntry; "Job Ledger Entry")
        {
            column(JobNo; "Job No.")
            {
                Caption = 'Project No.';
            }
            column(PostingDate; "Posting Date")
            {
                Caption = 'Posting Date';
            }
            column(DocumentNo; "Document No.")
            {
                Caption = 'Document No.';
            }
            column(DocumentDate; "Document Date")
            {
                Caption = 'Document Date';
            }
            column(Type; Type)
            {
                Caption = 'Type';
            }
            column(No; "No.")
            {
                Caption = 'No.';
            }
            column(Description; Description)
            {
                Caption = 'Description';
            }
            column(Quantity; Quantity)
            {
                Caption = 'Quantity';
            }
            column(DirectUnitCostLCY; "Direct Unit Cost (LCY)")
            {
                Caption = 'Direct Unit Cost (LCY)';
            }
            column(UnitCostLCY; "Unit Cost (LCY)")
            {
                Caption = 'Unit Cost (LCY)';
            }
            column(TotalCostLCY; "Total Cost (LCY)")
            {
                Caption = 'Total Cost (LCY)';
            }
            column(UnitPriceLCY; "Unit Price (LCY)")
            {
                Caption = 'Unit Price (LCY)';
            }
            column(TotalPriceLCY; "Total Price (LCY)")
            {
                Caption = 'Total Price (LCY)';
            }
            column(ResourceGroupNo; "Resource Group No.")
            {
                Caption = 'Resource Group No.';
            }
            column(UnitOfMeasureCode; "Unit of Measure Code")
            {
                Caption = 'Unit of Measure Code';
            }
            column(JobPostingGroup; "Job Posting Group")
            {
                Caption = 'Job Posting Group';
            }
            column(AmtToPostToGL; "Amt. to Post to G/L")
            {
                Caption = 'Amt. to Post to G/L';
            }
            column(AmtPostedToGL; "Amt. Posted to G/L")
            {
                Caption = 'Amt. Posted to G/L';
            }
            column(EntryType; "Entry Type")
            {
                Caption = 'Entry Type';
            }
            column(JournalBatchName; "Journal Batch Name")
            {
                Caption = 'Journal Batch Name';
            }
            column(ReasonCode; "Reason Code")
            {
                Caption = 'Reason Code';
            }
            column(TransactionType; "Transaction Type")
            {
                Caption = 'Transaction Type';
            }
            column(TransportMethod; "Transport Method")
            {
                Caption = 'Transport Method';
            }
            column(CountryRegionCode; "Country/Region Code")
            {
                Caption = 'Country/Region Code';
            }
            column(GenBusPostingGroup; "Gen. Bus. Posting Group")
            {
                Caption = 'Gen. Bus. Posting Group';
            }
            column(GenProdPostingGroup; "Gen. Prod. Posting Group")
            {
                Caption = 'Gen. Prod. Posting Group';
            }
            column(EntryExitPoint; "Entry/Exit Point")
            {
                Caption = 'Entry/Exit Point';
            }
            column(ExternalDocumentNo; "External Document No.")
            {
                Caption = 'External Document No.';
            }
            column(AreaCode; "Area")
            {
                Caption = 'Area';
            }
            column(TransactionSpecification; "Transaction Specification")
            {
                Caption = 'Transaction Specification';
            }
            column(NoSeries; "No. Series")
            {
                Caption = 'No. Series';
            }
            column(AdditionalCurrencyTotalCost; "Additional-Currency Total Cost")
            {
                Caption = 'Additional-Currency Total Cost';
            }
            column(AdditionalCurrencyTotalPrice; "Add.-Currency Total Price")
            {
                Caption = 'Add.-Currency Total Price';
            }
            column(AdditionalCurrencyLineAmount; "Add.-Currency Line Amount")
            {
                Caption = 'Add.-Currency Line Amount';
            }
            column(LineAmountLCY; "Line Amount (LCY)")
            {
                Caption = 'Line Amount (LCY)';
            }
            column(UnitCost; "Unit Cost")
            {
                Caption = 'Unit Cost';
            }
            column(TotalCost; "Total Cost")
            {
                Caption = 'Total Cost';
            }
            column(UnitPrice; "Unit Price")
            {
                Caption = 'Unit Price';
            }
            column(TotalPrice; "Total Price")
            {
                Caption = 'Total Price';
            }
            column(LineAmount; "Line Amount")
            {
                Caption = 'Line Amount';
            }
            column(LineDiscountAmount; "Line Discount Amount")
            {
                Caption = 'Line Discount Amount';
            }
            column(LineDiscountAmountLCY; "Line Discount Amount (LCY)")
            {
                Caption = 'Line Discount Amount (LCY)';
            }
            column(CurrencyCode; "Currency Code")
            {
                Caption = 'Currency Code';
            }
            column(CurrencyFactor; "Currency Factor")
            {
                Caption = 'Currency Factor';
            }
            column(LedgerEntryType; "Ledger Entry Type")
            {
                Caption = 'Ledger Entry Type';
            }
            column(LedgerEntryNo; "Ledger Entry No.")
            {
                Caption = 'Ledger Entry No.';
            }
            column(SerialNo; "Serial No.")
            {
                Caption = 'Serial No.';
            }
            column(LotNo; "Lot No.")
            {
                Caption = 'Lot No.';
            }
            column(LineDiscountPercent; "Line Discount %")
            {
                Caption = 'Line Discount %';
            }
            column(LineType; "Line Type")
            {
                Caption = 'Line Type';
            }
            column(GlobalDimension1Code; "Global Dimension 1 Code")
            {
                Caption = 'Global Dimension 1 Code';
            }
            column(GlobalDimension2Code; "Global Dimension 2 Code")
            {
                Caption = 'Global Dimension 2 Code';
            }
            column(ShortcutDimension3Code; "Shortcut Dimension 3 Code")
            {
                Caption = 'Shortcut Dimension 3 Code';
            }
            column(ShortcutDimension4Code; "Shortcut Dimension 4 Code")
            {
                Caption = 'Shortcut Dimension 4 Code';
            }
            column(ShortcutDimension5Code; "Shortcut Dimension 5 Code")
            {
                Caption = 'Shortcut Dimension 5 Code';
            }
            column(ShortcutDimension6Code; "Shortcut Dimension 6 Code")
            {
                Caption = 'Shortcut Dimension 6 Code';
            }
            column(ShortcutDimension7Code; "Shortcut Dimension 7 Code")
            {
                Caption = 'Shortcut Dimension 7 Code';
            }
            column(ShortcutDimension8Code; "Shortcut Dimension 8 Code")
            {
                Caption = 'Shortcut Dimension 8 Code';
            }
            dataitem(Job; Job)
            {
                DataItemLink = "No." = JobLedgerEntry."Job No.";
                column(JobDescription; Description)
                {
                    Caption = 'Project Description';
                }
                dataitem(JobTask; "Job Task")
                {
                    DataItemLink = "Job No." = JobLedgerEntry."Job No.", "Job Task No." = JobLedgerEntry."Job Task No.";
                    column(JobTaskNo; "Job Task No.")
                    {
                        Caption = 'Project Task No.';
                    }
                    column(JobTaskDescription; Description)
                    {
                        Caption = 'Project Task Description';
                    }
                    dataitem(WorkType; "Work Type")
                    {
                        DataItemLink = Code = JobLedgerEntry."Work Type Code";
                        column(WorkTypeCode; Code)
                        {
                            Caption = 'Work Type Code';
                        }
                        column(WorkTypeDescription; Description)
                        {
                            Caption = 'Work Type Description';
                        }
                        dataitem(Location; Location)
                        {
                            DataItemLink = "Code" = JobLedgerEntry."Location Code";
                            column(LocationCode; Code)
                            {
                                Caption = 'Location Code';
                            }
                            column(LocationName; Name)
                            {
                                Caption = 'Location Name';
                            }
                        }
                    }
                }
            }
        }
    }
}
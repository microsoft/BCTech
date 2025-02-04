query 50104 FixedAssetsAnalysis
{
    AboutTitle = 'About Fixed Assets Ad-hoc Analysis';
    AboutText = 'The Fixed Assets Analysis is a query that joins data from FA ledger entries with Fixed asset master data. Use it to reconcile at month end as an alternative to using the report ''Book Value 01''.';
    Caption = 'Fixed Assets Ad-hoc Analysis';
    //    ApplicationArea = All;
    DataAccessIntent = ReadOnly;
    QueryCategory = 'Fixed Assets', 'FA Ledger Entries';
    QueryType = Normal;
    UsageCategory = ReportsAndAnalysis;

    elements
    {
        dataitem(FALedgerEntry; "FA Ledger Entry")
        {
            column(No; "FA No.")
            {
                Caption = 'FA No.';
            }
            column(FAPostingDate; "FA Posting Date")
            {
                Caption = 'FA Posting Date';
            }
            column(PostingDate; "Posting Date")
            {
                Caption = 'Posting Date';
            }
            column(DocumentType; "Document Type")
            {
                Caption = 'Document Type';
            }
            column(DocumentDate; "Document Date")
            {
                Caption = 'Document Date';
            }
            column(DocumentNo; "Document No.")
            {
                Caption = 'Document No.';
            }
            column(Description; Description)
            {
                Caption = 'Description';
            }
            column(PostingCategory; "FA Posting Category")
            {
                Caption = 'Posting Category';
            }
            column(FAPostingType; "FA Posting Type")
            {
                Caption = 'Posting Type';
            }
            column(Amount; Amount)
            {
                Caption = 'Amount';
            }
            column(ReclassificationEntry; "Reclassification Entry")
            {
                Caption = 'Reclassification Entry';
            }
            column(FApostingGroup; "FA Posting Group")
            {
                Caption = 'FA Posting Group';
            }
            column(Depreciation_Method; "Depreciation Method")
            {
                Caption = 'Depreciation Method';
            }
            column(DepreciationStartingDate; "Depreciation Starting Date")
            {
                Caption = 'Depreciation Starting Date';
            }
            column(DepreciationEndingDate; "Depreciation Ending Date")
            {
                Caption = 'Depreciation Ending Date';
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
            dataitem(FixedAsset; "Fixed Asset")
            {
                DataItemLink = "No." = FALedgerEntry."FA No.";
                column(FADescription; Description)
                {
                    Caption = 'FA Description';
                }
                dataitem(FAClass; "FA Class")
                {
                    DataItemLink = Code = FALedgerEntry."FA Class Code";
                    column(ClassCode; Code)
                    {
                        Caption = 'Class Code';
                    }
                    column(ClassName; Name)
                    {
                        Caption = 'Class Name';
                    }
                    dataitem(FASubClass; "FA Subclass")
                    {
                        DataItemLink = Code = FALedgerEntry."FA Subclass Code";
                        column(SubClassCode; Code)
                        {
                            Caption = 'Subclass Code';
                        }
                        column(FASubClassName; Name)
                        {
                            Caption = 'Subclass Name';
                        }
                        dataitem(FALocation; "FA Location")
                        {
                            DataItemLink = Code = FALedgerEntry."FA Location Code";
                            column(FALocationCode; Code)
                            {
                                Caption = 'FA Location Code';
                            }
                            column(FALocationName; Name)
                            {
                                Caption = 'FA Location Name';
                            }
                            dataitem(DepreciationBook; "Depreciation Book")
                            {
                                DataItemLink = "Code" = FALedgerEntry."Depreciation Book Code";
                                column(DepBookCode; Code)
                                {
                                    Caption = 'Depreciation Book Code';
                                }
                                column(DepBookDescription; Description)
                                {
                                    Caption = 'Depreciation Book Desc.';
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
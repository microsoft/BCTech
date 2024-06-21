query 50100 ServiceAnalysis
{
    QueryType = Normal;
    DataAccessIntent = ReadOnly;
    UsageCategory = ReportsAndAnalysis;
    Caption = 'Service Ad-hoc Analysis';
    AboutTitle = 'About Service Analysis Ad-hoc Analysis';
    AboutText = 'The Service Analysis a query that joins data from service ledger entries with Service master data. Use it to...';

    elements
    {
        dataitem(ServiceLedgerEntry; "Service Ledger Entry")
        {
            column(ServiceContractNo; "Service Contract No.")
            {
                Caption = 'Service Contract No.';
            }
            column(DocumentType; "Document Type")
            {
                Caption = 'Document Type';
            }
            column(DocumentNo; "Document No.")
            {
                Caption = 'Document No.';
            }
            column(DocumentLineNo; "Document Line No.")
            {
                Caption = 'Document Line No.';
            }
            column(Type; "Type")
            {
                Caption = 'Type';
            }
            column(No; "No.")
            {
                Caption = 'No.';
            }
            column(Description; "Description")
            {
                Caption = 'Description';
            }
            column(MovedFromPrepaidAcc; "Moved from Prepaid Acc.")
            {
                Caption = 'Moved from Prepaid Acc.';
            }
            column(PostingDate; "Posting Date")
            {
                Caption = 'Posting Date';
            }
            column(AmountLCY; "Amount (LCY)")
            {
                Caption = 'Amount (LCY)';
            }
            column(ShipToCode; "Ship-to Code")
            {
                Caption = 'Ship-to Code';
            }
            column(ContractInvoicePeriod; "Contract Invoice Period")
            {
                Caption = 'Contract Invoice Period';
            }
            column(VariantCodeServiced; "Variant Code (Serviced)")
            {
                Caption = 'Variant Code (Serviced)';
            }
            column(ContractGroupCode; "Contract Group Code")
            {
                Caption = 'Contract Group Code';
            }
            column(CostAmount; "Cost Amount")
            {
                Caption = 'Cost Amount';
            }
            column(DiscountAmount; "Discount Amount")
            {
                Caption = 'Discount Amount';
            }
            column(UnitCost; "Unit Cost")
            {
                Caption = 'Unit Cost';
            }
            column(Quantity; "Quantity")
            {
                Caption = 'Quantity';
            }
            column(ChargedQty; "Charged Qty.")
            {
                Caption = 'Charged Qty.';
            }
            column(UnitPrice; "Unit Price")
            {
                Caption = 'Unit Price';
            }
            column(DiscountPercentage; "Discount %")
            {
                Caption = 'Discount %';
            }
            column(ContractDiscountAmount; "Contract Disc. Amount")
            {
                Caption = 'Contract Disc. Amount';
            }
            column(FaultReasonCode; "Fault Reason Code")
            {
                Caption = 'Fault Reason Code';
            }
            column(ServiceOrderType; "Service Order Type")
            {
                Caption = 'Service Order Type';
            }
            column(ServiceOrderNo; "Service Order No.")
            {
                Caption = 'Service Order No.';
            }
            column(UnitOfMeasureCode; "Unit of Measure Code")
            {
                Caption = 'Unit of Measure Code';
            }
            column(BinCode; "Bin Code")
            {
                Caption = 'Bin Code';
            }
            column(ResponsibilityCenter; "Responsibility Center")
            {
                Caption = 'Responsibility Center';
            }
            column(VariantCode; "Variant Code")
            {
                Caption = 'Variant Code';
            }
            column(EntryType; "Entry Type")
            {
                Caption = 'Entry Type';
            }
            column(Open; "Open")
            {
                Caption = 'Open';
            }
            column(ServicePriceAdjmtGrCode; "Serv. Price Adjmt. Gr. Code")
            {
                Caption = 'Serv. Price Adjmt. Gr. Code';
            }
            column(ServicePriceGroupCode; "Service Price Group Code")
            {
                Caption = 'Service Price Group Code';
            }
            column(Prepaid; "Prepaid")
            {
                Caption = 'Prepaid';
            }
            column(ApplyUntilEntryNo; "Apply Until Entry No.")
            {
                Caption = 'Apply Until Entry No.';
            }
            column(AppliesToEntryNo; "Applies-to Entry No.")
            {
                Caption = 'Applies-to Entry No.';
            }
            column(Amount; "Amount")
            {
                Caption = 'Amount';
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
            dataitem(Service_Contract_Header; "Service Contract Header")
            {
                DataItemLink = "Contract No." = ServiceLedgerEntry."Service Contract No.";
                DataItemTableFilter = "Contract Type" = const(Contract);
                column(ServiceContractDesc; Description)
                {
                    Caption = 'Service Contract Description';
                }
                dataitem(ServiceContractAccountGroup; "Service Contract Account Group")
                {
                    DataItemLink = Code = ServiceLedgerEntry."Serv. Contract Acc. Gr. Code";
                    column(ServiceContractAccGroupCode; "Code")
                    {
                        Caption = 'Service Contract Acc. Group Code.';
                    }
                    column(ServiceContractAccGroupDesc; Description)
                    {
                        Caption = 'Service Contract Acc. Group Desc.';
                    }
                    dataitem(Customer; Customer)
                    {
                        DataItemLink = "No." = ServiceLedgerEntry."Customer No.";
                        column(CustomerNo; "No.")
                        {
                            Caption = 'Customer No.';
                        }
                        column(CustomerName; Name)
                        {
                            Caption = 'Customer Name';
                        }
                        dataitem(BillToCustomer; Customer)
                        {
                            DataItemLink = "No." = ServiceLedgerEntry."Bill-to Customer No.";
                            column(BillToCustomerNo; "No.")
                            {
                                Caption = 'Bill-to Customer No.';
                            }
                            column(BillToCustomerName; Name)
                            {
                                Caption = 'Bill-to Customer Name';
                            }
                            dataitem(ItemServiced; Item)
                            {
                                DataItemLink = "No." = ServiceLedgerEntry."Item No. (Serviced)";
                                column(ItemServicedNo; "No.")
                                {
                                    Caption = 'Item No. (Serviced)';
                                }
                                column(ItemServicedDescription; Description)
                                {
                                    Caption = 'Item Description (Serviced)';
                                }
                                dataitem(ServiceItemServiced; "Service Item")
                                {
                                    DataItemLink = "No." = ServiceLedgerEntry."Service Item No. (Serviced)";
                                    column(ServiceItemServicedNo; "No.")
                                    {
                                        Caption = 'Service Item No. (Serviced)';
                                    }
                                    column(ServiceItemServicedDescription; Description)
                                    {
                                        Caption = 'Service Item Description (Serviced)';
                                    }

                                    dataitem(Job; Job)
                                    {
                                        DataItemLink = "No." = ServiceLedgerEntry."Job No.";
                                        column(JobDescription; Description)
                                        {
                                            Caption = 'Project Description';
                                        }
                                        dataitem(JobTask; "Job Task")
                                        {
                                            DataItemLink = "Job No." = ServiceLedgerEntry."Job No.", "Job Task No." = ServiceLedgerEntry."Job Task No.";
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
                                                DataItemLink = Code = ServiceLedgerEntry."Work Type Code";
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
                                                    DataItemLink = "Code" = ServiceLedgerEntry."Location Code";
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
                    }
                }
            }
        }
    }
}
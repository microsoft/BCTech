// Welcome to your new AL extension.
// Remember that object names and IDs should be unique across all extensions.
// AL snippets start with t*, like tpageext - give them a try and happy coding!

page 50100 TranslationTestPage
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = TranslationTestTable;
    Caption = 'Translation test page';

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                Caption = 'Below are some example fields for translation';
                field("Customer name"; "Customer name")
                {
                    ApplicationArea = All;
                    Caption = 'Customer name';

                }
                field(Balance; Balance)
                {
                    ApplicationArea = All;
                    Caption = 'Balance';
                }
                field("Total sales"; "Total sales")
                {
                    ApplicationArea = All;
                    Caption = 'Total Sales';
                }
                field("Phone no."; "Phone no.")
                {
                    ApplicationArea = All;
                    Caption = 'Phone no.';

                }
                field(Address; Address)
                {
                    ApplicationArea = All;
                    Caption = 'Address';

                }
                field(City; City)
                {
                    ApplicationArea = All;
                    Caption = 'City';
                }
            }
        }
    }

    var
        label1: Label 'Request Approval';
        label2: Label 'Prices and Discounts';
}
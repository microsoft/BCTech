page 50100 "Coffee Consumption Entries"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
#pragma warning disable AL0801
    // Table 'Coffee Consumption Entry' is marked to be moved. Reason: moved to separate Coffee Tracker app.. Tag: 1.1.0.0.
    SourceTable = "Coffee Consumption Entry";
#pragma warning restore AL0801

    layout
    {
        area(Content)
        {
            repeater(repeater1)
            {
                field("EntryNo."; Rec."EntryNo.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the coffee cup entry.';
                }
                field("Employee No."; Rec."Employee No.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the employee.';
                }
                field(TimeOfDay; Rec."Time Of Day")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the time of day the coffee was consumed.';
                }
                field("Number of Cups Consumed"; Rec."Number of Cups Consumed")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the number of cups consumed.';
                }
            }
        }
    }
}
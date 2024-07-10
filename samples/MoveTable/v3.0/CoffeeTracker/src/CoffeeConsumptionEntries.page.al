page 50100 "Coffee Consumption Entries"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Coffee Consumption Entry";

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
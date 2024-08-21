page 50101 "Employee Energy Level Entries"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Employee Energy Level Entry";

    layout
    {
        area(Content)
        {
            repeater(repeater)
            {
                field("EntryNo."; Rec."EntryNo.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the employee energy level entry.';
                }
                field("Employee No."; Rec."Employee No.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the unique identifier of the employee.';
                }
                field(TimeOfDay; Rec."Time Of Day")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the time of day when the energy level was recorded.';
                }
                field("Energy Level"; Rec."Energy Level")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the energy level of the employee.';
                }
            }
        }
    }
}
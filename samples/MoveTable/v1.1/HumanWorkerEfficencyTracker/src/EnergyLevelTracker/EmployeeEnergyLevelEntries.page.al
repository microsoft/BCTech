page 50101 "Employee Energy Level Entries"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
#pragma warning disable AL0801
    // Table 'Employee Energy Level Entry' is marked to be moved. Reason: moved to separate Energy Level Tracker app.. Tag: 1.1.0.0.
    SourceTable = "Employee Energy Level Entry";
#pragma warning restore AL0801

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
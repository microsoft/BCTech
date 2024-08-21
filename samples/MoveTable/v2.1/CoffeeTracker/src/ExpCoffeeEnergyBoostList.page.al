page 50120 "Exp. Coffee Energy Boost List"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
#pragma warning disable AL0801
    // Table 'Exp. Coffee Energy Boost' is marked to be moved. Reason: moving to Energy Level Tracker app.. Tag: 2.0.0.0.
    SourceTable = "Exp. Coffee Energy Boost";
#pragma warning restore AL0801

    layout
    {
        area(Content)
        {
            repeater(GroupName)
            {
                field("EmployeeNo."; Rec."EmployeeNo.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the employee number of the record.';
                }
                field("Number of Cups Consumed"; Rec."Number of Cups Consumed")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the number of cups consumed.';
                }
                field("Exp. Energy Level Boost"; Rec."Exp. Energy Level Boost")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the expected energy level boost.';
                }
            }
        }
    }
}
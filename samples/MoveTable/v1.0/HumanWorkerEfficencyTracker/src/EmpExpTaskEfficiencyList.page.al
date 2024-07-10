page 50130 "Emp. Exp. Task Efficiency List"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
    SourceTable = "Employee Exp. Task Efficiency";

    layout
    {
        area(Content)
        {
            repeater(GroupName)
            {
                field(EmployeeNo; Rec."EmployeeNo.")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the employee number.';
                }
                field(TaskCode; Rec."TaskCode")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the task code.';
                }
                field(ExpectedEfficiencyScore; Rec."Expected Efficiency Score")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the expected efficiency score.';
                }
            }
        }
    }
}
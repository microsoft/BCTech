page 50130 "Emp. Exp. Task Efficiency List"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Lists;
#pragma warning disable AL0801
    // Table 'Employee Exp. Task Efficiency' is marked to be moved. Reason: moving to Human Worker Efficiency Tracker app.. Tag: 3.1.0.0.    
    SourceTable = "Employee Exp. Task Efficiency";
#pragma warning restore AL0801

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
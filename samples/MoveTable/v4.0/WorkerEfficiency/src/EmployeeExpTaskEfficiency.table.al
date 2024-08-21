table 50130 "Employee Exp. Task Efficiency"
{
    DataClassification = CustomerContent;
    MovedFrom = '279534fe-6d71-4abe-81ee-ff81556ffd8c';

    fields
    {
        field(1; "EmployeeNo."; Code[20])
        {
            Caption = 'Employee No.';
            TableRelation = Employee."No.";
        }
        field(2; "TaskCode"; Code[20])
        {
            Caption = 'Task Code';
            TableRelation = Task.TaskCode;
        }
        field(3; "Expected Efficiency Score"; Integer)
        {
        }
    }

    keys
    {
        key(Key1; "EmployeeNo.", "TaskCode")
        {
            Clustered = true;
        }
    }
}
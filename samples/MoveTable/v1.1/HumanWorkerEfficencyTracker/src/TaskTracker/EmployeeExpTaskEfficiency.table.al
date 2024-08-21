table 50130 "Employee Exp. Task Efficiency"
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moving to separate Task Tracker app.';
    ObsoleteState = PendingMove;
    ObsoleteTag = '1.1.0.0';
    MovedTo = '279534fe-6d71-4abe-81ee-ff81556ffd8c';

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
#pragma warning disable AL0801
            // Table 'Task' is marked to be moved. Reason: moved to separate Task Tracker app.. Tag:
            TableRelation = Task.TaskCode;
#pragma warning restore AL0801
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
table 50120 "Exp. Coffee Energy Boost"
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moving to separate Coffee Tracker app.';
    ObsoleteState = PendingMove;
    ObsoleteTag = '1.1.0.0';
    MovedTo = 'e22bf1de-1ac3-4ef8-983e-945f50436d34';

    fields
    {
        field(1; "EmployeeNo."; Code[20])
        {
            Caption = 'Employee No.';
            TableRelation = Employee."No.";
        }
        field(2; "Number of Cups Consumed"; Integer)
        {
            MinValue = 0;
            MaxValue = 10;
        }
        field(3; "Exp. Energy Level Boost"; Integer)
        {
            InitValue = 0;
            MinValue = -7;
            MaxValue = 7;
        }
    }

    keys
    {
        key(Key1; "EmployeeNo.", "Number of Cups Consumed")
        {
            Clustered = true;
        }
    }
}
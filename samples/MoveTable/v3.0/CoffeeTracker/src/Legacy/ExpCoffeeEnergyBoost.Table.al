table 50120 "Exp. Coffee Energy Boost"
{
    DataClassification = CustomerContent;
    ObsoleteReason = 'moving to Energy Level Tracker app.';
    ObsoleteState = Moved;
    ObsoleteTag = '3.0.0.0';
    MovedTo = '1f9ab25f-1aaf-42dc-81cc-facfde1f942f';

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